open SqlTokenReplacer.Input

[<EntryPoint>]
let main argv =
    let sqlFileResults = getFilesFrom "path"
    
    match sqlFileResults with
    | Ok _ -> printf "Ok!"
    | Error _ ->
        eprintf $"Error reading path sql files"
        exit 1
    |> ignore
    
    0 // return an integer exit code