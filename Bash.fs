module Bash

open System.Text.Json
open System.Text.Json.Serialization
open Fli

let parseOutput (output: Output) =
   match output with
   | { ExitCode = 0 } -> Ok output
   | { ExitCode = code; Error = Some t } -> Error $"Error running command: {code} - {t}"
   | { ExitCode = code } -> Error $"Error running command: {code}"
   
let captureIO<'T> formatter : (ExecContext -> Result<'T,string>) =
    Command.execute >>
    parseOutput >>
    Result.map Output.toText >>
    Result.map formatter
    
let shellIO exec args formatter = cli { Exec exec; Arguments args; } |> captureIO formatter