module Devices

open System.Diagnostics.Metrics
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Registries

let row f id (RegistryStatus (name,status)) : IView list =
    let emoji = match status with | Online -> "✅" | Offline -> "❌"
    
    [ Button.create [
        Grid.row id
        Grid.column 0
        Button.isEnabled <| (status = Online)
        Button.onClick <| fun _ -> f (Some name)
        Button.content name ]
    
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

let deviceView dev : IView =
    match dev with
    | None -> TextBlock.create []
    | Some d -> Apps.view { name = d; location = "localhost:5000" }
                                                              
let view =
    Component.create("Counter", fun ctx ->
        let totalRegistries = ctx.useState <| List.map statusIO registries
        let active = ctx.useState <| None
        
        StackPanel.create [
            StackPanel.verticalAlignment VerticalAlignment.Center
            StackPanel.horizontalAlignment HorizontalAlignment.Center
            StackPanel.children [
                grid totalRegistries.Current active.Set
                deviceView active.Current
            ]
        ]
    )
    
