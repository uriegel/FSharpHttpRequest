namespace FSharpHttpRequest

open System
open System.Text

module BasicAuthentication =
    
    let addHeader name passwd = 
        {
            Key = "Authorization"
            Value = "Basic " + Convert.ToBase64String (Encoding.UTF8.GetBytes (name + ":" + passwd))
        }
