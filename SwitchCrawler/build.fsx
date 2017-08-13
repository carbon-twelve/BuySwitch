#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open System.Diagnostics
open System.Threading

let buildDir = "./build/"

Target "Clean" (fun _ ->
    CleanDir buildDir
)

Target "BuildApp" (fun _ ->
    !! "*.fsproj"
    |> MSBuildRelease buildDir "Build"
    |> Log "AppBuild-Output: "
)

Target "Default" (fun _ ->
    trace "Hello World from FAKE"
)

Target "Run" (fun _ ->
    if isLinux then
        ignore(Async.StartAsTask(Shell.AsyncExec("Xvfb", ":1")))
        Thread.Sleep(1000)
        ignore(Shell.Exec("xhost", "+local:"))
        ignore(Shell.Exec("mono", buildDir + "SwitchCrawler.exe"))
    else if isWindows then
        ignore(Shell.Exec(buildDir + "SwitchCrawler.exe"))
)

"Clean" ==> "BuildApp" ==> "Default"

RunTarget ()
