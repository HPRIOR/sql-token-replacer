module SqlTokenReplacer.InterpretTokens

open System.Text.RegularExpressions
open SqlTokenReplacer.Types
open SqlTokenReplacer.Utils

let getCmdType (cmdStr: string) : Option<Command> =
    let cmd = cmdStr.Split('[').[1].Split('(').[0]

    match cmd with
    | "All" -> Some All
    | "List" -> Some List
    | "Single" -> Some Single
    | "WhereList" -> Some WhereList
    | "WhereZip" -> Some WhereZip
    | _ -> None


let getCmdArgs (cmdStr: string) : string list =
    cmdStr.Trim('#').Split('(').[1]
        .TrimEnd(']')
        .TrimEnd(')')
        .Split(',')
    |> Array.where (fun x -> x.Length > 0)
    |> Array.toList


let getVariableArgs (cmdStr: string) (variables: FileInfo list) : FileInfo list =
    let variablesFromCmdStr =
        cmdStr.Trim('#').Split('[').[0].Split(',')

    variables
    |> List.where
        (fun v ->
            variablesFromCmdStr
            |> Array.exists (fun s -> s = v.FileName))


// use regex here
let checkCommandSyntax cmdStr : bool =
    Regex.IsMatch(cmdStr, "[A-Za-z0-9]+(,\s*[A-Za-z0-9]+)*\[[A-Za-z0-9]+\([A-Za-z0-9]*(,\s*[A-Za-z0-9]+)*\)\]")


// TODO check that correct number of args and variables for command
let interpretToken (variables: FileInfo list) (token: string) : Result<CmdInfo, string> =
    if (checkCommandSyntax token) then
        Error $"Syntax error in token: {token}"
    else
        let cmdType = getCmdType token

        match cmdType with
        | None -> Error $"Command in {token} not recognised"
        | Some cmd ->
            Ok(
                { CmdStr = token
                  Args = getCmdArgs token
                  CmdType = cmd
                  Variables = getVariableArgs token variables }
            )


let interpretTokens (tokens: string list) (variables: FileInfo list) : CmdInfo list * string list =
    tokens
    |> List.map (interpretToken variables)
    |> zipResults
