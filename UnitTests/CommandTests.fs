module CommandTests

open NUnit.Framework
open SqlTokenReplacer.Types
open SqlTokenReplacer.GenerateCommands

[<Test>]
let AllCommand_WillGenerateFuncThatReplacesAllContent () =
    let variables =
        [ { Content = [| "Insert"; "Me" |]
            FileName = "variable" } ]

    let inputSql =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content =
                [| "Replace"
                   "me"
                   "#token[token()]#" |]
            FileName = "sql" } ]

    let cmdInfo: CmdInfo =
        { CmdStr = "#token[token()]#"
          Variables = variables
          CmdType = All
          Args = []
          Type = "" }

    let command = generateCommand cmdInfo allParser

    let result = command inputSql

    let expected =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content = [| "Replace"; "me"; "Insert\nMe" |]
            FileName = "sql" } ]

    Assert.That(result, Is.EqualTo expected)


[<Test>]
let ListCommand_WillGenerateFuncThatReplacesAllContent () =
    let variables =
        [ { Content = [| "Insert"; "Me" |]
            FileName = "variable" } ]

    let inputSql =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content =
                [| "Replace"
                   "me"
                   "#token[token()]#" |]
            FileName = "sql" } ]

    let cmdInfo: CmdInfo =
        { CmdStr = "#token[token()]#"
          Variables = variables
          CmdType = All
          Args = []
          Type = ""}

    let command = generateCommand cmdInfo listParser

    let result = command inputSql

    let expected =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content = [| "Replace"; "me"; "Insert,Me" |]
            FileName = "sql" } ]

    Assert.That(result, Is.EqualTo expected)

[<Test>]
let ListCommand_WillGenerateFuncThatContainsType () =
    let variables =
        [ { Content = [| "Insert"; "Me" |]
            FileName = "variable" } ]

    let inputSql =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content =
                [| "Replace"
                   "me"
                   "#token[token()]#" |]
            FileName = "sql" } ]

    let cmdInfo: CmdInfo =
        { CmdStr = "#token[token()]#"
          Variables = variables
          CmdType = All
          Args = []
          Type = "string"}

    let command = generateCommand cmdInfo listParser

    let result = command inputSql

    let expected =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content = [| "Replace"; "me"; "'Insert','Me'" |]
            FileName = "sql" } ]

    Assert.That(result, Is.EqualTo expected)

[<Test>]
let WhereListCommand_WillGenerateFuncThatReplacesAllContent () =
    let variables =
        [ { Content = [| "Insert"; "Me" |]
            FileName = "variable" } ]

    let inputSql =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content =
                [| "Replace"
                   "me"
                   "#token[token()]#" |]
            FileName = "sql" } ]

    let cmdInfo: CmdInfo =
        { CmdStr = "#token[token()]#"
          Variables = variables
          CmdType = All
          Args = []
          Type = ""}

    let command = generateCommand cmdInfo whereListParser

    let result = command inputSql

    let expected =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content =
                [| "Replace"
                   "me"
                   "variable in (Insert,Me)" |]
            FileName = "sql" } ]

    Assert.That(result, Is.EqualTo expected)

[<Test>]
let WhereListCommand_WillGenerateFuncThatReplacesAllContent_WithType() =
    let variables =
        [ { Content = [| "Insert"; "Me" |]
            FileName = "variable" } ]

    let inputSql =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content =
                [| "Replace"
                   "me"
                   "#token[token()]#" |]
            FileName = "sql" } ]

    let cmdInfo: CmdInfo =
        { CmdStr = "#token[token()]#"
          Variables = variables
          CmdType = All
          Args = []
          Type = "string"}

    let command = generateCommand cmdInfo whereListParser

    let result = command inputSql

    let expected =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content =
                [| "Replace"
                   "me"
                   "variable in ('Insert','Me')" |]
            FileName = "sql" } ]

    Assert.That(result, Is.EqualTo expected)

[<Test>]
let WhereListCommand_WillGenerateFuncThatReplacesAllContent_WithArg() =
    let variables =
        [ { Content = [| "Insert"; "Me" |]
            FileName = "variable" } ]

    let inputSql =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content =
                [| "Replace"
                   "me"
                   "#token[token()]#" |]
            FileName = "sql" } ]

    let cmdInfo: CmdInfo =
        { CmdStr = "#token[token()]#"
          Variables = variables
          CmdType = All
          Args = ["a"]
          Type = "string"}

    let command = generateCommand cmdInfo whereListParser

    let result = command inputSql

    let expected =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content =
                [| "Replace"
                   "me"
                   "a.variable in ('Insert','Me')" |]
            FileName = "sql" } ]

    Assert.That(result, Is.EqualTo expected)

