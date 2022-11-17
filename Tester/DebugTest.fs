module DebugTest

open FSharpHttpRequest

let run () = async {
    let settings = { 
        Request.defaultSettings with 
            // Url = "http://nodomain.no" 
            Url = "http://checkip.dyndns.org"
            // Url = "https://caesar2go.caseris.de/nopath"
    } 
    
    match! Request.getString settings with
    | Ok ok  -> printfn "OK: {%s}" ok
    | Error err -> 
        let affe = exn
        printfn "Ende"
        ()
}