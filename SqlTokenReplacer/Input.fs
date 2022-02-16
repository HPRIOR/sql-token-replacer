module SqlTokenReplacer.Input

open System.IO
open SqlTokenReplacer.Utils
open SqlTokenReplacer.Types



// TODO return error if no files returned
let private getFile (filePath: string) : Result<FileInfo, string> =
    let filePath = Path.GetFullPath filePath
    let fileName = Path.GetFileName filePath

    try
        Ok
        <| { Content = (File.ReadAllLines filePath)
             FileName = fileName }
    with
    | :? System.Exception -> Error $"Problem reading file at path: {filePath}"



let getFilesFrom (directoryPath: string) : Result<FileInfo list, string> =
    let ioResults =
        try
            Ok(
                Directory.GetFiles
                <| Path.GetFullPath directoryPath
            )
        with
        | :? System.Exception -> Error $"Problem reading directory at: {directoryPath}"

    match ioResults with
    | Ok result ->
        result
        |> Array.map (fun x -> getFile x)
        |> Array.toList
    | Error err -> [ (Error err) ]
    |> collectResults
