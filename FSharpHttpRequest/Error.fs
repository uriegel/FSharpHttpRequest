namespace FSharpHttpRequest

open System
open System.Threading.Tasks

type Error = 
    | Timeout
    | HostNotFound     of string
    | InvalidOperation of string
    | SocketError      of string
    | Exception        of Exception

module ErrorExt =
    open System.Net.Http
    open System.Net.Sockets
    open FSharpTools.Option

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

