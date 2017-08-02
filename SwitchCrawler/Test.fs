module AutoPurchaseTest

open NUnit.Framework
open NUnit.Framework.Constraints
open OpenQA.Selenium.Firefox
open System.IO
open Program
open Chiron
open System

let config: Config = File.ReadAllText("config.json") |> Json.parse |> Json.deserialize
let driver = new FirefoxDriver(FirefoxProfile())

[<Test>]
let トイザらスTest() =
    let url = "https://www.toysrus.co.jp/s/dsg-580786700"
    driver.Navigate().GoToUrl(url)
    //Program.トイザらス購入 url driver config.Credentials.["トイザらス"] true
    ()

[<Test>]
let ヨドバシTest() =
    let url = "http://www.yodobashi.com/product/100000001003431580/"
    Program.ヨドバシ.Buy(driver)