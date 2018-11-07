open System
open Argu
open System.Diagnostics

module AutoGit =
    type Arguments =
        | Interval of interval:int
        | Working_Directory of dir:string
        | Push
    
    let Loop interval work =
        while true do
            let async = async {
                printfn "Wait for %i" interval
                do! Async.Sleep interval
                printfn "Do some work"
                work()
            }
            Async.RunSynchronously async
            

    let PrintStatus () =
        use p = new Process()
        p.StartInfo <- ProcessStartInfo("git", "status")
        p.Start() |> ignore
        ()
        

[<EntryPoint>]
let main argv =
    printfn "=== AutoGit started ==="
    let seconds = 5 * 1000
    AutoGit.Loop seconds AutoGit.PrintStatus
    0 // return an integer exit code
