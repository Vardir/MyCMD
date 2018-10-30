module RParser

open System
open ParserDef

//create table tab1: t1 float, t2 integer, t3 text(30);
//create table tab2: a float, b integer, c text(30);
//insert tab1 values(17, 1, &#39;t&#39;);
//insert tab1 values(13.7, 15, &#39;text&#39;);
//insert tab1 values(13.7, 15, &#39;text&#39;);
//#length of text = 42, so it will be truncated to 30
//insert tab2 values(13.7, 15, &#39;lorem ipsum dolor sit amet ipsi at spectum&#39;);
//insert tab2 values(6, 11, &#39;lorem ipsum dolor sit amet&#39;);
//insert tab2 values(13.7, 15, &#39;text&#39;);
//table tab3 = tab1 union tab2;

type Id = string

type DataType =
    | TInteger
    | TFloat
    | TText
    | TBool

type Expression =
    | RInteger of int
    | RFloat of float
    | RText of string
    | RBool of bool
    | RConstraint of Expression
    | RColumnDef of Id * DataType * Expression
    | RList of Expression list
    | RTuple of Expression list
    | RUnion of string * string
    | REmpty
    
type Statement =
    | Script of Statement list
    | StTable of Id * Expression
    | StCreate of Id * Expression
    | StInsert of Id * Expression

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

let rValue, rValueRef = createParserForwardedToRef<Statement>()
//end_region: ParserForwardedToRef

let idchar = satisfy (fun ch -> Char.IsLetterOrDigit ch) "idchar"
let idstart = satisfy (fun ch -> Char.IsLetter ch || ch = '_') "idstart"

let rId = 
    idstart .>>. manyChars idchar
    |>> (fun (c, str) -> (string c) + str)

let rType = 
    [ pstring "integer" >>% TInteger 
      pstring "boolean" >>% TBool
      pstring "text"    >>% TText
      pstring "float"   >>% TFloat ]
    |> choice

let rBool =
    let rtrue = (pstring "true") >>% (RBool true)
    let rfalse = (pstring "false") >>% (RBool false)
    rtrue <|> rfalse
    <?> "bool"

//region: String parsing
let rUnescapedChar =
    satisfy (fun ch -> (ch <> '\\') && (ch <> '"')) "char"

let rEscapedChar =
    [
        ("\\\"", '\"'); ("\\\\", '\\'); ("\\/", '/'); ("\\b", '\b')
        ("\\f", '\f'); ("\\r", '\r'); ("\\n", '\n'); ("\\t", '\t')
    ]
    |> List.map (fun (toMatch, result) -> pstring_c toMatch >>% result)
    |> choice
    <?> "escaped char"

let rUnicodeChar =        
    let backslash = pchar_c '\\'
    let uChar = pchar_c 'u'
    let hexdigit = anyOf (['0'..'9'] @ ['A'..'F'] @ ['a'..'f'])
    
    let convertToChar (((h1,h2),h3),h4) = 
        let str = sprintf "%c%c%c%c" h1 h2 h3 h4
        Int32.Parse(str, Globalization.NumberStyles.HexNumber) |> char

    backslash  >>. uChar >>. hexdigit .>>. hexdigit .>>. hexdigit .>>. hexdigit
    |>> convertToChar
    <?> "char"

let quotedString =
    let quote = pchar_c '\"'
    let rchar = rUnescapedChar <|> rEscapedChar <|> rUnicodeChar
    quote >>. manyChars rchar .>> quote
    <?> "string"

let rString = 
    quotedString
    |>> RText
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
    if fractionPart.IsNone then str |> int |> RInteger
    else str |> float |> RFloat

let rNumber =
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

let rLiteral =
    [ rNumber; rString; rBool ]
    |> choice

let rTypeConstaint =
    let left = pchar '(' .>> spaces
    let right = pchar ')'
    let size = rNumber .>> spaces

    opt (between left size right)
    |>> (fun x -> match x with
                  | None -> RConstraint REmpty
                  | Some expr -> RConstraint expr)
    <?> "constraint"

let rList valueParser =
    let comma = pchar ',' .>> spaces
    sepBy1 valueParser comma
    <?> "list"

let rTuple =
    let left = pchar '(' .>> spaces
    let right = pchar ')'
    let value = rLiteral .>> spaces

    between left (rList value) right
    |>> RTuple
    <?> "tuple"

let rDefList =
    let typeDef = rType .>> spaces .>>. rTypeConstaint
    let item = rId .>> spaces1 .>>. typeDef
    rList item
    |>> (fun p -> p |> List.map (fun (id, (t, c)) -> RColumnDef (id, t, c)))
    <?> "def_list"
    
let rCreate =
    let create = pstring "create"
    let table = pstring "table"
    let colon = pchar ':'
    let terminator = pchar ';'

    let left = create >>. spaces1 >>. table >>. spaces1 >>. rId .>> spaces
    let right = colon >>. spaces >>. rDefList .>> spaces .>> terminator
    left .>> spaces .>>. right
    |>> (fun (id, defs) -> (id, RList defs))
    |>> StCreate
    <?> "create_table"

let rInsert =
    let insert = pstring "insert"
    let values = pstring "values"
    let terminator = pchar ';'

    insert >>. spaces1 >>. rId .>> spaces1 .>> values .>>. rTuple .>> spaces .>> terminator
    |>> StInsert
    <?> "insert_in"

let rUnion =
    let union = pstring "union"

    rId .>> spaces1 .>> union .>> spaces1 .>>. rId
    |>> RUnion
    <?> "union"

let rTable =
    let table = pstring "table"
    let equals = pchar '='
    let terminator = pchar ';'
    let operator = [ rUnion ] |> choice

    let left = table >>. spaces1 >>. rId .>> spaces .>> equals .>> spaces
    let right = operator .>> spaces .>> terminator
    left .>>. right
    |>> StTable
    <?> "table"

let rScript =
    let start = pstring "start"
    let sEnd = pstring "end"

    let statements = [ rCreate; rInsert; rTable ] |> choice .>> spaces
    start >>. spaces1 >>. (many statements) .>> spaces .>> sEnd
    |>> Script
    <?> "script"

rValueRef := rScript