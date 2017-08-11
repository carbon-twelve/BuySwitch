#r @"packages/FAKE/tools/FakeLib.dll"
open Fake

let buildDir = "./build/"
let configDir = "./config/"
let driverDir = "./drivers/"
Target "Clean" (fun _ ->
    CleanDir buildDir
)

Target "BuildApp" (fun _ ->
    !! "*.fsproj"
    |> MSBuildRelease buildDir "Build"
    |> Log "AppBuild-Output: "
)

Target "CopyConfigs" (fun _ ->
    ["config.json"; "NLog.config"]
    |> List.map (fun file -> configDir + file)
    |> Copy buildDir
)

Target "CopySeleniumDrivers" (fun _ ->
    !! (driverDir + "*")
    |> Copy buildDir
)

Target "Default" (fun _ ->
    trace "Hello World from FAKE"
)

"Clean" ==> "BuildApp" ==> "CopyConfigs" ==> "CopySeleniumDrivers" ==> "Default"

RunTargetOrDefault "Default"
