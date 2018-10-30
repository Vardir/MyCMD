module CExtern

open CParser
open ParserDef

let runParser (line : string) = 
    let result = run cCommand line
    match result with
    | Success (statement, _) -> statement
    | _ -> CEmpty