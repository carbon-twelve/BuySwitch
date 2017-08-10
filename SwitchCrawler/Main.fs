module Main

open System
open OpenQA.Selenium.Firefox
open Program
open System.Diagnostics


[<EntryPoint>]
let main argv =
    let timers =
        通販TaskList
        |> List.map begin fun task ->
            new Threading.Timer(
                begin fun stateInfo ->
                    task.Run()
                end,
                [],
                0,
                60 * 1000
            )
        end
    while true do ()
    0
