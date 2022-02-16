module SqlTokenReplacer.InterpretTokens

open System.Text.RegularExpressions
open SqlTokenReplacer.Types
open SqlTokenReplacer.Utils

let getBetween (charOne: char) (charTwo: char) (str: string) =
    str.Split(charOne).[1].Split(charTwo).[0]

let getCmdType (token: string) : Option<Command> =
    let cmd = token.Split('[').[1].Split('(').[0]

    match cmd with
    | "All" -> Some All
    | "List" -> Some List
    | "Single" -> Some Single
    | "WhereList" -> Some WhereList
    | "FlexZip" -> Some FlexZip
    | _ -> None


let getCmdArgs (token: string) : string list =
    (token |> getBetween '(' ')').Split(',')
    |> Array.where (fun x -> x.Length > 0)
    |> Array.toList


let getVariableArgs (token: string) (variables: FileInfo list) : Result<FileInfo list, string> =
    let varsFromCmdStr =
        token.Trim('#').Split('[').[0].Split(',')

    let matchedVars =
        variables
        |> List.where
            (fun variable ->
                varsFromCmdStr
                |> Array.exists (fun s -> s = variable.FileName))

    if (matchedVars.Length <> varsFromCmdStr.Length) then
        let cmdStrVarsStr =
            varsFromCmdStr
            |> Array.reduce (fun x y -> $"{x}, {y}")

        let matchedVarsStr =
            match matchedVars with
            | [] -> ""
            | _ ->
                matchedVars
                |> List.map (fun x -> x.FileName)
                |> List.reduce (fun x y -> $"{x}, {y}")

        Error $"Variable arguments not satisfied. Needed {cmdStrVarsStr}, but found: {matchedVarsStr}"
    else
        Ok matchedVars

let getType (token: string) : string = token |> getBetween '<' '>'

let commandSyntaxIsOk cmdStr : bool =
    Regex.IsMatch(
        cmdStr,
        "^#[A-Za-z0-9]+(,\s*[A-Za-z0-9]+)*\[[A-Za-z0-9]+\([A-Za-z0-9]*(,\s*[A-Za-z0-9]+)*\)\]<[A-Za-z0-9]*>#$"
    )

(*
To avoid lost of nested match cases, each 'get' function could return a new Result<Cmdinfo, string> or just CmdInfo.
These could be chained together with bind and map, but would be less efficient 
*)
let interpretToken (variables: FileInfo list) (token: string) : Result<CmdInfo, string> =
    if not (commandSyntaxIsOk token) then
        Error $"Syntax error in token: {token}.\nMust be in the form of: #variable[command(args)]#"
    else
        let cmdType = getCmdType token

        match cmdType with
        | None -> Error $"Command in {token} not recognised"
        | Some cmdType ->
            let varArgs = getVariableArgs token variables

            match varArgs with
            | Ok args ->
                Ok(
                    { CmdStr = token
                      Args = getCmdArgs token
                      CmdType = cmdType
                      Variables = args
                      Type = getType token }
                )
            | Error e -> Error e


let interpretTokens (tokens: string list) (variables: FileInfo list) : CmdInfo list * string list =
    tokens
    |> List.map (interpretToken variables)
    |> zipResults
