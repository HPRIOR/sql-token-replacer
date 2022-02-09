module SqlTokenReplacer.Input

open System.IO
open SqlTokenReplacer.Types



let private getSqlFile (filePath: string) : Result<SqlFile, string> =
    let filePath = Path.GetFullPath filePath
    let fileName = Path.GetFileName filePath

    try
        Ok
        <| { Content = (File.ReadAllLines filePath)
             FileName = fileName }
    with
    | :? System.Exception as ex -> Error $"Problem reading SQL file: {ex}"
    
let rec private reduceResults (results: Result<'a, 'b>[]): Result<'a[], 'b> =
    match results with
        result when result.Length = 1 -> 
           
let  getSqlFiles (directoryPath: string) : Result<SqlFile [], string> =
    let ioResult =
        try
                Directory.GetFiles
                <| Path.GetFullPath directoryPath
                |> Array. getSqlFile
        with
        | :? System.Exception as ex -> Error $"Problem reading Sql directory: {ex}"

    let showme =
        match ioResult with
        | Ok result -> result |> Array.map getSqlFile
        | Error err -> [|Error err|] 

    
    Ok [| { Content = [| "content" |]
            FileName = "" } |]

