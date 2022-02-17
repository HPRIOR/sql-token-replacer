module SqlTokenReplacer.EnvironmentVars

open System
open System.IO
open SqlTokenReplacer.Utils


let checkIfFilesAreIn path : Result<unit, string> =
    let fileCount =
        (Directory.EnumerateFiles(path) |> List.ofSeq)
            .Length

    match fileCount with
    | 0 -> Error $"No files found at: {path}"
    | _ -> Ok()

let checkEnvironmentVars (vars: Map<string, string>) : Result<Map<string, string>, string> =
    let checkResults =
        [ (checkIfFilesAreIn vars.["VARIABLES"])
          (checkIfFilesAreIn vars.["MODIFY"]) ]
        |> collectResults

    match checkResults with
    | Ok _ -> Ok vars
    | Error e -> Error e


let getVariable key : Result<string * string, string> =
    let var = Environment.GetEnvironmentVariable(key)

    match var with
    | null -> Error $"Variable: {key}, not found in the environment"
    | var -> Ok(key, var)

let getEnvironmentVars: Result<Map<string, string>, string> =
    [ "VARIABLES"
      "MODIFY"
      "OUTPUTTO"
      "SAVEAS" ]
    |> List.map getVariable
    |> collectResults
    |> Result.map Map.ofList
    |> Result.bind checkEnvironmentVars
