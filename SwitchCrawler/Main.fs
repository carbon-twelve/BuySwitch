module Main

open System
open OpenQA.Selenium.Firefox
open Program
open System.Diagnostics


[<EntryPoint>]
let main argv =
    use timer =
        new Threading.Timer(
            begin fun stateInfo ->
                通販TaskList
                |> List.iter (fun task -> task.Run())
            end,
            [],
            0,
            60 * 1000
        )
    while true do ()
    0
