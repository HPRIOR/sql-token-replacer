module InterpretTokensTest

open NUnit.Framework
open SqlTokenReplacer.InterpretTokens
open SqlTokenReplacer.Types

[<Test>]
let CheckCommandSyntax_WillAcceptASingleVariable () =
    let input = "#var[command()]<int>#"
    let result = commandSyntaxIsOk input
    Assert.That(result, Is.True)

[<Test>]
let CheckCommandSyntax_WillAcceptMultiVariables () =
    let input = "#var1,var2[command()]<string>#"
    let result = commandSyntaxIsOk input
    Assert.That(result, Is.True)


[<Test>]
let CheckCommandSyntax_WillRejectNoVariables () =
    let input = "#[command()]<string>#"
    let result = commandSyntaxIsOk input
    Assert.That(result, Is.False)

[<Test>]
let CheckCommandSyntax_WillAcceptSingleArg () =
    let input = "#var1[command(arg1)]<int>#"
    let result = commandSyntaxIsOk input
    Assert.That(result, Is.True)

[<Test>]
let CheckCommandSyntax_WillAcceptMultpleArgs () =
    let input = "#var1[command(arg1,arg2)]<string>#"
    let result = commandSyntaxIsOk input
    Assert.That(result, Is.True)

[<Test>]
let CheckCommandSyntax_WillRejectMultipleCommands () =
    let input =
        "#var1[command,command2(arg1,arg2)]<int>#"

    let result = commandSyntaxIsOk input
    Assert.That(result, Is.False)

[<Test>]
let CheckCommandSyntax_AcceptUnderScoresInVariables () =
    let input =
        "#var1[command_1,command_2(arg1,arg2)]<int>#"

    let result = commandSyntaxIsOk input
    Assert.That(result, Is.False)

[<Test>]
let GetVariableArgs_WillGetMatchingVariables () =
    let cmdStr = "#var[command()]<string>#"

    let variables =
        [ { Content = [| "some content" |]
            FileName = "var" }
          { Content = [| "some content" |]
            FileName = "var2" } ]

    let result =
        match getVariableArgs cmdStr variables with
        | Ok result -> result
        | Error _ -> failwith "error"

    let expected =
        [ { Content = [| "some content" |]
            FileName = "var" } ]

    Assert.That(result, Is.EquivalentTo(expected))

[<Test>]
let GetVariableArgs_WillReturnErrorIfNoMatches () =
    let cmdStr = "#var3[command()]<string>#"

    let variables =
        [ { Content = [| "some content" |]
            FileName = "var" }
          { Content = [| "some content" |]
            FileName = "var2" } ]

    let result =
        match getVariableArgs cmdStr variables with
        | Ok _ -> "Ok!"
        | Error _ -> "error"

    Assert.That(result, Is.EqualTo("error"))

[<Test>]
let GetVariableArgs_WillReturnError_OnEmptyVariableInput () =
    let cmdStr = "#var3[command()]<string>#"
    let variables = []

    let result =
        match getVariableArgs cmdStr variables with
        | Ok _ -> failwith "ok"
        | Error _ -> "error"


    Assert.That(result, Is.EqualTo("error"))

[<Test>]
let GetCmdArgs_WillReturnArgList () =
    let cmdStr = "#var3[command(a,b,c,d)]<string>#"
    let result = getCmdArgs cmdStr
    let expected = [ "a"; "b"; "c"; "d" ]

    Assert.That(result, Is.EquivalentTo(expected))

[<Test>]
let GetCmdArgs_WillReturnEmptyListNoArgs () =
    let cmdStr = "#var3[command()]<int>#"
    let result = getCmdArgs cmdStr
    Assert.That(result.Length, Is.EqualTo(0))

[<Test>]
let GetCmdType_WillGetAll () =
    let cmdStr = "#var[All()]<string>#"
    let result = getCmdType cmdStr
    Assert.That(result, Is.EqualTo(Some All))

[<Test>]
let GetCmdType_WillGetList () =
    let cmdStr = "#var[List()]<string>#"
    let result = getCmdType cmdStr
    Assert.That(result, Is.EqualTo(Some List))

[<Test>]
let GetCmdType_WillGetSingle () =
    let cmdStr = "#var[Single()]<string>#"
    let result = getCmdType cmdStr
    Assert.That(result, Is.EqualTo(Some Single))

[<Test>]
let GetCmdType_WillGetWhereList () =
    let cmdStr = "#var[WhereList()]<string>#"
    let result = getCmdType cmdStr
    Assert.That(result, Is.EqualTo(Some WhereList))

[<Test>]
let GetCmdType_WillGetWhereZip () =
    let cmdStr = "#var[FlexZip()]<string>#"
    let result = getCmdType cmdStr
    Assert.That(result, Is.EqualTo(Some FlexZip))

[<Test>]
let GetCmdType_WillReturnNone () =
    let cmdStr = "#var[SDASfasdas()]<string>#"
    let result = getCmdType cmdStr
    Assert.That(result, Is.EqualTo(None))


[<Test>]
let GetType_WillGetTypeInBrackets () =
    let cmdStr = "#var[command()]<string>#"
    let result = getType cmdStr
    Assert.That(result, Is.EqualTo("string"))
