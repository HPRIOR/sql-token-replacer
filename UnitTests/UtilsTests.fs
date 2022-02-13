module UtilsTest

open NUnit.Framework
open SqlTokenReplacer.Utils

[<Test>]
let ReduceResult_WillReduceAllOk () =
    let input = [ Ok 1; Ok 2; Ok 3 ]
    let expected = Ok [ 1; 2; 3 ]
    let result = collectResults input
    Assert.AreEqual(expected, result)

[<Test>]
let ReduceResult_WillReduceToFirstError () =
    let input = [ Ok 1; Error "error1"; Error "error2" ]

    let result =
        match (collectResults input) with
        | Ok _ -> "not error"
        | Error _ -> "error"

    Assert.AreEqual("error", result)

[<Test>]
let ZipResult_WillZip () =
    let input =
        [ Ok 1
          Ok 2
          Ok 3
          Error "1"
          Error "2"
          Error "3" ]

    let result = zipResults input
    let expected = ([ 1; 2; 3 ], [ "1"; "2"; "3" ])

    Assert.That(result, Is.EqualTo expected)
