module RInterpreter

open RParser
open System
open System.Collections.Generic

type Result<'a> =
    | Success of 'a
    | Failure of string

type Column = {    
    id : string;
    dataType : DataType;
    fieldConstraint : Expression
}

type Row = Row of List<obj>

type Table = {
    id : string;
    columns : List<Column>;
    rows : List<Row>
}

let (>>?) (value:obj) dataType =
    let matches = match dataType with
                  | TBool -> value :? bool
                  | TFloat -> value :? float
                  | TInteger -> value :? int
                  | TText -> value :? string
    if matches then Some value
    else None

let createColumn expr =
    match expr with
    | RColumnDef (id, t, constr) -> { id = id; dataType = t; fieldConstraint = constr}
    | _ -> failwith "invalid context"

let extractColumns expr =
    match expr with
    | RList exprs -> exprs
                     |> List.map (fun e -> match e with
                                           | RColumnDef _ -> createColumn e
                                           | _ -> failwith "invalid expression")
    | _ -> failwith "invalid expression"
    
let extractValues expr =
    match expr with
    | RTuple exprs -> exprs
                      |> List.map (fun e -> match e with
                                            | RFloat f -> box f
                                            | RInteger i -> box i
                                            | RText t -> box t
                                            | RBool b -> box b
                                            | _ -> failwith "invalid value type provided")
    | _ -> failwith "invalid expression"

let createTable id (columns:Column list) =
    { id = id; columns = new List<Column>(columns); rows = new List<Row>()}

let createTable2 id columnsDefs =
    let columns = columnsDefs 
                  |> List.map (fun cd -> createColumn cd)
    { id = id; columns = new List<Column>(columns); rows = new List<Row>()}

let addRow (values:obj array) table =
    if values.Length <> table.columns.Count then
        Failure (sprintf "cant add row to table %s: count of columns mismatches" table.id)
    else
        let invalid = values
                      |> Array.mapi (fun i value -> value >>? table.columns.[i].dataType)
                      |> Array.filter (fun value -> value.IsNone)
        if invalid.Length > 0 then 
            Failure (sprintf "cant add row to table %s: one or more values have invalid type" table.id)
        else
            table.rows.Add(Row (new List<obj>(values)))
            Success true

let tables = new List<Table>()
        
let rec interpret statement =
    match statement with
    | StCreate (id, expr) -> let table = createTable id (extractColumns expr)
                             tables.Add(table)
    | StInsert (id, expr) -> let values = extractValues expr                            
                             let table = tables.Find(fun t -> t.id = id)
                             match table with
                             | t when t.id <> null ->
                                                     let result = addRow (List.toArray values) table //failwith (sprintf "no table with id '%s'" id)
                                                     match result with
                                                     | Success _ -> ()
                                                     | Failure err -> failwith err
                             | _ -> ()
    | Script ss -> ss |> List.iter (interpret)
    | _ -> failwith "invalid statement"