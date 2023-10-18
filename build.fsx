#r "paket:
version 7.0.2
framework: net6.0
source https://api.nuget.org/v3/index.json
nuget Be.Vlaanderen.Basisregisters.Build.Pipeline 6.0.5 //"

#load "packages/Be.Vlaanderen.Basisregisters.Build.Pipeline/Content/build-generic.fsx"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.JavaScript
open Fake.DotNet
open ``Build-generic``
open System
open System.IO


let product = "Basisregisters Vlaanderen"
let copyright = "Copyright (c) Vlaamse overheid"
let company = "Vlaamse overheid"

let dockerRepository = "organisation-registry"
let assemblyVersionNumber = (sprintf "2.%s")
let nugetVersionNumber = (sprintf "%s")

let buildSolution = buildSolution assemblyVersionNumber
let buildSource = build assemblyVersionNumber
let buildTest = buildTest assemblyVersionNumber
let setVersions = (setSolutionVersions assemblyVersionNumber product copyright company)
let publishSource = publish assemblyVersionNumber
let pack = pack nugetVersionNumber
let containerize = containerize dockerRepository
let push = push dockerRepository

supportedRuntimeIdentifiers <- [ "linux-x64" ]

let testWithDotNet path =
  let fxVersion = getDotNetClrVersionFromGlobalJson()

  let cmd = sprintf "test --no-build --no-restore --logger trx --configuration Release --no-build --no-restore /p:RuntimeFrameworkVersion=%s --dcReportType=HTML" fxVersion

  let result = DotNet.exec (id) "dotcover" cmd
  if result.ExitCode <> 0 then failwith "Test Failure"

let test project =
  testWithDotNet ("test" @@ project @@ (sprintf "%s.csproj" project))

let testSolution sln =
  testWithDotNet (sprintf "%s.sln" sln)

Target.create "CleanAll" (fun _ ->
  Shell.cleanDir buildDir
  Shell.cleanDir ("src" @@ "OrganisationRegistry.UI" @@ "wwwroot")
)

// Solution -----------------------------------------------------------------------

Target.create "SetSolutionInfo" (fun _ ->
  setVersions "SolutionInfo.cs"
)

Target.create "Restore_Solution" (fun _ -> restore "OrganisationRegistry")

Target.create "Build_Solution" (fun _ ->
  setVersions "SolutionInfo.cs"
  buildSolution "OrganisationRegistry")

Target.create "Build_AcmIdm" (fun _ ->
  setVersions "SolutionInfo.cs"
  buildSolution "src/IdentityServer/IdentityServer")

Target.create "Site_Build" (fun _ ->
  Npm.exec "run build" id

  let dist = (buildDir @@ "OrganisationRegistry.UI" @@ "linux")
  let source = "src" @@ "OrganisationRegistry.UI"

  Shell.copyDir (dist @@ "wwwroot") (source @@ "wwwroot") (fun _ -> true)
  Shell.copyFile dist (source @@ "Dockerfile")
  Shell.copyFile dist (source @@ "default.conf")
  Shell.copyFile dist (source @@ "config.js")
  Shell.copyFile dist (source @@ "init.sh")

  ()
)

Target.create "Vue_Build" (fun _ ->
  let dist = (buildDir @@ "OrganisationRegistry.UI" @@ "linux")
  let source = "src" @@ "OrganisationRegistry.UI"

  Npm.install (fun o -> { o with WorkingDirectory = "src" @@ "OrganisationRegistry.Vue2" })

  Npm.exec "run build"  (fun o -> { o with WorkingDirectory = "src" @@ "OrganisationRegistry.Vue2" })

  let vueDist = ("src" @@ "OrganisationRegistry.Vue2" @@ "dist")

  Shell.mkdir (dist @@ "wwwroot" @@ "vue")
  Shell.copyDir (dist @@ "wwwroot" @@ "vue") (vueDist) (fun _ -> true)

  ()
)

Target.create "Test_Solution" (fun _ -> testSolution "OrganisationRegistry")

Target.create "Publish_Solution" (fun _ ->
  [
    "OrganisationRegistry.Api"
    "OrganisationRegistry.AgentschapZorgEnGezondheid.FtpDump"
    "OrganisationRegistry.VlaanderenBeNotifier"
    "OrganisationRegistry.ElasticSearch.Projections"
    "OrganisationRegistry.Projections.Delegations"
    "OrganisationRegistry.Projections.Reporting"
    "OrganisationRegistry.KboMutations"
    "OrganisationRegistry.Rebuilder"
  ] |> Seq.toArray
    |> Array.Parallel.iter publishSource
)

Target.create "Publish_Api" (fun _ ->
  publishSource "OrganisationRegistry.Api"
)
Target.create "Publish_AgentschapZorgEnGezondheid" (fun _ ->
  publishSource "OrganisationRegistry.AgentschapZorgEnGezondheid.FtpDump"
)
Target.create "Publish_VlaanderenBeNotifier" (fun _ ->
  publishSource "OrganisationRegistry.VlaanderenBeNotifier"
)
Target.create "Publish_ElasticSearch" (fun _ ->
  publishSource "OrganisationRegistry.ElasticSearch.Projections"
)
Target.create "Publish_Delegations" (fun _ ->
  publishSource "OrganisationRegistry.Projections.Delegations"
)
Target.create "Publish_OrganisationRegistry.Reporting" (fun _ ->
  publishSource "OrganisationRegistry.Projections.Reporting"
)
Target.create "Publish_KboMutions" (fun _ ->
  publishSource "OrganisationRegistry.KboMutations"
)
Target.create "Publish_Rebuilder" (fun _ ->
  publishSource "OrganisationRegistry.Rebuilder"
)

