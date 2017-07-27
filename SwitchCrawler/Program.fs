open OpenQA.Selenium.Firefox
open System
open System.Text.RegularExpressions
open System.Globalization
open OpenQA.Selenium
open CoreTweet
open System.Diagnostics
open System.IO

let ``オムニセブンUrlList`` =
    [
        "http://iyec.omni7.jp/detail/4902370537338";
        "http://iyec.omni7.jp/detail/4902370535709";
        "http://iyec.omni7.jp/detail/4902370535716";
        "http://7net.omni7.jp/detail/2110599526";
        "http://7net.omni7.jp/detail/2110595636";
        "http://7net.omni7.jp/detail/2110595637";
    ]
let ``オムニセブン`` (driver: FirefoxDriver) =
    driver.FindElementByCssSelector("input.js-pressTwice").Enabled

let ``ヨドバシ.comUrlList`` =
    [
        "http://www.yodobashi.com/product/100000001003431565/";
        "http://www.yodobashi.com/product/100000001003431566/";
        "http://www.yodobashi.com/product/100000001003570628/";
    ]

let ``ヨドバシ.com`` (driver: FirefoxDriver) = 
    driver.FindElementByCssSelector("#js_buyBoxMain > ul:nth-child(1) > li:nth-child(1) > div:nth-child(1) > p:nth-child(1)").Text <> "予定数の販売を終了しました"        

let ``トイザらスUrlList`` =
    [
        "https://www.toysrus.co.jp/s/dsg-580782400";
        "https://www.toysrus.co.jp/s/dsg-572186500";
        "https://www.toysrus.co.jp/s/dsg-572182200";
    ]

let ``トイザらス`` (driver: FirefoxDriver) =
    driver.FindElementByCssSelector("#isStock_c").GetAttribute("hidden") <> null

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

let 通販List =
    [
        (オムニセブン, オムニセブンUrlList);
        (``ヨドバシ.com``, ``ヨドバシ.comUrlList``);
        (トイザらス, トイザらスUrlList);
        (ヤマダウェブコム, ヤマダウェブコムUrlList);
        //(``amazon.co.jp``, ``amazon.co.jpUrlList``);
        //(``amazon.co.jp2``, ``amazon.co.jp2UrlList``);
        (ノジマオンライン, ノジマオンラインUrlList);
        (楽天ブックス, 楽天ブックスUrlList);
        (あみあみ, あみあみUrlList);
        (TSUTAYA, TSUTAYAUrlList);
    ]

open FSharp.Data.JsonExtensions
open Chiron.Mapping
open Chiron.Functional
open Chiron

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
    
type Config = { Consumer: Consumer; AccessToken: AccessToken option } with
    static member ToJson (c: Config) = json {
        do! Json.write "consumer" c.Consumer
        do! Json.write "accessToken" c.AccessToken
    }
    static member FromJson (_: Config) = json {
        let! consumer = Json.read "consumer"
        let! accessToken = Json.read "accessToken"
        return { Consumer = consumer; AccessToken = accessToken }
    }

let getTwitterTokens () =
    let config: Config =
        File.ReadAllText("../../config.json")
        |> Json.parse
        |> Json.deserialize
    match config.AccessToken with
    | Some accessToken -> Tokens.Create(config.Consumer.Key, config.Consumer.Secret, accessToken.Token, accessToken.Secret)
    | None -> 
        let session = OAuth.Authorize(config.Consumer.Key, config.Consumer.Secret)
        Debug.WriteLine("Access here: {0}", session.AuthorizeUri)
        let pincode = Console.ReadLine()
        let tokens = OAuth.GetTokens(session, pincode)
        let newConfig = { config with AccessToken = Some { Token = tokens.AccessToken; Secret = tokens.AccessTokenSecret } }
        newConfig |> Json.serialize |> Json.format |> (fun content -> File.WriteAllText("../../config.json", content))
        tokens

let tweet (o: obj) =
    let rawStatus = sprintf "%A" o
    let status = rawStatus.Substring(0, Math.Min(139, rawStatus.Length))
    Debug.WriteLine("Tweeting: " + status)
    try
        let tokens = getTwitterTokens()
        tokens.Statuses.Update(status) |> ignore
    with
    | e -> Debug.WriteLine(sprintf "Tweet failed: %A" e)

[<EntryPoint>]
let main argv =
    use timer =
        new Threading.Timer(
            begin fun stateInfo ->
                use driver = new FirefoxDriver(FirefoxProfile())
                try
                    try
                        let found =
                            通販List
                            |> List.map (通販 driver)
                            |> List.filter (fun result -> result.IsSome)
                        if not (List.isEmpty found) then tweet found
                    with
                    | e -> tweet e
                finally
                    Debug.WriteLine("Loop completed")
                    driver.Quit()
            end,
            [],
            0,
            3 * 60 * 1000
        )
    0
