module Tests

open Xunit
open Xunit.Abstractions

open FSharpHttpRequest

type ClassDataBase(generator : obj [] seq) = 
    interface seq<obj []> with
        member this.GetEnumerator() = generator.GetEnumerator()
        member this.GetEnumerator() = 
            generator.GetEnumerator() :> System.Collections.IEnumerator

type getStringUrls () = 
    inherit ClassDataBase([ 
        [| null; true |]        
        [| ""; true |]        
        [| "dump"; true |]        
        [| "http://nodomain.no"; true |]        
        [| "https://caesar2go.caseris.de:999"; true |]        
        [| "https://caesar2go.caseris.de/nopath"; true |]        
        [| "http://checkip.dyndns.org"; true |] 
    ])

type Tests(output: ITestOutputHelper) =
    [<Theory>]
    [<ClassData(typeof<getStringUrls>)>]
    let getString (url, result) = 
        sprintf "\n=================================================================\ngetString - %s\n" url
        |> output.WriteLine 
        
        async {
            let settings = { Request.defaultSettings with Url = url }
        
            match! Request.getString settings with
            | Ok ok  -> sprintf "OK:\n%s" ok |> output.WriteLine
            | Error err -> sprintf "Err:\n%O" err |> output.WriteLine
            Assert.Equal(true, result)
        } 
        |> Async.RunSynchronously   
        
        