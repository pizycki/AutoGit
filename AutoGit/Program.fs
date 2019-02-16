open Argu
open System
open System.Diagnostics

module Common = 
    let timespanMiliseconds (ts:TimeSpan):int = int ts.TotalMilliseconds

open Common

module Process =

    type ProcessExec = string

    type ProcessExecArgsProvider = unit -> string
    type ProcessExecArgs =
        | Args of string
        | ArgsProvider of ProcessExecArgsProvider

    type ProcessWithArgs = ProcessExec * ProcessExecArgs

    let runnableProcess (data : ProcessWithArgs) =
        fun () -> 
            let proc = fst data
            let args = match snd data with
                       | Args s -> s
                       | ArgsProvider f -> f()

            use p = new Process()
            p.StartInfo <- ProcessStartInfo(proc, args)
            p.Start() |> ignore
            p.WaitForExit();
            ()
        
open Process

module Git =

    type GitRepository = string

    let gitProc args : ProcessWithArgs = ("git", args)

    let commit (message:(_ -> string)) : ProcessWithArgs = 
        let provider () = String.Format("commit -m \"{0}\"", message())
        ("git", ArgsProvider provider)

    let status : ProcessWithArgs =
        ("git", Args "status")

    let stageAll : ProcessWithArgs = 
        ("git", Args "add -A")

    let push : ProcessWithArgs =
        ("git", Args "push")

    let inDirectory (proc : ProcessWithArgs) (dir : GitRepository) : ProcessWithArgs =
        let args: ProcessExecArgs = 
            match (snd proc) with
            | Args a -> Args ("-C " + dir + " " + a)
            | ArgsProvider f -> ArgsProvider (fun () -> ("-C " + dir + " " + f()))

        (fst proc, args)

open Git

module AutoGit =
    type Arguments =
        | Interval of int
        | Directory of string
        | Push of bool
    with
        interface IArgParserTemplate with
            member arg.Usage =
                match arg with
                | Interval _ -> "Commit interval in minutes"
                | Directory _ -> "Path to Git repository directory"
                | Push _ -> "Pushes changes to remote with each commit"
    
    type CommitInterval = TimeSpan

    let setRepository (cmds : ProcessWithArgs list) (dir : GitRepository) : ProcessWithArgs list = 
        List.map (fun x -> inDirectory x dir) cmds
         
    let runnableCommands (cmds : ProcessWithArgs list) =
        List.map runnableProcess cmds

    let loop interval work =
        while true do
            let async = async {
                do! Async.Sleep interval
                work()
            }
            Async.RunSynchronously async

open AutoGit

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
        Git.stageAll;
        Git.commit commitMessage
    ]

    if push then cmds <- List.append cmds [Git.push]

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

    // Print Git Status once
    //Git.status |> runnableProcess |> (fun p -> p())

    let work () =
        let time = DateTime.Now.ToShortTimeString()
        printfn "Time: %s" time
        runnableCommands cmds |> List.iter (fun c -> c())
    
    // Start loop
    AutoGit.loop interval work

    0 // return an integer exit code