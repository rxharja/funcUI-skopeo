module Counter 

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI
open Avalonia.Layout
open Registries
open GridComponent

let display res =
     match res with
     | Ok imgList -> grid [imgList]
     | Error error -> TextBlock.create [ TextBlock.text error ]

let view registry =
    Component.create("Counter", fun ctx ->
        let state = ctx.useState <| inspectIO registry (Container "newapp")
        
        StackPanel.create [
            StackPanel.verticalAlignment VerticalAlignment.Center
            StackPanel.horizontalAlignment HorizontalAlignment.Center
            StackPanel.children [ display state.Current ]
        ]
    )
    