namespace ParserLib

module CmdParser =

    open System
    open ParserDef

    //exit                                  --command without parameters/arguments                  +
    //help exit                             --command with one argument                             +
    //read -f "file.txt"                    --command with parameter with argument                  +
    //read -f "1.txt", read -f "2.txt"      --sequence of commands, results are combined in list    -
    //copy "1.txt", "2.txt", "3.txt"        --list of vales                                         -
    //read -f "1.txt" | write -f "2.txt"    --pipeline                                              +

    //"text"    --string literal    +
    //-a        --parameter         +
    //var       --item              +
    //|         --pipeline operator +
    //2.0       --number            +
    //true      --boolean           +

    type DataType =
        | TString
        | TNumber
        | TBoolean

    type Expression =
        | CNumber    of float
        | CString    of string
        | CBoolean   of bool
        | CVar       of string
        | CParameter of string
        | CArgument  of Expression
        | CList      of Expression list
        | CQuery     of Expression list
        | CPipeline  of Expression list
        | CCommand   of string * Expression
        | CExpr      of Expression
        | CEmpty


    //region: ParserForwardedToRef
    let createParserForwardedToRef<'a>() =
        let dummyParser= 
            let innerFn input : Result<'a * Input> = failwith "unfixed forwarded parser"
            { parseFn = innerFn; label = "unknown" }
    
        let parserRef = ref dummyParser 

        let innerFn input = 
            runOnInput !parserRef input 
        let wrapperParser = { parseFn = innerFn; label = "unknown" }
        wrapperParser, parserRef

    let cValue, cValueRef = createParserForwardedToRef<Expression>()
    //end_region: ParserForwardedToRef

    let idchar = satisfy (fun ch -> Char.IsLetterOrDigit ch) "idchar"
    let idstart = satisfy (fun ch -> Char.IsLetter ch || ch = '_') "idstart"

    let cId = 
        idstart .>>. manyChars idchar
        |>> (fun (c, str) -> (string c) + str)

    let cVar =
        cId
        |>> CVar

    let cType = 
        [ pstring "number"  >>% TNumber
          pstring "boolean" >>% TBoolean
          pstring "string"  >>% TString ]
        |> choice

    let cBool =
        let ctrue = (pstring "true")   >>% (CBoolean true)
        let cfalse = (pstring "false") >>% (CBoolean false)
        ctrue <|> cfalse
        <?> "bool"

    //region: String parsing
    let cUnescapedChar =
        satisfy (fun ch -> (ch <> '\\') && (ch <> '"')) "char"

    let cEscapedChar =
        [
            ("\\\"", '\"'); ("\\\\", '\\'); ("\\/", '/'); ("\\b", '\b')
            ("\\f", '\f'); ("\\r", '\r'); ("\\n", '\n'); ("\\t", '\t')
        ]
        |> List.map (fun (toMatch, result) -> pstring_c toMatch >>% result)
        |> choice
        <?> "escaped char"

    let cUnicodeChar =        
        let backslash = pchar_c '\\'
        let uChar = pchar_c 'u'
        let hexdigit = anyOf (['0'..'9'] @ ['A'..'F'] @ ['a'..'f'])
    
        let convertToChar (((h1,h2),h3),h4) = 
            let str = sprintf "%c%c%c%c" h1 h2 h3 h4
            Int32.Parse(str, Globalization.NumberStyles.HexNumber) |> char

        backslash  >>. uChar >>. hexdigit .>>. hexdigit .>>. hexdigit .>>. hexdigit
        |>> convertToChar
        <?> "char"

    let quotedString quote =
        let cchar = cUnescapedChar <|> cEscapedChar <|> cUnicodeChar
        quote >>. (manyChars cchar) .>> quote

    let doubleQuotedString =
        let quote = pchar '"'
        quotedString quote
        <?> "string"

    let singleQuotedString =
        let quote = pchar '\''
        quotedString quote
        <?> "string"

    let cString = 
        [ singleQuotedString; doubleQuotedString ] |> choice //fix -- parsing with '
        |>> CString
        <?> "string"
    //end_region: String parsing

    //region: Numbers parsing
    let optSign = opt (pchar_c '-')
    let optPlusMinus = opt (pchar_c '-' <|> pchar_c '+')

    let zero = pstring_c "0"
    let point = pchar_c '.'
    let exp = pchar_c 'e' <|> pchar_c 'E'

    let digitOneNine = 
        satisfy (fun ch -> Char.IsDigit ch && ch <> '0') "1-9"

    let digit =
        satisfy (fun ch -> Char.IsDigit ch) "digit"
        
    let convertToRNumber (((optSign,intPart),fractionPart),expPart) =
        let (|>?) opt f = 
            match opt with
            | None -> ""
            | Some x -> f x

        let signStr = 
            optSign 
            |>? string

        let fractionPartStr = 
            fractionPart 
            |>? (fun digits -> "." + digits )

        let expPartStr = 
            expPart 
            |>? fun (optSign, digits) ->
                let sign = optSign |>? string
                "e" + sign + digits

        let str = (signStr + intPart + fractionPartStr + expPartStr)
        str |> float |> CNumber

    let cNumber =
        let nonZeroInt = 
            digitOneNine .>>. manyChars digit
            |>> fun (first, rest) -> string first + rest

        let intPart = zero <|> nonZeroInt

        let fractionalPart = 
            point >>. manyChars1 digit

        let exponentPart =
            exp >>. optPlusMinus .>>. manyChars1 digit

        optSign
        .>>. intPart
        .>>. opt fractionalPart
        .>>. opt exponentPart
        |>> convertToRNumber
        <?> "number"
    //end_region: Numbers parsing

    let cLiteral =
        [ cNumber; cString; cBool ]
        |> choice

    let cList valueParser =
        let comma = pchar ',' .>> spaces
        sepBy1 valueParser comma
        <?> "list"

    let cArgument = 
        [ cNumber; cString; cBool; cVar (*add list*) ]
        |> choice
        |>> CArgument
        <?> "argument"

    let cParameter =
        let dash = pchar '-'
        dash >>. cId
        |>> CParameter
        <?> "parameter"

    let cCommand =
        let pquery = [ cParameter; cArgument; ] |> choice .>> spaces1

        cId .>>. opt (spaces1 >>. (many pquery))
        |>> (fun (id, optQuery) -> match optQuery with
                                   | Some q -> CCommand (id, CQuery q)
                                   | None -> CCommand (id, CEmpty))
        <?> "command"

    let cPipeline =
        let pipeline = pchar '|'

        let sepThenP = pipeline >>. spaces1 >>. cCommand
        cCommand .>>. many1 sepThenP
        |>> (fun (p, plist) -> p::plist)
        |>> CPipeline
        <?> "pipeline"

    let cExpr =
        [ cPipeline; cCommand; cBool; cString; cNumber ] |> choice
        |>> CExpr
        <?> "command"

    cValueRef := cExpr