[<Test>]
let FlexZip_WillGenerateFuncThatReplacesUnevenContents () =
    let variables =
        [ { Content = [| "1"; "2" |]
            FileName = "var1" }
          { Content = [| "3" |]
            FileName = "var2" } ]

    let inputSql =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content =
                [| "Replace"
                   "me"
                   "where #token[token()]#" |]
            FileName = "sql" } ]

    let cmdInfo: CmdInfo =
        { CmdStr = "#token[token()]#"
          Variables = variables
          CmdType = All
          Args = []
          Type = ""}

    let command = generateCommand cmdInfo flexZipParser

    let result = command inputSql

    let expected =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content =
                [| "Replace"
                   "me"
                   "where var1 in (1,2)\nAND var2 in (3)" |]
            FileName = "sql" } ]

    Assert.That(result, Is.EqualTo expected)
    
[<Test>]
let FlexZip_WillGenerateFuncThatReplacesUnevenContents_WithType () =
    let variables =
        [ { Content = [| "1"; "2" |]
            FileName = "var1" }
          { Content = [| "3" |]
            FileName = "var2" } ]

    let inputSql =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content =
                [| "Replace"
                   "me"
                   "where #token[token()]#" |]
            FileName = "sql" } ]

    let cmdInfo: CmdInfo =
        { CmdStr = "#token[token()]#"
          Variables = variables
          CmdType = All
          Args = []
          Type = "string"}

    let command = generateCommand cmdInfo flexZipParser

    let result = command inputSql

    let expected =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content =
                [| "Replace"
                   "me"
                   "where var1 in ('1','2')\nAND var2 in ('3')" |]
            FileName = "sql" } ]

    Assert.That(result, Is.EqualTo expected)
    
    
[<Test>]
let FlexZip_WillGenerateFuncThatReplacesUnevenContents_WithArgs () =
    let variables =
        [ { Content = [| "1"; "2" |]
            FileName = "var1" }
          { Content = [| "3" |]
            FileName = "var2" } ]

    let inputSql =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content =
                [| "Replace"
                   "me"
                   "where #token[token()]#" |]
            FileName = "sql" } ]

    let cmdInfo: CmdInfo =
        { CmdStr = "#token[token()]#"
          Variables = variables
          CmdType = All
          Args = ["a"; "b"; "c"]
          Type = "string"}

    let command = generateCommand cmdInfo flexZipParser

    let result = command inputSql

    let expected =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content =
                [| "Replace"
                   "me"
                   "where a.var1 in ('1','2')\nAND b.var2 in ('3')" |]
            FileName = "sql" } ]

    Assert.That(result, Is.EqualTo expected)
    
[<Test>]
let FlexZip_WillGenerateFuncThatReplacesEvenContents_WithType () =
    let variables =
        [ { Content = [| "1"; "2" |]
            FileName = "var1" }
          { Content = [| "3" ; "4"|]
            FileName = "var2" } ]

    let inputSql =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content =
                [| "Replace"
                   "me"
                   "where #token[token()]#" |]
            FileName = "sql" } ]

    let cmdInfo: CmdInfo =
        { CmdStr = "#token[token()]#"
          Variables = variables
          CmdType = All
          Args = []
          Type = "string"}

    let command = generateCommand cmdInfo flexZipParser

    let result = command inputSql

    let expected =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content =
                [| "Replace"
                   "me"
                   "where ((var1 = '1' AND var2 = '3') OR\n(var1 = '2' AND var2 = '4'))" |]
            FileName = "sql" } ]

    Assert.That(result, Is.EqualTo expected)
    
[<Test>]
let FlexZip_WillGenerateFuncThatReplacesEvenContents_WithArgs() =
    let variables =
        [ { Content = [| "1"; "2" |]
            FileName = "var1" }
          { Content = [| "3" ; "4"|]
            FileName = "var2" } ]

    let inputSql =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content =
                [| "Replace"
                   "me"
                   "where #token[token()]#" |]
            FileName = "sql" } ]

    let cmdInfo: CmdInfo =
        { CmdStr = "#token[token()]#"
          Variables = variables
          CmdType = All
          Args = ["a"; "b"]
          Type = "string"}

    let command = generateCommand cmdInfo flexZipParser

    let result = command inputSql

    let expected =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content =
                [| "Replace"
                   "me"
                   "where ((a.var1 = '1' AND b.var2 = '3') OR\n(a.var1 = '2' AND b.var2 = '4'))" |]
            FileName = "sql" } ]

    Assert.That(result, Is.EqualTo expected)

[<Test>]
let FlexZip_WillGenerateFuncThatReplacesEvenContents () =
    let variables =
        [ { Content = [| "1"; "2" |]
            FileName = "var1" }
          { Content = [| "3" ; "4"|]
            FileName = "var2" } ]

    let inputSql =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content =
                [| "Replace"
                   "me"
                   "where #token[token()]#" |]
            FileName = "sql" } ]

    let cmdInfo: CmdInfo =
        { CmdStr = "#token[token()]#"
          Variables = variables
          CmdType = All
          Args = []
          Type = ""}

    let command = generateCommand cmdInfo flexZipParser

    let result = command inputSql

    let expected =
        [ { Content = [| "Don't"; "replace"; "me" |]
            FileName = "variable" }
          { Content =
                [| "Replace"
                   "me"
                   "where ((var1 = 1 AND var2 = 3) OR\n(var1 = 2 AND var2 = 4))" |]
            FileName = "sql" } ]

    Assert.That(result, Is.EqualTo expected)
