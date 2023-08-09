module Apps 

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI
open Avalonia.Layout
open Registries
open GridComponent
open Bash

let display res =
     match res with
     | Ok imgList -> grid [imgList]
     | Error (NonZeroExitCode e)
     | Error (CommandNotFound e) -> TextBlock.create [ TextBlock.text e ]

let view registry =
    Component.create("Registry View", fun ctx ->
        let state = ctx.useState <| inspectIO registry (Container "newapp")
        
        StackPanel.create [
            StackPanel.verticalAlignment VerticalAlignment.Center
            StackPanel.horizontalAlignment HorizontalAlignment.Center
            StackPanel.children [ display state.Current ]
        ]
    )
    