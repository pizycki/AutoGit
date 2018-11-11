open System
open Argu
open System.Diagnostics

module Common = 
    let timespanMiliseconds (ts:TimeSpan):int = int ts.TotalMilliseconds

open Common
module Process =

    type ProcessExec = string
    type ProcessExecArgs = string
    type ProcessWithArgs = ProcessExec * ProcessExecArgs 

    let runnableProcess (data : ProcessWithArgs) =
        (fun () -> 
            use p = new Process()
            p.StartInfo <- ProcessStartInfo(fst data, snd data)
            p.Start() |> ignore
            p.WaitForExit();
            ())
        

open Process
module Git =

    type GitRepository = string

    let commit (message:string) : ProcessWithArgs = 
        ("git", String.Format("commit -m \"{0}\"", message))

    let status : ProcessWithArgs =
        ("git", "status")

    let stageAll : ProcessWithArgs = 
        ("git", "add -A")

    let push : ProcessWithArgs =
        ("git", "push")

    let inDirectory (proc : ProcessWithArgs) (dir : GitRepository) : ProcessWithArgs = 
        let args = "-C " + dir + " " + (snd proc)
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

    let interval = match results.TryGetResult<@Arguments.Interval@> with Some v -> v | None -> 1 |> (float) |> TimeSpan.FromMinutes |> timespanMiliseconds
    let repository = results.TryGetResult<@Directory@> 
    let push = match results.TryGetResult<@Arguments.Push@> with Some v -> v | None -> false
    
    let commitMessage = "AutoGit - " + System.DateTime.Now.ToShortTimeString()

    let mutable cmds = [
        Git.stageAll;
        Git.commit commitMessage
    ]

    if push then cmds <- List.append cmds [Git.push]

    match repository with
    | Some r -> cmds <- setRepository cmds r
    | None -> ()    

    // Print Git Status once
    Git.status |> runnableProcess |> (fun p -> p())

    let work () =
        runnableCommands cmds |> List.iter (fun c -> c())
    
    // Start loop
    AutoGit.loop interval work

    0 // return an integer exit code