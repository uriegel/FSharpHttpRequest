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
        [| null; 34 |]        
        [| ""; 34 |]        
        [| "dump"; 34 |]        
        [| "http://nodomain.no"; 34 |]        
        [| "https://caesar2go.caseris.de:999"; 34 |]        
        [| "https://caesar2go.caseris.de/nopath"; 34 |]        
        [| "http://checkip.dyndns.org"; 34 |] 
    ])

type Tests(output: ITestOutputHelper) =
    [<Theory>]
    [<ClassData(typeof<getStringUrls>)>]
    let getString (url, result) = 
        output.WriteLine (sprintf "\n=================================================================\ngetString - %s\n" url)
        async {
            let settings = { Request.defaultSettings with Url = url }
        
            match! Request.getString settings with
            | Ok ok  -> output.WriteLine (sprintf "OK:\n%s" ok)
            | Error err -> 
                output.WriteLine (sprintf "Err:\n%O" err)
            Assert.Equal(34, result)
        } 
        |> Async.RunSynchronously   
        
        