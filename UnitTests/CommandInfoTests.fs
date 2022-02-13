module CommandInfoTests

open NUnit.Framework
open SqlTokenReplacer.CommandTokens

[<Test>]
let GetTokensFromStr_WillParseSingleToken () =
    let input =
        "This is a test string with a simple #token#"

    let expected = [ "#token#" ]

    let result =
        match (getTokensFromStr input) with
        | Ok r -> r
        | Error _ -> [ "error" ]

    Assert.That(result, Is.EquivalentTo expected)

[<Test>]
let GetTokensFromStr_WillParseMultipleTokens () =
    let input =
        "This is a #test# string with a simple #token#"

    let expected = [ "#test#"; "#token#" ]

    let result =
        match (getTokensFromStr input) with
        | Ok r -> r
        | Error _ -> [ "error" ]

    Assert.That(result, Is.EquivalentTo expected)

[<Test>]
let GetTokensFromStr_WillParseTwoTokens_SideBySide () =
    let input =
        "This is a #test##token#"

    let expected = [ "#test#"; "#token#" ]

    let result =
        match (getTokensFromStr input) with
        | Ok r -> r
        | Error _ -> [ "error" ]

    Assert.That(result, Is.EquivalentTo expected)

[<Test>]
let GetTokensFromStr_ReturnErrorIfUnclosedToken () =
    let input =
        "This is a test string with a simple #token"

    let expected = [ "error" ]

    let result =
        match (getTokensFromStr input) with
        | Ok r -> r
        | Error _ -> [ "error" ]

    Assert.That(result, Is.EquivalentTo expected)
    
[<Test>]
let GetTokensFromStr_ReturnErrorIfSingleUnclosedOfTwo () =
    let input =
        "This is a #test string with a simple #token#"

    let expected = [ "error" ]

    let result =
        match (getTokensFromStr input) with
        | Ok r -> r
        | Error _ -> [ "error" ]

    Assert.That(result, Is.EquivalentTo expected)
    
