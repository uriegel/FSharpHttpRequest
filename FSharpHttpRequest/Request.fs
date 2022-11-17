namespace FSharpHttpRequest

open System.Net.Http

open FSharpTools
open AsyncResult
open ErrorExt

module Request = 

    let defaultSettings = {
        Method = HttpMethod.Get
        BaseUrl = None
        Url = ""
        Version = { 
            Major = 2
            Minor = 0 
        }
        Headers = None
        AddContent = None
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
        let sendAsync () = 
            let request = createRequest settings

            match settings.AddContent with
            | Some addContent -> request.Content <- addContent ()
            | None            -> ()

            addHeaders request settings

            Client
                .get()
                .SendAsync request

        sendAsync 
        |> catch
        |> mapError ErrorExt.fromException
        >>= (fromResponse >> Async.toAsync)

    let getString settings = 
        let getString (responseMessage: HttpResponseMessage) = 
            let read () = responseMessage.Content.ReadAsStringAsync ()
            read
            |> catch
            |> mapError ErrorExt.fromException

        request settings
        >>= getString
