namespace FSharpHttpRequest

open System.Net.Http

type Version = {
    Major: int
    Minor: int
}

type Header = {
    Key: string
    Value: string
}

type Settings = {
    Method:     HttpMethod
    BaseUrl:    string option
    Url:        string
    Version:    Version
    Headers:    Header array option
    AddContent: (Unit->HttpContent) option
}

