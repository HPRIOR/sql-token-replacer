open System.IO
open SqlTokenReplacer.CommandTokens
open SqlTokenReplacer.Input
open SqlTokenReplacer.InterpretTokens
open SqlTokenReplacer.GenerateCommands
open SqlTokenReplacer.Output
open SqlTokenReplacer.Types
open SqlTokenReplacer.EnvironmentVars

let extractResultOrFail result =
    match result with
    | Ok result -> result
    | Error e ->
        eprintf $"{e}"
        exit 1


let removeFileExtensions (fileInfo: FileInfo) =
    let newFileName =
        Path.GetFileNameWithoutExtension fileInfo.FileName

    { fileInfo with FileName = newFileName }



[<EntryPoint>]
let main _ =
    let envVar =
        match getEnvironmentVars with
        | Ok vars -> vars
        | Error e ->
            eprintf $"{e}"
            exit 1
    
    
    let sqlFiles =
        extractResultOrFail (getFilesFrom envVar.["MODIFY"])

    let variableFiles =
        extractResultOrFail (getFilesFrom envVar.["VARIABLES"])
        |> List.map removeFileExtensions

    let outputDirectory = envVar.["OUTPUTTO"]
    let saveName = envVar.["SAVEAS"]

    let initTokens =
        extractResultOrFail (getCommandTokensFrom sqlFiles)

    let cmdInfoList, tokenErrors =
        interpretTokens initTokens variableFiles

    tokenErrors
    |> List.iter (fun x -> eprintf $"{x}\n")

    if (cmdInfoList.Length = 0) then
        eprintf "No valid tokens found"
        exit 1

    let replaceTokens: CommandFunc =
        generateCommands cmdInfoList
        |> List.reduce (fun x y -> x >> y)

    let resultFiles = replaceTokens sqlFiles
    
    let outputDirectory = Path.Join([outputDirectory; saveName] |> List.toArray)
    let saveResult = saveFiles resultFiles outputDirectory  
    
    match saveResult with
    | Ok _ ->
        printf $"Results saved to: {outputDirectory}"
        exit 0
    | Error e ->
        eprintf $"Error saving files: {e}"
        exit 1
