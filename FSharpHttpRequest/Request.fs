namespace FSharpHttpRequest

open System
open System.Net.Http
open System.Threading.Tasks

open FSharpTools
open FSharpTools.Functional
open AsyncResult

module Request = 

    // TODO Result<exn> to Result<HttpRequestError> 
    // SocketError with Message and Code and exn
    // Exception exn
    // StatusCode <> null

    type KeyValue = {
        Key: string
        Value: string
    }

    // TODO FSharpTools
    let toResult (task: Task<'a>) = 
        let continueFrom ((ok: Result<'a, exn> -> Unit), _, _) = 
            let continueWith (task: Task<'a>) =
                let result = 
                    if task.IsCompletedSuccessfully then
                        Ok task.Result
                    elif task.IsFaulted && task.Exception.InnerException <> null then
                        Error task.Exception.InnerException
                    elif task.IsFaulted then
                        Error task.Exception
                    elif task.IsCanceled then
                        Error <| TaskCanceledException ()
                    else
                        Error <| Exception ()
                ok result
            task.ContinueWith continueWith 
            |> ignore

        Async.FromContinuations continueFrom


    let getGet (uri: string) = new HttpRequestMessage (HttpMethod.Get, uri)

    let get uri (headers: KeyValue array) =
    
        let request = getGet uri 
        let addHeader keyValue = 
            request.Headers.TryAddWithoutValidation(keyValue.Key, [|keyValue.Value|])
            |> ignore

        headers
        |> Array.iter addHeader

        Client.get()
            .SendAsync request
            |> toResult


    let getString headers uri = 
        let getString (responseMessage: HttpResponseMessage) = async {
            return! responseMessage.Content.ReadAsStringAsync () |> Async.AwaitTask
        }

        get uri headers 
        |>> getString
