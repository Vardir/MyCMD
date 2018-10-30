// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
module ParserDef

open System
open System.Collections.Generic

type Input = TextInput.InputState
type ParserLabel = string
type ParserError = string

type ParserPosition = {
    currentLine : string
    line : int
    column : int
}

type Result<'a> =
    | Success of 'a
    | Failure of ParserLabel * ParserError * ParserPosition
    
type Parser<'a> = { 
    parseFn: (Input -> Result<'a * Input>);
    label: ParserLabel
}

let runOnInput parser input =
    parser.parseFn input

let run parser inputStr =
    runOnInput parser (TextInput.fromStr inputStr)

let parserPositionFromInputState (inputState:Input) = {
    currentLine = TextInput.currentLine inputState
    line = inputState.position.line
    column = inputState.position.column
 }
        
let sprint result =
    match result with
    | Success (value, _) -> sprintf "%A" value
    | Failure (label, error, parserPos) -> 
        let errorLine = parserPos.currentLine
        let colPos = parserPos.column
        let linePos = parserPos.line
        let failureCaret = sprintf "%*s^%s" colPos "" error
        sprintf "Line:%i Col:%i Error parsing %s\n%s\n%s" (linePos+1) (colPos+1) label errorLine failureCaret 

///Ternary operator
let (?>) cond (a,b) = if cond then a else b
///Compare chars case-insensitive
let (!=!) c1 c2 = Char.ToLower(c1) = Char.ToLower(c2)

let (<?>) parser newLabel =
    let newInnerFn input = 
        match (parser.parseFn input) with
        | Success s -> Success s 
        | Failure (_, err, pos) -> Failure (newLabel, err, pos)     
    { parseFn = newInnerFn; label = newLabel }
    
let bindP fn parser =
    let innerFn input =
        match (runOnInput parser input) with
        | Failure (label, err, pos) -> Failure (label, err, pos)  
        | Success (value, remainingInput) -> runOnInput (fn value) remainingInput
    { parseFn = innerFn; label = parser.label }
    
let (>>=) parser fn = bindP fn parser 

let returnP x =
    let innerFn input = Success (x,input)
    { parseFn = innerFn; label = sprintf "%A" x} 

let mapP f = bindP (f >> returnP)

let (<!>) = mapP
let (<*>) fP xP =
    fP >>= (fun f -> xP >>= (fun x -> returnP (f x) ))
    
let lift2 f xP yP =
    returnP f <*> xP <*> yP

let (.>>.) parserA parserB =
    let label = sprintf "%s andThen %s" (parserA.label) (parserA.label)
    parserA >>= (fun p1Result -> 
                     parserB >>= (fun p2Result -> 
                                      returnP (p1Result,p2Result) ))
    <?> label

let (<|>) parserA parserB =
    let innerFn input =
        match (runOnInput parserA input) with
        | Success sc1 -> Success sc1
        | Failure _ ->
                  match (runOnInput parserB input) with
                  | Success sc2 -> Success sc2
                  | Failure (label, err, pos) -> Failure (label, err, pos)
    { parseFn = innerFn; label = sprintf "%s orElse %s" parserA.label parserB.label }

let (|>>) x f = mapP f x
   
let (.>>) parserA parserB =
    (parserA .>>. parserB) |>> (fun (a, _) -> a)
let (>>.) parserA parserB =
    (parserA .>>. parserB) |>> (fun (_, b) -> b)

let sequence parsersList =
    let concat p1 p2 = 
        p1 .>>. p2
        |>> (fun (lst1, lst2) -> lst1 @ lst2)
    
    parsersList 
    |> Seq.map (fun parser -> parser |>> List.singleton)
    |> Seq.reduce concat

let between parserA parserB parserC = parserA >>. parserB .>> parserC

let choice parsersList =
    parsersList |> List.reduce (<|>)
    
let rec parseZeroOrMore parser input =
    match (runOnInput parser input) with
    | Failure _ -> ([], input)
    | Success (fvalue, rem) ->
              let (subvalues, rems) = parseZeroOrMore parser rem
              (fvalue :: subvalues, rems)

