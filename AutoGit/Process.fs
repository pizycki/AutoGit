namespace AutoGit

open System.Diagnostics

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
        