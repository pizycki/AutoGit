namespace AutoGit

open System

module Common = 
    let timespanMiliseconds (ts:TimeSpan):int = int ts.TotalMilliseconds
