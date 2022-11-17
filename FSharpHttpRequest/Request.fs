namespace FSharpHttpRequest

open System.Net.Http

open FSharpTools
open AsyncResult
open Async

module Request = 
    // TODO Result<exn> to Result<HttpRequestError> 
    // SocketError with Message and Code and exn
    // Exception exn
    // StatusCode <> null

    let defaultSettings = {
        Method = HttpMethod.Get
        BaseUrl = None
        Url = ""
        Version = { 
            Major = 2
            Minor = 0 
        }
        Headers = None
    }

    let createRequest settings = 

        let msg = new HttpRequestMessage (
            settings.Method,
            (settings.BaseUrl |> Option.defaultValue "") + settings.Url)
        msg.Version <- System.Version(settings.Version.Major, settings.Version.Minor)
        msg

    let addHeaders msg settings = 
        let addHeader (msg: HttpRequestMessage) header =
            if msg.Headers.TryAddWithoutValidation(header.Key, header.Value) = false then
                if msg.Content <> null then
                    msg.Content.Headers.TryAddWithoutValidation(header.Key, header.Value) |> ignore

        match settings.Headers with
        | Some headers -> 
            headers
            |> Array.iter (addHeader msg)
        | None -> ()

    let request settings =     
        let request = createRequest settings
        addHeaders request settings

        Client.get()
            .SendAsync request
            |> toResult

    let getString settings = 
        let getString (responseMessage: HttpResponseMessage) = async {
            return! responseMessage.Content.ReadAsStringAsync () |> Async.AwaitTask
        }

        request settings
        |>> getString
