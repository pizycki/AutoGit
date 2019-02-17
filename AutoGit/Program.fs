open Argu
open System
open AutoGit.Common
open AutoGit.Git
open AutoGit.Process
open AutoGit.AutoGit

[<EntryPoint>]
let main argv =
    let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)
    let parser = ArgumentParser.Create<Arguments>(programName = "ls", errorHandler = errorHandler)     
    let results = parser.ParseCommandLine argv

    let interval = 
        match results.TryGetResult<@Arguments.Interval@> with 
        | Some argInterval -> 
            printfn "AutoGit will commit changes every %d minute(s)." argInterval
            argInterval |> (float) |> TimeSpan.FromMinutes |> timespanMiliseconds
        | None -> 
            let defaultIntervalInMinutes = 1
            printfn "No interval set. AutoGit will commit changes every %d minute(s)." defaultIntervalInMinutes
            defaultIntervalInMinutes |> (float) |> TimeSpan.FromMinutes |> timespanMiliseconds            
            
            
    let repository = results.TryGetResult<@Directory@>
    let push = match results.TryGetResult<@Arguments.Push@> with Some v -> v | None -> false
    
    let commitMessage = fun () -> "AutoGit - " + System.DateTime.Now.ToShortTimeString()

    let mutable cmds = [
        stageAll;
        commit commitMessage
    ]

    if push then cmds <- List.append cmds [AutoGit.Git.push]

    match repository with
    | Some r -> cmds <- setRepository cmds r
    | None -> ()    

    printfn "Commands to run"
    for cmd in cmds do
        let proc = fst cmd
        let args = match snd cmd with
                   | Args a -> a
                   | ArgsProvider f -> f()
        printfn "%s %s" proc args

    let work () =
        let time = DateTime.Now.ToShortTimeString()
        printfn "Time: %s" time
        runnableCommands cmds |> List.iter (fun c -> c())
    
    // Start loop
    loop interval work

    0 // return an integer exit code