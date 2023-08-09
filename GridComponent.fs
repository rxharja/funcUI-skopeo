module GridComponent

open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.Types
open Avalonia.FuncUI.DSL
open Registries

let buildRow id img: IView list =
    [ TextBlock.create
          [ TextBlock.dock Dock.Top
            Grid.row <| id + 1
            Grid.column 0
            TextBlock.fontSize 20.0
            TextBlock.text img.name ]
          
      TextBlock.create [
            TextBlock.dock Dock.Top
            Grid.row <| id + 1
            Grid.column 2
            TextBlock.fontSize 20.0
            TextBlock.text <| img.latestTag ]
      ]
    
let buildGrid rows : IView =
    let rec rowString length =
        match length with
        | 0 -> []
        | n -> "*" :: rowString (n - 1)
        
    let rowDef = rowString >> String.concat ","

    Component.create("grid", fun _ ->
        Grid.create [
            Grid.columnSpan 2
            Grid.margin 4
            Grid.rowDefinitions (RowDefinitions.Parse (List.length rows |> rowDef))
            Grid.columnDefinitions (ColumnDefinitions.Parse "300,*")
            Grid.children
                ( TextBlock.create [
                    TextBlock.dock Dock.Top
                    Grid.row 0
                    Grid.column 0
                    TextBlock.fontSize 20.0
                    TextBlock.text <| "App" ] ::
                  TextBlock.create [
                    TextBlock.dock Dock.Top
                    Grid.row 0
                    Grid.column 1
                    TextBlock.fontSize 20.0
                    TextBlock.text <| "Version" ] :: rows) 
        ] )
    
let grid = List.mapi buildRow >> List.collect id >> buildGrid
