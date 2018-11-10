open System
open Argu
open System.Diagnostics

module Git =
    
    type GitArgName = string
    type GitArgValue = string
    type GitArg = GitArgName * GitArgValue
    type GitRepository = string  

    let directoryArg (repo:GitRepository option): GitArg option =
        match repo with
        | Some r -> Some ("-C", r)
        | _ -> None
    
//    let commitAll 

    let printStatus () =
        use p = new Process()
        p.StartInfo <- ProcessStartInfo("git", "status")
        p.Start() |> ignore
        ()   

open Git
module AutoGit =
    type Arguments =
        | Interval of int
        | Directory of dir:string
        | Push of bool
    with
        interface IArgParserTemplate with
            member arg.Usage =
                match arg with
                | Interval _ -> "Commit interval in minutes"
                | Directory _ -> "Path to Git repository directory"
                | Push _ -> "Pushes changes to remote with each commit"
    
    type CommitInterval = TimeSpan  

    type AutoCommitArguments = {
        Interval: CommitInterval
        Repository: GitRepository option
        Push: bool
    }

    let loop interval work =
        while true do
            let async = async {
                do! Async.Sleep interval
                work()
            }
            Async.RunSynchronously async

    let convert (args:AutoCommitArguments) = 
        let gitArgs = [
            directoryArg args.Repository;
        ]

        let f a = 
            match a with 
            | Some x -> [fst x; snd x]
            | None -> []

        List.collect f gitArgs

open AutoGit
[<EntryPoint>]
let main argv =
    let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)
    let parser = ArgumentParser.Create<Arguments>(programName = "ls", errorHandler = errorHandler)     
    let results = parser.ParseCommandLine argv

    let interval = match results.TryGetResult<@Arguments.Interval@> with Some v -> v | None -> 3600
    let repository = results.TryGetResult<@Directory@> 
    let push = match results.TryGetResult<@Arguments.Push@> with Some v -> v | None -> false
    
    let commitArgs = {
        Interval = TimeSpan.FromSeconds (float interval);
        Repository = repository;
        Push = push
    }

    printfn "Bla %A" (convert commitArgs |> List.toArray)

    0 // return an integer exit code
