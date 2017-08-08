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
                myNintendoStoreTask.Run()
            end,
            [],
            0,
            60 * 1000
        )
    while true do ()
    0
