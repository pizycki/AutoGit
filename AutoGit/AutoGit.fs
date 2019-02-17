namespace AutoGit

module AutoGit =

    open Argu
    open Process
    open Git

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
