module AutoPurchaseTest

open NUnit.Framework
open NUnit.Framework.Constraints
open OpenQA.Selenium.Firefox
open System.IO
open Program
open Chiron
open System
open OpenQA.Selenium.Chrome

let config: Config = File.ReadAllText("config.json") |> Json.parse |> Json.deserialize
let driver = new ChromeDriver()

[<Test>]
let トイザらスTest() =
    let url = "https://www.toysrus.co.jp/s/dsg-580786700"
    driver.Navigate().GoToUrl(url)
    //Program.トイザらス購入 url driver config.Credentials.["トイザらス"] true
    ()

[<Test>]
let ヨドバシTest() =
    let url = "http://www.yodobashi.com/product/100000001003431580/"
    //Program.ヨドバシ.Buy(driver)
    ()

[<Test>]
let 楽天ブックスTest() =
    let url = "http://books.rakuten.co.jp/rb/14943341/"
    let 通販 = Program.楽天ブックス({ config.Stores.["楽天ブックス"] with Urls = [url] }, true) :> 通販
    通販.Buy(driver)
    ()
