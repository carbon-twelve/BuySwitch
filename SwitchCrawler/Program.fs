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
        Consumer: Consumer;
        AccessToken: AccessToken option;
        Stores: Map<string, StoreInfo>
        DryRun: bool
    } with
    static member ToJson (c: Config) = json {
        do! Json.write "consumer" c.Consumer
        do! Json.write "accessToken" c.AccessToken
        do! Json.write "stores" c.Stores
        do! Json.write "dryRun" c.DryRun
    }
    static member FromJson (_: Config) = json {
        let! consumer = Json.read "consumer"
        let! accessToken = Json.read "accessToken"
        let! stores = Json.read "stores"
        let! dryRun = Json.read "dryRun"
        return { Consumer = consumer; AccessToken = accessToken; Stores = stores; DryRun = dryRun }
    }

let config: Config =
    let configFilePath =
        ["../../config.json"; "config.json"]
        |> List.find File.Exists
    let config =
        File.ReadAllText(configFilePath)
        |> Json.parse
        |> Json.deserialize
    match config.AccessToken with
    | None ->
        let session = OAuth.Authorize(config.Consumer.Key, config.Consumer.Secret)
        Debug.WriteLine("Access here: {0}", session.AuthorizeUri)
        let pincode = Console.ReadLine()
        let tokens = OAuth.GetTokens(session, pincode)
        let newConfig = { config with AccessToken = Some { Token = tokens.AccessToken; Secret = tokens.AccessTokenSecret } }
        newConfig |> Json.serialize |> Json.format |> (fun content -> File.WriteAllText(configFilePath, content))
        newConfig
    | _ -> config



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


    
let ``ヤマダウェブコムUrlList`` =
    [
        "http://www.yamada-denkiweb.com/1178028018";
    ]

let ``ヤマダウェブコム`` (driver: FirefoxDriver) =
    driver.FindElementByCssSelector(".side_button > button:nth-child(1)").Enabled

let ``amazon.co.jpUrlList`` =
    [
        "http://amzn.asia/drmTPI8";
        "http://amzn.asia/46UEI2w";
        //"http://amzn.asia/631kePg";
    ]

let ``amazon.co.jp`` (driver: FirefoxDriver) =
    try 
        driver.FindElementByCssSelector("#add-to-cart-button") |> ignore
        true
    with
    | :? NoSuchElementException -> false

let ``amazon.co.jp2UrlList`` =
    []

let ``amazon.co.jp2`` (driver: FirefoxDriver) =
    let regexMatch = Regex.Match(driver.FindElementByCssSelector("#priceblock_ourprice").Text, "￥ (.*)")
    Int32.Parse(regexMatch.Groups.[1].Value, NumberStyles.Any, CultureInfo.GetCultureInfo("ja-JP")) < 49800

let ``ノジマオンラインUrlList`` =
    [
        "https://online.nojima.co.jp/Nintendo-HAC-S-KACEA-%E3%80%90Switch%E3%80%91-%E3%83%8B%E3%83%B3%E3%83%86%E3%83%B3%E3%83%89%E3%83%BC%E3%82%B9%E3%82%A4%E3%83%83%E3%83%81%E6%9C%AC%E4%BD%93-%E3%82%B9%E3%83%97%E3%83%A9%E3%83%88%E3%82%A5%E3%83%BC%E3%83%B32%E3%82%BB%E3%83%83%E3%83%88/4902370537338/1/cd/";
        "https://online.nojima.co.jp/Nintendo-HAC-S-KAAAA-%E3%80%90Switch%E3%80%91-%E3%83%8B%E3%83%B3%E3%83%86%E3%83%B3%E3%83%89%E3%83%BC%E3%82%B9%E3%82%A4%E3%83%83%E3%83%81%E6%9C%AC%E4%BD%93-Joy-Con%28L%29-%28R%29-%E3%82%B0%E3%83%AC%E3%83%BC/4902370535709/1/cd/";
        "https://online.nojima.co.jp/Nintendo-HAC-S-KABAA-%E3%80%90Switch%E3%80%91-%E3%83%8B%E3%83%B3%E3%83%86%E3%83%B3%E3%83%89%E3%83%BC%E3%82%B9%E3%82%A4%E3%83%83%E3%83%81%E6%9C%AC%E4%BD%93-Joy-Con%28L%29-%E3%83%8D%E3%82%AA%E3%83%B3%E3%83%96%E3%83%AB%E3%83%BC-%28R%29-%E3%83%8D%E3%82%AA%E3%83%B3%E3%83%AC%E3%83%83%E3%83%89/4902370535716/1/cd/";
    ]

