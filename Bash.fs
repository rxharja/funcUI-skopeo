module Bash

open Fli

type ErrorType =
    | NonZeroExitCode of string
    | CommandNotFound of string

let parseOutput (output: Output) =
   match output with
   | { ExitCode = 0 } -> Ok output
   | { ExitCode = code; Error = Some t } -> Error (NonZeroExitCode $"Error running command: {code} - {t}")
   | { ExitCode = code } -> Error (NonZeroExitCode $"Error running command: {code}")
   
let captureIO<'T> formatter : (ExecContext -> Result<'T,ErrorType>) =
    Command.execute >>
    parseOutput >>
    Result.map Output.toText >>
    Result.map formatter
    
let shellIO exec args formatter =
    try
        cli { Exec exec; Arguments args; } |> captureIO formatter
    with | _ -> Error (CommandNotFound $"{exec} not found")