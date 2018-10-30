module CExtern

open CParser
open ParserDef

type ParserResult = {
    successfull : bool; expression : Expression; message : string
}

let runParser (line : string) = 
    let result = run cCommand line
    match result with
    | Success (expr, _) -> { successfull = true; expression = expr; 
                             message = System.String.Empty }

    | Failure _ -> { successfull = false; expression = CEmpty; 
                                   message = sprint result }

let extractInnerExpression (expression : Expression) =
    match expression with
    | CExpr expr -> expr
    | CArgument expr -> expr
    | _ -> raise (System.ArgumentException("argument is not an expression container"))

let extractQuery (expression : Expression) =
    match expression with
    | CQuery lst -> Seq.ofList lst
    | _ -> raise (System.ArgumentException("argument is not a query"))

let extractCmd (expression : Expression) =
    match expression with
    | CCommand (id, query) -> (id, query)
    | _ -> raise (System.ArgumentException("argument is not a command"))

let extractBoolean (expression : Expression) =
    match expression with
    | CBoolean b -> b
    | _ -> raise (System.ArgumentException("argument is not a boolean"))

let extractNumber (expression : Expression) =
    match expression with
    | CNumber n -> n
    | _ -> raise (System.ArgumentException("argument is not a number"))

let extractString (expression : Expression) =
    match expression with
    | CString s -> s
    | _ -> raise (System.ArgumentException("argument is not a string"))

let extractVar (expression : Expression) =
    match expression with
    | CVar v -> v
    | _ -> raise (System.ArgumentException("argument is not a variable"))
