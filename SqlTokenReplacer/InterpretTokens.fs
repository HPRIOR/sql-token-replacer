module SqlTokenReplacer.InterpretTokens

open System.Text.RegularExpressions
open SqlTokenReplacer.Types
open SqlTokenReplacer.Utils


// TODO account for mismatch in token variables and found file variables
// e.g. var1.txt, var2.txt in Variable dir, but token references var3
// token should be invalid

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


let getVariableArgs (cmdStr: string) (variables: FileInfo list) : FileInfo list =
    let variablesFromCmdStr =
        cmdStr.Trim('#').Split('[').[0].Split(',')

    variables
    |> List.where
        (fun v ->
            variablesFromCmdStr
            |> Array.exists (fun s -> s = v.FileName))

let getType (token: string) : string = token |> getBetween '<' '>'


let commandSyntaxIsOk cmdStr : bool =
    Regex.IsMatch(
        cmdStr,
        "^#[A-Za-z0-9]+(,\s*[A-Za-z0-9]+)*\[[A-Za-z0-9]+\([A-Za-z0-9]*(,\s*[A-Za-z0-9]+)*\)\]<[A-Za-z0-9]*>#$"
    )
    


let interpretToken (variables: FileInfo list) (token: string) : Result<CmdInfo, string> =
    if (checkCommandSyntax token) then
        Error $"Syntax error in token: {token}.\nMust be in the form of: #variable[command(args)]#"
    else
        let cmdType = getCmdType token

        match cmdType with
        | None -> Error $"Command in {token} not recognised"
        | Some cmd ->
            Ok(
                { CmdStr = token
                  Args = getCmdArgs token
                  CmdType = cmd
                  Variables = getVariableArgs token variables
                  Type = getType token }
            )


let interpretTokens (tokens: string list) (variables: FileInfo list) : CmdInfo list * string list =
    tokens
    |> List.map (interpretToken variables)
    |> zipResults
