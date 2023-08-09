module Registries

open System
open System.Globalization
open System.Text.Json
open Bash

type Registry = { name: string; location : string }

type Container = Container of string

type Status = | Online | Offline

type RegistryStatus = RegistryStatus of Registry * Status

type ImageResult =
   { Name: string option
     RepoTags: string list option
     Created: DateTime
     Digest: string }
   
type Image =
    { name: string
      latestTag: string 
      created: DateTime
      digest: string }
   
let imageName (fullName: string) =
    fullName.Split('/')
    |> Array.toList
    |> List.last
    |> fun x -> x.ToCharArray()
    |> Array.toList
    |> List.takeWhile (fun x -> x <> ':')
    |> fun x -> String.Join("", x)
    |> CultureInfo.CurrentCulture.TextInfo.ToTitleCase
    
let rec parseTags tagOp =
    match tagOp with
    | None | Some [] -> "No tags provided :("
    | Some(x :: xs) -> if x = "latest" then parseTags (Some xs) else x
    
let parseName nameOp =
    match nameOp with
    | None | Some "" -> "No name provided :("
    | Some x -> x
    
let parseImageFromRes { RepoTags = tags; Name = name; Created = c; Digest = d } =
    { latestTag = parseTags tags
      name = name |> parseName |> imageName
      created = c
      digest = d }

let parseStatus registry res =
    match res with
    | Ok _                      -> Ok <| RegistryStatus (registry, Online)
    | Error (NonZeroExitCode _) -> Ok <| RegistryStatus (registry,Offline)
    | Error (CommandNotFound s) -> Error <| CommandNotFound s
    
let statusIO registry =
    shellIO "curl" ["-Is"; registry.location ] id |> parseStatus registry

let statusesIO (rs: Registry list) =
    List.fold
        (fun res r ->
            match (statusIO r, res) with
            | Ok s, Ok xs -> Ok(s :: xs)
            | _, Error e
            | Error e, _ -> Error e)
        (Ok [])
        rs    
    
let inspectIO { location = location } (Container container) =
    shellIO "skopeo"
            [ "inspect"; $"docker://{location}/{container}"; "--tls-verify=false" ] 
            JsonSerializer.Deserialize
    |> Result.map parseImageFromRes
    
let registries = [
    { name = "Hub"; location = "localhost:5000" }
    { name = "Edge"; location = "localhost:5001" }
]