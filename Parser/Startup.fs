module Startup

//open System
//open ParserDef
//open RParser
//open System.IO
//open System.Diagnostics

//[<EntryPoint>]
//let main _ =
//    printfn "%s" "Specify a file to parse:"
//    let path = Console.ReadLine()
//    let lines = File.ReadAllText path

//    let stopwatch = new Stopwatch()
//    stopwatch.Start()
//    let result = run rScript lines    
//    stopwatch.Stop()

//    //File.WriteAllText("result.txt", sprint result)
//    printfn "Done in %Ams" stopwatch.ElapsedMilliseconds

//    match result with
//    | Success (statement, _) -> RInterpreter.interpret statement
//    | _ -> ()
//    printfn "%A" RInterpreter.tables

//    Console.ReadKey() |> ignore
//    0