let ``ノジマオンライン`` (driver: FirefoxDriver) =
    not (driver.FindElementByCssSelector(".hassoumeyasu2 > strong:nth-child(1) > strong:nth-child(2) > span:nth-child(1)").Text.Contains("完売御礼"))

let ``楽天ブックスUrlList`` =
    [
        "http://ts.books.rakuten.co.jp/rb/14647221/";
        "http://ts.books.rakuten.co.jp/rb/14647222/";
        "http://ts.books.rakuten.co.jp/rb/14943334/";
    ]

let ``楽天ブックス`` (driver: FirefoxDriver) =
    not (driver.FindElementByCssSelector(".status").Text.Contains("ご注文できない商品*"))

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

let getTwitterTokens () =
    match config.AccessToken with
    | Some accessToken -> Tokens.Create(config.Consumer.Key, config.Consumer.Secret, accessToken.Token, accessToken.Secret)

let tweet (o: obj) =
    let rawStatus = sprintf "%A" o
    let status = rawStatus.Substring(0, Math.Min(139, rawStatus.Length))
    Debug.WriteLine("Tweeting: " + status)
    try
        let tokens = getTwitterTokens()
        tokens.Statuses.Update(status) |> ignore
    with
    | e -> Debug.WriteLine(sprintf "Tweet failed: %A" e)

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
                driver.Navigate().GoToUrl(url)
                if isAvailable driver then
                    try
                        Debug.WriteLine("Switch is available. Trying to buy...")
                        driver.FindElementByCssSelector(".quick-note > a:nth-child(1)").Click()
                        driver.FindElementByCssSelector("input#mail").SendKeys(storeInfo.Credential.Id)
                        driver.FindElementByCssSelector("input#pw").SendKeys(storeInfo.Credential.Password)
                        driver.FindElementByCssSelector(".exbutton").Click()
                        driver.Navigate().GoToUrl(url)
                        driver.FindElementByCssSelector("li.quick > button.exbutton").Click()
                        if not dryRun then driver.FindElementByCssSelector(".exbutton").Click(); Environment.Exit(0)
                    with
                    | _ -> Debug.WriteLine("Exception occurred.")
                else
                    Debug.WriteLine("Switch is NOT available.")

let トイザらス = トイザらス(config.Stores.["トイザらス"], config.DryRun) :> 通販

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
                driver.Navigate().GoToUrl(url)
                if isAvailable driver then
                    driver.FindElementByCssSelector("#js_m_submitRelated").Click()
                    driver.FindElementByCssSelector(".buyButton a[href='/yc/shoppingcart/index.html?next=true']").Click()
                    driver.FindElementByCssSelector(".checkout a").Click()
                    driver.FindElementByCssSelector("input#memberId").SendKeys(storeInfo.Credential.Id)
                    driver.FindElementByCssSelector("input#password").SendKeys(storeInfo.Credential.Password)
                    driver.FindElementByCssSelector("#js_i_login0").Click()
                    driver.FindElementByCssSelector("#sc_i_buy").Click()
                    driver.FindElementByCssSelector("#a04").Click()
                    driver.FindElementByCssSelector(".js_c_next").Click()
                    if not dryRun then driver.FindElementByCssSelector("div.buyButton a").Click(); Environment.Exit(0)
                else
                    Debug.WriteLine("Switch is NOT available.")

let ヨドバシ = ヨドバシ(config.Stores.["ヨドバシ.com"], config.DryRun) :> 通販

let 通販TaskList =
    [
        for 通販 in [トイザらス; ヨドバシ] do
            let run () =
                use driver = new ChromeDriver()
                try
                    通販.Buy(driver)
                finally
                    driver.Quit()
            yield 通販Task(run)
    ]
    