module InterpretTokensTest

open NUnit.Framework
open SqlTokenReplacer.InterpretTokens
open SqlTokenReplacer.Types

[<Test>]
let CheckCommandSyntax_WillAcceptASingleVariable () =
    let input = "#var[command()]#"
    let result = checkCommandSyntax input
    Assert.That(result, Is.True)

[<Test>]
let CheckCommandSyntax_WillAcceptMultiVariables () =
    let input = "#var1,var2[command()]#"
    let result = checkCommandSyntax input
    Assert.That(result, Is.True)


[<Test>]
let CheckCommandSyntax_WillRejectNoVariables () =
    let input = "#[command()]#"
    let result = checkCommandSyntax input
    Assert.That(result, Is.False)

[<Test>]
let CheckCommandSyntax_WillAcceptSingleArg () =
    let input = "#var1[command(arg1)]#"
    let result = checkCommandSyntax input
    Assert.That(result, Is.True)

[<Test>]
let CheckCommandSyntax_WillAcceptMultpleArgs () =
    let input = "#var1[command(arg1,arg2)]#"
    let result = checkCommandSyntax input
    Assert.That(result, Is.True)

[<Test>]
let CheckCommandSyntax_WillRejectMultipleCommands () =
    let input = "#var1[command,command2(arg1,arg2)]#"
    let result = checkCommandSyntax input
    Assert.That(result, Is.False)

[<Test>]
let GetVariableArgs_WillGetMatchingVariables () =
    let cmdStr = "#var[command()]#"

    let variables =
        [ { Content = [| "some content" |]
            FileName = "var" }
          { Content = [| "some content" |]
            FileName = "var2" } ]

    let result = getVariableArgs cmdStr variables

    let expected =
        [ { Content = [| "some content" |]
            FileName = "var" } ]

    Assert.That(result, Is.EquivalentTo(expected))

[<Test>]
let GetVariableArgs_WillReturnEmptyListIfNoMatches () =
    let cmdStr = "#var3[command()]#"

    let variables =
        [ { Content = [| "some content" |]
            FileName = "var" }
          { Content = [| "some content" |]
            FileName = "var2" } ]

    let result = getVariableArgs cmdStr variables

    let expected = []

    Assert.That(result, Is.EquivalentTo(expected))

[<Test>]
let GetVariableArgs_WillReturnEmptyArray_WithEmptyVariablesInput () =
    let cmdStr = "#var3[command()]#"
    let variables = []
    let result = getVariableArgs cmdStr variables
    let expected = []

    Assert.That(result, Is.EquivalentTo(expected))

[<Test>]
let GetCmdArgs_WillReturnArgList () =
    let cmdStr = "#var3[command(a,b,c,d)]#"
    let result = getCmdArgs cmdStr
    let expected = [ "a"; "b"; "c"; "d" ]

    Assert.That(result, Is.EquivalentTo(expected))

[<Test>]
let GetCmdArgs_WillReturnEmptyListNoArgs () =
    let cmdStr = "#var3[command()]#"
    let result = getCmdArgs cmdStr
    Assert.That(result.Length, Is.EqualTo(0))

[<Test>]
let GetCmdType_WillGetAll () =
    let cmdStr = "#var[All()]#"
    let result = getCmdType cmdStr
    Assert.That(result, Is.EqualTo(Some All))

[<Test>]
let GetCmdType_WillGetList () =
    let cmdStr = "#var[List()]#"
    let result = getCmdType cmdStr
    Assert.That(result, Is.EqualTo(Some List))

[<Test>]
let GetCmdType_WillGetSingle () =
    let cmdStr = "#var[Single()]#"
    let result = getCmdType cmdStr
    Assert.That(result, Is.EqualTo(Some Single))

[<Test>]
let GetCmdType_WillGetWhereList () =
    let cmdStr = "#var[WhereList()]#"
    let result = getCmdType cmdStr
    Assert.That(result, Is.EqualTo(Some WhereList))

[<Test>]
let GetCmdType_WillGetWhereZip () =
    let cmdStr = "#var[WhereZip()]#"
    let result = getCmdType cmdStr
    Assert.That(result, Is.EqualTo(Some WhereZip))

[<Test>]
let GetCmdType_WillReturnNone () =
    let cmdStr = "#var[SDASfasdas()]#"
    let result = getCmdType cmdStr
    Assert.That(result, Is.EqualTo(None))

// todo check interpetTokens 