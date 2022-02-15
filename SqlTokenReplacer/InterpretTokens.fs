module SqlTokenReplacer.InterpretTokens

open System.Text.RegularExpressions
open SqlTokenReplacer.Types
open SqlTokenReplacer.Utils

let getCmdType cmdStr : Option<Command> = failwith ""

let getCmdArgs cmdStr : string list = failwith ""


let getVariableArgs (cmdStr: string) (variables: FileInfo list) : FileInfo list =
    let variablesFromCmdStr =
        cmdStr.Trim('#').Split('[').[0].Split(',')

    variables
    |> List.where
        (fun v ->
            variablesFromCmdStr
            |> Array.exists (fun s -> s.Contains(v.FileName)))




// use regex here
let checkCommandSyntax cmdStr : bool = failwith ""


let interpretToken (variables: FileInfo list) (token: string) : Result<CmdInfo, string> =
    let cmdType = getCmdType token

    match cmdType with
    | None -> Error $"Command in {token} not recognised"
    | Some cmd ->
        Ok(
            { CmdStr = token
              Args = getCmdArgs token
              CmdType = cmd
              Variables = getVariableArgs token }
        )




let interpretTokens (tokens: string list) (variables: FileInfo list) : CmdInfo list * string list =
    tokens
    |> List.map (interpretToken variables)
    |> zipResults
