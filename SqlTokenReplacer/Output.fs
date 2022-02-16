module SqlTokenReplacer.Output

open System.IO
open SqlTokenReplacer.Types
open SqlTokenReplacer.Utils


let tryCreateDirectoryAt (path: string) : Result<unit, string> =
    try
        Directory.CreateDirectory(path) |> ignore
        Ok()
    with
    | :? System.Exception -> Error $"Problem creating directory at path: {path}"


let saveFile dirPath (file: FileInfo) =
    let path =
        Path.Join([ dirPath; file.FileName ] |> List.toArray)

    try
        File.WriteAllLines(path, file.Content)
        Ok()
    with
    | :? System.Exception -> Error $"Problem saving file at path: {path}"


let saveFilesTo directoryPath (files: FileInfo list) =
    files |> List.map (saveFile directoryPath)

let saveFiles (files: FileInfo list) path : Result<unit, string> =
    match tryCreateDirectoryAt path with
    | Ok _ ->
        let collectedResults = (saveFilesTo path files) |> collectResults
        match collectedResults with
        | Ok _ -> Ok ()
        | Error e -> Error e
    | Error e -> Error e




