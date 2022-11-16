module DebugTest

open FSharpHttpRequest

let run () = async {
    match! Request.getString [||] "http://nodomain.no" with
    | Error err -> 
        ()
    | _ -> ()
}