let many parser =
    let innerFn input =
        Success (parseZeroOrMore parser input)
    { parseFn = innerFn; label = sprintf "manyOrZero %s" parser.label }

let many1 parser =
    parser >>= (fun head -> 
    many parser >>= (fun tail -> returnP (head::tail) ))
    <?> (sprintf "manyOr1 %s" parser.label)

let sepBy1 parser separator =
    let sepThenP = separator >>. parser
    parser .>>. many sepThenP
    |>> (fun (p, plist) -> p::plist)

let sepBy parser separator =
    (sepBy1 parser separator) <|> returnP []

///Satisfy char
let satisfy predicate label =
    let innerFn input =
        let (remainingInput, charOpt) = TextInput.nextChar input 
        match charOpt with
        | None -> 
            let pos = parserPositionFromInputState input
            Failure (label, "No more input", pos)
        | Some c -> if predicate c then Success (c, remainingInput)
                    else
                        let err = sprintf "Unexpected '%c'" c
                        let pos = parserPositionFromInputState input
                        Failure (label, err, pos)
    { parseFn = innerFn; label = label }
   
///Satisfy string
let satisfy_s (value:string) useCase label =
    let innerFn input =
        let rec iter i charOpt rem =
            let pos = parserPositionFromInputState rem
            match charOpt with
            | None -> Failure (label, "No more input", pos)
            | Some c -> if useCase ?> (value.[i] = c, value.[i] !=! c) then 
                            if i = value.Length - 1 then
                                Success (value, rem)
                            else
                                let (remainingInput, nchar) = TextInput.nextChar rem
                                iter (i + 1) nchar remainingInput
                        else 
                            let err = sprintf "Expected '%c' but got '%c'" value.[i] c
                            Failure (label, err, pos)
        let (remainingInput, charOpt) = TextInput.nextChar input
        iter 0 charOpt remainingInput
        
    { parseFn = innerFn; label = label }

///Satisfy any of chars
let satisfy_any (chars:Set<char>) label =
    let innerFn input =
        let (remainingInput, charOpt) = TextInput.nextChar input 
        match charOpt with
        | None -> let pos = parserPositionFromInputState input
                  Failure (label, "No more input", pos)
        | Some c -> if chars.Contains c then Success (c, remainingInput)
                    else
                        let err = sprintf "Unexpected '%c'" c
                        let pos = parserPositionFromInputState input
                        Failure (label, err, pos)
    { parseFn = innerFn; label = label }

///Parse char (case-sensitive)
let pchar_c charToMatch = 
    let label = sprintf "%c" charToMatch
    let predicate ch = (ch = charToMatch) 
    satisfy predicate label 

///Parse char (case-insensitive)
let pchar charToMatch =
    let label = sprintf "%c" charToMatch
    let predicate ch = ch !=! charToMatch
    satisfy predicate label
    
let anyOf listOfChars = 
    let label = sprintf "anyOf %A" listOfChars
    satisfy_any (Set.ofList listOfChars) label

let charListToStr charList =
    String(List.toArray charList) 

let manyChars charParser =
    many charParser
    |>> charListToStr

let manyChars1 charParser =
    many1 charParser
    |>> charListToStr

let opt parser = 
    let label = sprintf "opt %s" parser.label
    let some = parser |>> Some
    let none = returnP None
    (some <|> none) <?> label

///Parse string (case-sensitive)
let pstring_c str = satisfy_s str true str

///Parse string (case-insensitive)
let pstring str = satisfy_s str false str

let (>>%) parser x =
    parser |>> (fun _ -> x)

let whitespaceChar = 
    let predicate = Char.IsWhiteSpace
    satisfy predicate "whitespace"

let spaces = many whitespaceChar
let spaces1 = many1 whitespaceChar

let (|>?) opt (f, d) = 
    match opt with
    | None -> d
    | Some x -> f x