namespace FSharpHttpRequest

open System
open System.Net
open System.Net.Http
open System.Net.Sockets
open System.Threading.Tasks

open FSharpTools
open FSharpTools.Option

type Status = {
    Code: HttpStatusCode
    Text: string
    Msg:  HttpResponseMessage
}

type Error = 
| Timeout
| HostNotFound     of string
| InvalidOperation of string
| SocketError      of string
| Exception        of Exception
| Status           of Status

module ErrorExt =
    let socketExceptionToError (hre: HttpRequestException) (se: SocketException) =
        match se with
        | se when se.SocketErrorCode = SocketError.HostNotFound -> HostNotFound hre.Message
        | _ -> Error.SocketError se.Message

    let httpRequestExnToError (hre: HttpRequestException) =
        match hre.InnerException with
        | :? SocketException as se -> socketExceptionToError hre se
        | _ -> Exception hre

    let fromException (exn: Exception) =
        match exn with
        | :? InvalidOperationException as ioe -> InvalidOperation ioe.Message
        | :? TaskCanceledException -> Timeout
        | :? HttpRequestException as hre -> httpRequestExnToError hre
        | _ as exn -> Exception exn

    let fromResponse (msg: HttpResponseMessage) =
        match msg.StatusCode with
        | HttpStatusCode.OK -> Ok msg
        | _                 -> Error <| Status { Code = msg.StatusCode; Text = msg.ReasonPhrase; Msg = msg} 
        

