module SqlTokenReplacer.Utils



let collectResults<'a, 'b> (results: Result<'a, 'b> list) : Result<'a list, 'b> =
    let rec loop (results: Result<'a, 'b> list) (acc: Result<'a list, 'b>) : Result<'a list, 'b> =
        match results with
        | [] -> acc |> Result.map List.rev
        | x :: xs ->
            match x with
            | Ok hd ->
                let newAcc = acc |> Result.map (fun tl -> hd :: tl)
                loop xs newAcc
            | Error e -> Error e

    loop results (Ok [])


let rec zipResults (results: Result<'a, 'b> list) : 'a list * 'b list =
    let rec loop (results: Result<'a, 'b> list) (acc: 'a list * 'b list) : 'a list * 'b list =
        match results with
        | [] ->
            let a, b = acc
            (List.rev a, List.rev b)
        | x :: xs ->
            match x with
            | Ok hd ->
                let a, b = acc
                let newAcc = (hd :: a), b
                loop xs newAcc
            | Error hd ->
                let a, b = acc
                let newAcc = a, (hd :: b)
                loop xs newAcc

    loop results ([], [])

let enumerate (list: 'a list) =
    [ for i in 0 .. list.Length - 1 -> i ]
    |> List.zip list

let flatten (listOfLists: 'a list list) : 'a list =
    listOfLists |> List.collect (fun x -> x)
