namespace ParserLib

module Interop =

    open CmdParser
    open ParserDef

    type ParserResult = {
        successfull : bool; expression : Expression; message : string
    }

    let runParser (line : string) = 
        let result = run cExpr line
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

    let extractParameter (expression : Expression) =
        match expression with
        | CParameter p -> p
        | _ -> raise (System.ArgumentException("argument is not a parameter"));

    let extractQuery (expression : Expression) =
        match expression with
        | CQuery lst -> Seq.ofList lst
        | _ -> raise (System.ArgumentException("argument is not a query"))

    let extractCmd (expression : Expression) =
        match expression with
        | CCommand (id, query) -> (id, query)
        | _ -> raise (System.ArgumentException("argument is not a command"))

    let extractPipeline (expression : Expression) =
        match expression with
        | CPipeline lst -> lst |> List.map (fun e -> extractCmd e) |> Seq.ofList
        | _ -> raise (System.ArgumentException("argument is not a pipeline"))
        
    let extractNumber (expression : Expression) =
        match expression with
        | CNumber n -> n
        | _ -> raise (System.ArgumentException("argument is not a number"))

    let extractString (expression : Expression) =
        match expression with
        | CString s -> s
        | _ -> raise (System.ArgumentException("argument is not a string"))

    let extractObject (expression : Expression) =
        match expression with
        | CNumber n -> box(n)
        | CString s -> s :> obj
        | _ -> raise (System.ArgumentException("argument can not be extracted"))
