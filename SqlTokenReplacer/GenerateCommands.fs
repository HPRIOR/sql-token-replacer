module SqlTokenReplacer.GenerateCommands

open SqlTokenReplacer.Types

type VariableParser = FileInfo list * CmdInfo -> string

let getTypeStr type_ =
    match type_ with
    | "string" -> "'"
    | "int" -> ""
    | _ -> ""

let getArgStrs (cmdInfo: CmdInfo) =
    match cmdInfo.Args with
    | [] -> "", ""
    | [ one ] -> $"{one}.", ""
    | [ one; two ] -> $"{one}.", $"{two}."
    | one :: two :: _ -> $"{one}.", $"{two}."

let flattenToParagraph input =
    input |> Array.reduce (fun x y -> $"{x}\n{y}")

let flattenToCommaList type_ input =
    let t = getTypeStr type_

    input
    |> Array.map (fun x -> $"{t}{x}{t}")
    |> Array.reduce (fun x y -> $"{x},{y}")

// Single
let singleParser (variables: FileInfo list, cmdInfo: CmdInfo) =
    let t = getTypeStr cmdInfo.Type
    $"{t}{variables.Head.Content.[0]}{t}"

// All
let allParser (variables: FileInfo list, _: CmdInfo) =
    variables.Head.Content |> flattenToParagraph

// List
let listParser (variables: FileInfo list, cmdInfo: CmdInfo) =
    variables.Head.Content
    |> flattenToCommaList cmdInfo.Type

// WhereList
let whereListParser (variables: FileInfo list, cmdInfo: CmdInfo) =
    let list =
        variables.Head.Content
        |> flattenToCommaList cmdInfo.Type

    let arg =
        match cmdInfo.Args with
        | [] -> ""
        | head :: _ -> $"{head}."


    let fileName = cmdInfo.Variables.Head.FileName
    $"{arg}{fileName} in ({list})"

// FlexZip
let flattenContentIntoWhereIn content fileName type_ arg =
    let list = content |> flattenToCommaList type_
    $"{arg}{fileName} in ({list})"

let generateWithUnequalVariables (variables: FileInfo list, cmdInfo: CmdInfo) =
    let variableOne = variables.[0]
    let variableTwo = variables.[1]

    let argOne, argTwo = getArgStrs cmdInfo
    $"{flattenContentIntoWhereIn variableOne.Content variableOne.FileName cmdInfo.Type argOne}\nAND {flattenContentIntoWhereIn variableTwo.Content variableTwo.FileName cmdInfo.Type argTwo}"

let getVariableFileNamePairs (contents: string []) (fileName: string) =
    contents
    |> Array.zip (
        [ for _ in 0 .. contents.Length - 1 -> fileName ]
        |> List.toArray
    )


let generateWithEqualVariables (variables: FileInfo list, cmdInfo: CmdInfo) =
    let variableOne = variables.[0]
    let variableTwo = variables.[1]

    let variableOneWithFileNames =
        getVariableFileNamePairs variableOne.Content variableOne.FileName

    let variableTwoWithFileNames =
        getVariableFileNamePairs variableTwo.Content variableTwo.FileName

    let zippedTuples =
        variableTwoWithFileNames
        |> Array.zip variableOneWithFileNames

    let argOne, argTwo = getArgStrs cmdInfo

    let result =
        zippedTuples
        |> Array.map
            (fun ((fileNameA, contentA), (fileNameB, contentB)) ->
                let t = getTypeStr cmdInfo.Type
                $"({argOne}{fileNameA} = {t}{contentA}{t} AND {argTwo}{fileNameB} = {t}{contentB}{t})")
        |> Array.reduce (fun x y -> $"{x} OR\n{y}")

    $"({result})"

let flexZipParser (variables: FileInfo list, cmdInfo: CmdInfo) =
    let variableOneLength = variables.[0].Content.Length
    let variableTwoLength = variables.[1].Content.Length

    if (variableOneLength <> variableTwoLength) then
        generateWithUnequalVariables (variables, cmdInfo)
    else
        generateWithEqualVariables (variables, cmdInfo)


// Generator
let parseFileContent cmdInfo (variableParser: VariableParser) fileInfo =
    let newContent =
        fileInfo.Content
        |> Array.map (fun s -> s.Replace(cmdInfo.CmdStr, variableParser (cmdInfo.Variables, cmdInfo)))

    { fileInfo with Content = newContent }

let generateCommand cmdInfo (variableParser: VariableParser) : CommandFunc =
    fun (inputFiles: FileInfo list) ->
        inputFiles
        |> List.map (parseFileContent cmdInfo variableParser)


let getCommandFromCommandType (cmdInfo: CmdInfo) =
    let generateCommand = generateCommand cmdInfo

    match cmdInfo.CmdType with
    | All -> generateCommand allParser
    | List -> generateCommand listParser
    | Single -> generateCommand singleParser
    | WhereList -> generateCommand whereListParser
    | FlexZip -> generateCommand flexZipParser

let generateCommands (cmdInfoList: CmdInfo list) : CommandFunc list =
    cmdInfoList |> List.map getCommandFromCommandType
