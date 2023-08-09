module Devices

open System.Diagnostics.Metrics
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Bash
open Registries

let row f id (RegistryStatus (registry,status)) : IView list =
    let emoji = match status with | Online -> "✅" | Offline -> "❌"
    
    [ Button.create [
        Grid.row id
        Grid.column 0
        Button.isEnabled <| (status = Online)
        Button.onClick <| fun _ -> f (Some registry)
        Button.content registry.name ]
    
      TextBlock.create [
        Grid.row id
        Grid.column 1
        TextBlock.text emoji
      ] ]

let grid xs f =
    Grid.create
        [ Grid.columnSpan 2
          Grid.margin 4
          Grid.rowDefinitions (RowDefinitions.Parse "*,*")
          Grid.columnDefinitions (ColumnDefinitions.Parse "100,*")
          Grid.children <| ( List.mapi (row f) xs |> List.collect id) ]

let renderAll rs f : IView =
    match rs with
    | Ok xs -> grid xs f
    | Error (NonZeroExitCode s) 
    | Error (CommandNotFound s) -> TextBlock.create [ TextBlock.text $"Something went wrong :( {s}" ] 
    
let deviceView dev : IView =
    match dev with
    | None -> TextBlock.create []
    | Some { name = name; location = location } -> Apps.view { name = name; location = location }
                                                              
let view =
    Component.create("Counter", fun ctx ->
        let totalRegistries = ctx.useState <| statusesIO registries
        let active = ctx.useState <| None
        
        StackPanel.create [
            StackPanel.verticalAlignment VerticalAlignment.Center
            StackPanel.horizontalAlignment HorizontalAlignment.Center
            StackPanel.children [
                renderAll totalRegistries.Current active.Set
                deviceView active.Current
            ]
        ]
    )
    
