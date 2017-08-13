module Program

open System
open System.Text.RegularExpressions
open System.Globalization
open OpenQA.Selenium
open CoreTweet
open System.Diagnostics
open System.IO

open Chiron.Mapping
open Chiron
open OpenQA.Selenium.Remote
open OpenQA.Selenium.Chrome
open RestSharp
open System.Collections.Generic
open System.Collections.Concurrent
open NLog

LogManager.ThrowExceptions <- true
let logger = LogManager.GetCurrentClassLogger()

type AccessToken = { Token: string; Secret: string } with
    static member ToJson (a: AccessToken) = json {
        do! Json.write "token" a.Token
        do! Json.write "secret" a.Secret
    }
    static member FromJson (_: AccessToken) = json {
        let! token = Json.read "token"
        let! secret = Json.read "secret"
        return { Token = token; Secret = secret }
    }

type Consumer = { Key: string; Secret: string } with
    static member ToJson (c: Consumer) = json {
        do! Json.write "key" c.Key
        do! Json.write "secret" c.Secret    
    }
    static member FromJson (_: Consumer) = json {
        let! key = Json.read "key"
        let! secret = Json.read "secret"
        return { Key = key; Secret = secret }
    }

type Credential = { Id: string; Password: string } with
    static member ToJson (c: Credential) = json {
        do! Json.write "id" c.Id
        do! Json.write "password" c.Password
    }
    static member FromJson (_: Credential) = json {
        let! id = Json.read "id"
        let! password = Json.read "password"
        return { Id = id; Password = password }
    }
(*
type PaymentInfo = {} with
    static member ToJson (p: PaymentInfo) = json {
    }
    static member FromJson (_: PaymentInfo) = json {
        return {}
    }
*)

type StoreInfo = { Credential: Credential; Urls: string list } with
    static member ToJson (s: StoreInfo) = json {
        do! Json.write "credential" s.Credential
        do! Json.write "urls" s.Urls
    }
    static member FromJson (_: StoreInfo) = json {
        let! credential = Json.read "credential"
        let! urls = Json.read "urls"
        return { Credential = credential; Urls = urls }
    }

type Config =
    {
        Stores: Map<string, StoreInfo>
        DryRun: bool
        IftttKey: string
    } with
    static member ToJson (c: Config) = json {
        do! Json.write "stores" c.Stores
        do! Json.write "dryRun" c.DryRun
        do! Json.write "iftttKey" c.IftttKey
    }
    static member FromJson (_: Config) = json {
        let! stores = Json.read "stores"
        let! dryRun = Json.read "dryRun"
        let! iftttKey = Json.read "iftttKey"
        return { Stores = stores; DryRun = dryRun; IftttKey = iftttKey }
    }

let config: Config =
    let configFilePath =
        [ "config.json"; "../../config.json"]
        |> List.map Path.GetFullPath
        |> List.find File.Exists
    logger.Info("Using {0} as a config file", configFilePath)
    let config =
        File.ReadAllText(configFilePath)
        |> Json.parse
        |> Json.deserialize
    config


