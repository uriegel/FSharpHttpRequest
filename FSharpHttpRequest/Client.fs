module Client

open System.Net.Http

open FSharpTools.Functional

let mutable private maxConnections = 8

let initServerConnections maxCount =
    maxConnections <- maxCount

let get =  
    let getHttpHandler () = 
        let handler = new HttpClientHandler()
        handler.MaxConnectionsPerServer <- maxConnections
        handler

    let getHttpClient () = new HttpClient(getHttpHandler ())
    memoizeSingle getHttpClient
