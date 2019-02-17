namespace AutoGit

module Git =
            
    open Process
    open System
    
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
