module Client

open System.Net.Http

open FSharpTools.Functional

let mutable private maxConnections = 8

let initServerConnections maxCount =
    maxConnections <- maxCount

let get =  
    let getHttpClient () = new HttpClient(
        new HttpClientHandler(
            MaxConnectionsPerServer = maxConnections
        )
    )
    memoizeSingle getHttpClient
