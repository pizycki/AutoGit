module CommonModuleTests

open System
open Xunit
open FsUnit.Xunit

[<Fact>]
let ``Returns TimeSpan as miliseconds`` () =
    TimeSpan.FromMinutes((float) 1) |> AutoGit.Common.timespanMiliseconds |> should equal 60000