Target.create "Clean_Solution" (fun _ ->
  !! "src/**/obj/*" |> File.deleteAll
  !! "src/**/bin/*" |> File.deleteAll
  !! "packages/**/*" -- "packages/*Basisregisters*/**/*" |> File.deleteAll
)

Target.create "Pack_Solution" (fun _ ->
  [
    "OrganisationRegistry.Api"
  ] |> List.iter pack)

Target.create "Containerize_Api" (fun _ -> containerize "OrganisationRegistry.Api" "api")
Target.create "PushContainer_Api" (fun _ -> push "api")

Target.create "Containerize_AgentschapZorgEnGezondheid" (fun _ -> containerize "OrganisationRegistry.AgentschapZorgEnGezondheid.FtpDump" "batch-agentschapzorgengezondheidftpdump")
Target.create "PushContainer_AgentschapZorgEnGezondheid" (fun _ -> push "batch-agentschapzorgengezondheidftpdump")

Target.create "Containerize_VlaanderenBeNotifier" (fun _ -> containerize "OrganisationRegistry.VlaanderenBeNotifier" "batch-vlaanderenbe")
Target.create "PushContainer_VlaanderenBeNotifier" (fun _ -> push "batch-vlaanderenbe")

Target.create "Containerize_ElasticSearch" (fun _ -> containerize "OrganisationRegistry.ElasticSearch.Projections" "projections-elasticsearch")
Target.create "PushContainer_ElasticSearch" (fun _ -> push "projections-elasticsearch")

Target.create "Containerize_Delegations" (fun _ -> containerize "OrganisationRegistry.Projections.Delegations" "projections-delegations")
Target.create "PushContainer_Delegations" (fun _ -> push "projections-delegations")

Target.create "Containerize_Reporting" (fun _ -> containerize "OrganisationRegistry.Projections.Reporting" "projections-reporting")
Target.create "PushContainer_Reporting" (fun _ -> push "projections-reporting")

Target.create "Containerize_KboMutations" (fun _ -> containerize "OrganisationRegistry.KboMutations" "kbo-mutations")
Target.create "PushContainer_KboMutations" (fun _ -> push "kbo-mutations")

Target.create "Containerize_Rebuilder" (fun _ -> containerize "OrganisationRegistry.Rebuilder" "rebuilder")
Target.create "PushContainer_Rebuilder" (fun _ -> push "rebuilder")

Target.create "Containerize_Site" (fun _ -> containerize "OrganisationRegistry.UI" "ui")
Target.create "PushContainer_Site" (fun _ -> push "ui")

let containerizeAcmIdm path containerName =
  let result1 =
    [ "build"; "."; "--no-cache"; "--tag"; sprintf "%s/%s/%s:%s" dockerRegistry dockerRepository containerName buildNumber; "--build-arg"; sprintf "build_number=%s" buildNumber]
    |> CreateProcess.fromRawCommand "docker"
    |> CreateProcess.withWorkingDirectory (path)
    |> CreateProcess.withTimeout (TimeSpan.FromMinutes 5.)
    |> Proc.run

  if result1.ExitCode <> 0 then failwith "Failed result from Docker Build"

  let result2 =
    [ "tag"; sprintf "%s/%s/%s:%s" dockerRegistry dockerRepository containerName buildNumber; sprintf "%s/%s/%s:latest" dockerRegistry dockerRepository containerName]
    |> CreateProcess.fromRawCommand "docker"
    |> CreateProcess.withTimeout (TimeSpan.FromMinutes 5.)
    |> Proc.run

  if result2.ExitCode <> 0 then failwith "Failed result from Docker Tag"

Target.create "Containerize_AcmIdm" (fun _ -> containerizeAcmIdm "src/IdentityServer" "acmidm")
Target.create "PushContainer_AcmIdm" (fun _ -> push "acmidm")


// --------------------------------------------------------------------------------

Target.create "Default" ignore
Target.create "Publish" ignore
Target.create "Pack" ignore
Target.create "Containerize" ignore
Target.create "Push" ignore

"Restore_Solution"
  ==> "Build_Solution"
  ==> "Default"

"Restore_Solution"
  ==> "Build_Solution"
  ==> "Test_Solution"

"Site_Build"
  ==> "Default"

"Vue_Build"
  ==> "Default"

"Restore_Solution"
  ==> "Build_AcmIdm"
  ==> "Containerize_AcmIdm"

"Default"
  ==> "Publish_Solution"
  ==> "Clean_Solution"
  ==> "Publish"

"Publish"
  ==> "Pack_Solution"
  ==> "Pack"

"Site_Build"
  ==> "Vue_Build"
  ==> "Containerize_Site"


"Containerize"
  ==> "DockerLogin"
  ==> "PushContainer_Api"
  ==> "PushContainer_AgentschapZorgEnGezondheid"
  ==> "PushContainer_VlaanderenBeNotifier"
  ==> "PushContainer_ElasticSearch"
  ==> "PushContainer_Delegations"
  ==> "PushContainer_Reporting"
  ==> "PushContainer_KboMutations"
  ==> "PushContainer_Rebuilder"
  ==> "PushContainer_Site"
  ==> "PushContainer_AcmIdm"
  ==> "Push"

Target.create "Containerize_All" ignore

// By default we build & test
Target.runOrDefault "Default"