type WebDriverPool(driverCount: int) =
    let drivers = new BlockingCollection<RemoteWebDriver>(driverCount)

    do
        Seq.init driverCount (fun _ -> new ChromeDriver("./drivers") :> RemoteWebDriver)
        |> Seq.iter (fun driver -> drivers.Add(driver))

    member this.Loan(f: RemoteWebDriver -> 'T): 'T =
        let driver = drivers.Take()
        let result = f driver
        drivers.Add(driver)
        result
        
let webDriverPool = WebDriverPool(3)

(*
let ``オムニセブン`` (driver: FirefoxDriver) =
    driver.FindElementByCssSelector("input.js-pressTwice").Enabled

let ``オムニセブン購入`` (driver: FirefoxDriver) (credential: Credential) (dryRun: bool)=
    driver.FindElementByCssSelector("input.js-pressTwice").Click()
    driver.FindElementByCssSelector(".js-weightLimitBtn").Click()
    driver.FindElementByCssSelector("input[name='login']").SendKeys(credential.Id)
    driver.FindElementByCssSelector("input[name='password']").SendKeys(credential.Password)
    driver.FindElementByCssSelector("#loginBtn").Click()
    driver.FindElementByCssSelector("input[name='accptKbn']").Click()
    driver.FindElementByCssSelector(".js-weightLimitBtn").Click()
    driver.FindElementByCssSelector("input[name='pay_mthd'][value='02']").Click()
    driver.FindElementByCssSelector("input.js-pressTwice").Click()
    //if not dryRun then driver.FindElementByCssSelector("input[name='btnConfirm']").Click()

let ``ノジマオンラインUrlList`` =
    [
        "https://online.nojima.co.jp/Nintendo-HAC-S-KACEA-%E3%80%90Switch%E3%80%91-%E3%83%8B%E3%83%B3%E3%83%86%E3%83%B3%E3%83%89%E3%83%BC%E3%82%B9%E3%82%A4%E3%83%83%E3%83%81%E6%9C%AC%E4%BD%93-%E3%82%B9%E3%83%97%E3%83%A9%E3%83%88%E3%82%A5%E3%83%BC%E3%83%B32%E3%82%BB%E3%83%83%E3%83%88/4902370537338/1/cd/";
        "https://online.nojima.co.jp/Nintendo-HAC-S-KAAAA-%E3%80%90Switch%E3%80%91-%E3%83%8B%E3%83%B3%E3%83%86%E3%83%B3%E3%83%89%E3%83%BC%E3%82%B9%E3%82%A4%E3%83%83%E3%83%81%E6%9C%AC%E4%BD%93-Joy-Con%28L%29-%28R%29-%E3%82%B0%E3%83%AC%E3%83%BC/4902370535709/1/cd/";
        "https://online.nojima.co.jp/Nintendo-HAC-S-KABAA-%E3%80%90Switch%E3%80%91-%E3%83%8B%E3%83%B3%E3%83%86%E3%83%B3%E3%83%89%E3%83%BC%E3%82%B9%E3%82%A4%E3%83%83%E3%83%81%E6%9C%AC%E4%BD%93-Joy-Con%28L%29-%E3%83%8D%E3%82%AA%E3%83%B3%E3%83%96%E3%83%AB%E3%83%BC-%28R%29-%E3%83%8D%E3%82%AA%E3%83%B3%E3%83%AC%E3%83%83%E3%83%89/4902370535716/1/cd/";
    ]

let ``ノジマオンライン`` (driver: FirefoxDriver) =
    not (driver.FindElementByCssSelector(".hassoumeyasu2 > strong:nth-child(1) > strong:nth-child(2) > span:nth-child(1)").Text.Contains("完売御礼"))

let ``あみあみUrlList`` =
    [
        "http://www.amiami.jp/top/detail/detail?gcode=GAME-0018286";
        "http://www.amiami.jp/top/detail/detail?gcode=GAME-0017599&page=top%2Fsearch%2Flist%3Finc_txt2%3D15%24s_cate3%3D10054%24s_sortkey%3Dpriced%24pagemax%3D40%24getcnt%3D0%24pagecnt%3D1<";
        "http://www.amiami.jp/top/detail/detail?gcode=GAME-0017598&page=top%2Fsearch%2Flist%3Finc_txt2%3D15%24s_cate3%3D10054%24s_sortkey%3Dpriced%24pagemax%3D40%24getcnt%3D0%24pagecnt%3D1";
    ]

let ``あみあみ`` (driver: FirefoxDriver) =
    not (driver.FindElementByCssSelector("#right_menu > span:nth-child(2) > img:nth-child(1)").GetAttribute("alt").Contains("販売停止中"))

let ``TSUTAYAUrlList`` =
    [
        "http://shop.tsutaya.co.jp/game/product/4902370537338/";
        "http://shop.tsutaya.co.jp/game/product/4902370535716/";
        "http://shop.tsutaya.co.jp/game/product/4902370535709/";
    ]

let ``TSUTAYA`` (driver: FirefoxDriver) =
    not (driver.FindElementByCssSelector("li.tolBtn > img:nth-child(1)").GetAttribute("alt").Contains("在庫なし"))

let 通販 (driver: FirefoxDriver) (通販strategy: FirefoxDriver -> bool, urlList: string list) =
    try
        urlList
        |> List.tryFind
            begin fun url ->
                driver.Navigate().GoToUrl(url)
                通販strategy driver
            end
    with
    | e -> Debug.WriteLine(sprintf "Crawling failed: %A" e); None
*)

let ifttt () =
    let client = RestClient("https://maker.ifttt.com")
    let request = RestRequest("trigger/my_nintendo_store/with/key/{key}", Method.GET)
    request.AddUrlSegment("key", config.IftttKey) |> ignore
    client.Execute(request) |> ignore
    logger.Info("Sent an event to IFTTT")

type 通販 =
    abstract member Buy: RemoteWebDriver -> unit

type 通販Task(run: unit -> unit) =
    member this.Run() = run()

type トイザらス(storeInfo: StoreInfo, dryRun: bool) =
    let isAvailable (driver: RemoteWebDriver) =
        try
            driver.FindElementByCssSelector("#isStock_c").GetAttribute("hidden") <> null
        with
        | _ -> false

    interface 通販 with
        member this.Buy(driver) =
            for url in storeInfo.Urls do
                try
                    driver.Navigate().GoToUrl(url)
                    if isAvailable driver then
                        logger.Info("Switch is available at トイザらス. Trying to buy...")
                        driver.FindElementByCssSelector(".quick-note > a:nth-child(1)").Click()
                        driver.FindElementByCssSelector("input#mail").SendKeys(storeInfo.Credential.Id)
                        driver.FindElementByCssSelector("input#pw").SendKeys(storeInfo.Credential.Password)
                        driver.FindElementByCssSelector(".exbutton").Click()
                        driver.Navigate().GoToUrl(url)
                        driver.FindElementByCssSelector("li.quick > button.exbutton").Click()
                        if not dryRun then
                            driver.FindElementByCssSelector(".exbutton").Click()
                            ifttt()
                            Environment.Exit(0)
                    else
                        logger.Info("Switch is NOT available at トイザらス at this moment.")
                with
                | e -> logger.Warn(e, "Exception occurred during トイザらス operation.")


type ヨドバシ(storeInfo: StoreInfo, dryRun: bool) =
    let isAvailable (driver: RemoteWebDriver) =
        try
            driver.FindElementByCssSelector("#js_m_submitRelated") |> ignore
            true
        with
        | _ -> false

    interface 通販 with
        member this.Buy(driver) =
            for url in storeInfo.Urls do
                try
                    driver.Navigate().GoToUrl(url)
                    if isAvailable driver then
                        logger.Info("Switch is available at ヨドバシ.com. Trying to buy...")
                        driver.FindElementByCssSelector("#js_m_submitRelated").Click()
                        driver.FindElementByCssSelector(".buyButton a[href='/yc/shoppingcart/index.html?next=true']").Click()
                        driver.FindElementByCssSelector(".checkout a").Click()
                        driver.FindElementByCssSelector("input#memberId").SendKeys(storeInfo.Credential.Id)
                        driver.FindElementByCssSelector("input#password").SendKeys(storeInfo.Credential.Password)
                        driver.FindElementByCssSelector("#js_i_login0").Click()
                        driver.FindElementByCssSelector("#sc_i_buy").Click()
                        driver.FindElementByCssSelector("#a04").Click()
                        driver.FindElementByCssSelector(".js_c_next").Click()
                        if not dryRun then
                            driver.FindElementByCssSelector("div.buyButton a").Click()
                            ifttt()
                            Environment.Exit(0)
                    else
                        logger.Info("Switch is NOT available at ヨドバシ.com at this moment.")
                with
                | e -> logger.Warn(e, "Exception occurred during ヨドバシ.com operation.")


type 楽天ブックス(storeInfo: StoreInfo, dryRun: bool) =
    let isAvailable (driver: RemoteWebDriver) =
        try
            driver.FindElementByCssSelector(".new_addToCart") |> ignore
            true
        with
        | _ -> false
    
    interface 通販 with
        member this.Buy(driver) =
            for url in storeInfo.Urls do
                try
                    driver.Navigate().GoToUrl(url)
                    if isAvailable driver then
                        logger.Info("Switch is available at 楽天ブックス. Trying to buy...")
                        driver.FindElementByCssSelector(".new_addToCart").Click()
                        driver.FindElementByCssSelector("#js-cartBtn").Click()
                        driver.FindElementByCssSelector("input[name='u']").SendKeys(storeInfo.Credential.Id)
                        driver.FindElementByCssSelector("input[name='p']").SendKeys(storeInfo.Credential.Password)
                        driver.FindElementByCssSelector("form#login_valid button[name='submit']").Click()
                        if not dryRun then
                            driver.FindElementByCssSelector("button[name='commit_order']").Click()
                            ifttt()
                    else
                        logger.Info("Switch is not available at 楽天ブックス at this moment.")
                with
                | e -> logger.Warn(e, "Exception occurred during 楽天ブックス operation.")

type MyNintendoStore(storeInfo: StoreInfo) =
    member this.IsAvailable(driver: RemoteWebDriver) =
        storeInfo.Urls
        |> List.exists begin fun url ->
            driver.Navigate().GoToUrl(url)
            not (driver.FindElementByCssSelector("p.stock").Text.Contains("SOLD OUT2"))
        end

let myNintendoStoreTask =
    let myNintendoStore = MyNintendoStore(config.Stores.["マイニンテンドーストア"])
    let run () =
        if webDriverPool.Loan(myNintendoStore.IsAvailable) then ifttt ()
    通販Task(run)

let 通販TaskList =
    [
        トイザらス(config.Stores.["トイザらス"], config.DryRun) :> 通販
        ヨドバシ(config.Stores.["ヨドバシ.com"], config.DryRun) :> 通販
        楽天ブックス(config.Stores.["楽天ブックス"], config.DryRun) :> 通販
    ]
    |> List.map begin fun 通販 ->
        let run () =
            webDriverPool.Loan(通販.Buy)
        通販Task(run)
    end

