module SqlTokenReplacer.CommandTokens

open SqlTokenReplacer.Types
open SqlTokenReplacer.Utils


let getIndexesOfHashes (str: string) =
    Seq.toList str
    |> enumerate
    |> List.filter (fun (ch, _) -> ch = '#')
    |> List.map (fun (_, i) -> i)


let getHashIndexPairs indexes =
    let enumeratedIndexes = indexes |> enumerate

    enumeratedIndexes
    |> List.where (fun (_, index) -> index % 2 <> 0)
    |> List.zip (
        enumeratedIndexes
        |> List.where (fun (_, index) -> index % 2 = 0)
    )
    |> List.map (fun ((firstHash, _), (secondHash, _)) -> (firstHash, secondHash))


let getTokensUsing (indexPairs: (int * int) list) (str: string) =
    indexPairs |> List.map (fun (f, l) -> str.[f..l])


let getTokensFromStr (str: string) =
    let hashIndexes = getIndexesOfHashes str
    let hasOddNumberOfHashes = (fun (x: 'a list) -> x.Length % 2 <> 0)

    match hashIndexes with
    | i when i |> hasOddNumberOfHashes -> Error "Unclosed hash in token: could not parse"
    | i ->
        let indexPairs = getHashIndexPairs i
        let tokens = str |> getTokensUsing indexPairs
        Ok tokens


let getListOfSqlContent (sqlFiles: FileInfo list) =
    sqlFiles
    |> List.map (fun x -> x.Content |> Array.toList)
    |> flatten


let tryGetTokensFromContent (strs: string list) =
    strs
    |> List.map getTokensFromStr
    |> collectResults
    |> Result.map flatten


let removeBlankChars (input: string list) : string list =
    input |> List.map (fun y -> y.Replace(" ", ""))


let removeDuplicates (input: string list) : string list = Set(input) |> Set.toList

let getCommandTokensFrom (sqlFiles: FileInfo list) : Result<string list, string> =
    let filterStrings =
        removeDuplicates
        >> removeDuplicates
        >> removeNewLines

    sqlFiles
    |> getListOfSqlContent
    |> tryGetTokensFromContent
    |> Result.map filterStrings
