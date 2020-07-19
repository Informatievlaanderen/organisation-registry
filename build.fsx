#r "paket:
version 5.247.4
framework: netstandard20
source https://api.nuget.org/v3/index.json
nuget Be.Vlaanderen.Basisregisters.Build.Pipeline 4.2.2 //"

#load "packages/Be.Vlaanderen.Basisregisters.Build.Pipeline/Content/build-generic.fsx"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.JavaScript
open ``Build-generic``

let product = "Basisregisters Vlaanderen"
let copyright = "Copyright (c) Vlaamse overheid"
let company = "Vlaamse overheid"

let dockerRepository = "organisation-registry"
let assemblyVersionNumber = (sprintf "2.%s")
let nugetVersionNumber = (sprintf "%s")

let build = buildSolution assemblyVersionNumber
let setVersions = (setSolutionVersions assemblyVersionNumber product copyright company)
let test = testSolution
let publish = publish assemblyVersionNumber
let pack = pack nugetVersionNumber
let containerize = containerize dockerRepository
let push = push dockerRepository

supportedRuntimeIdentifiers <- [ "linux-x64" ]

Target.create "CleanAll" (fun _ ->
  Shell.cleanDir buildDir
  Shell.cleanDir ("src" @@ "OrganisationRegistry.UI" @@ "wwwroot")
)

// Solution -----------------------------------------------------------------------

Target.create "Restore_Solution" (fun _ -> restore "OrganisationRegistry")

Target.create "Build_Solution" (fun _ ->
  setVersions "SolutionInfo.cs"
  build "OrganisationRegistry")

Target.create "Site_Build" (fun _ ->
  Npm.exec "run build" id

  let dist = (buildDir @@ "OrganisationRegistry.UI" @@ "linux")
  let source = "src" @@ "OrganisationRegistry.UI"

  Shell.copyDir (dist @@ "wwwroot") (source @@ "wwwroot") (fun _ -> true)
  Shell.copyFile dist (source @@ "Dockerfile")
  Shell.copyFile dist (source @@ "default.conf")
  Shell.copyFile dist (source @@ "config.js")
  Shell.copyFile dist (source @@ "init.sh")
)

Target.create "Test_Solution" (fun _ -> test "OrganisationRegistry")

Target.create "Publish_Solution" (fun _ ->
  [
    "OrganisationRegistry.Api"
    "OrganisationRegistry.AgentschapZorgEnGezondheid.FtpDump"
    "OrganisationRegistry.VlaanderenBeNotifier"
    "OrganisationRegistry.ElasticSearch.Projections"
    "OrganisationRegistry.Projections.Delegations"
    "OrganisationRegistry.Projections.Reporting"
    "OrganisationRegistry.KboMutations"
  ] |> List.iter publish

  let dist = (buildDir @@ "OrganisationRegistry.Scheduler" @@ "linux")
  let source = "src" @@ "OrganisationRegistry.Scheduler"

  Shell.mkdir dist
  Shell.copyFile dist (source @@ "Dockerfile")
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

Target.create "Containerize_Site" (fun _ -> containerize "OrganisationRegistry.UI" "ui")
Target.create "PushContainer_Site" (fun _ -> push "ui")

Target.create "Containerize_Scheduler" (fun _ -> containerize "OrganisationRegistry.Scheduler" "scheduler")
Target.create "PushContainer_Scheduler" (fun _ -> push "scheduler")

// --------------------------------------------------------------------------------

Target.create "Build" ignore
Target.create "Test" ignore
Target.create "Publish" ignore
Target.create "Pack" ignore
Target.create "Containerize" ignore
Target.create "Push" ignore

"NpmInstall"
  ==> "DotNetCli"
  ==> "CleanAll"
  ==> "Restore_Solution"
  ==> "Build_Solution"
  ==> "Build"

"Build"
  ==> "Site_Build"
  ==> "Test_Solution"
  ==> "Test"

"Test"
  ==> "Publish_Solution"
  ==> "Clean_Solution"
  ==> "Publish"

"Publish"
  ==> "Pack_Solution"
  ==> "Pack"

"Pack"
  ==> "Containerize_Api"
  ==> "Containerize_AgentschapZorgEnGezondheid"
  ==> "Containerize_VlaanderenBeNotifier"
  ==> "Containerize_ElasticSearch"
  ==> "Containerize_Delegations"
  ==> "Containerize_Reporting"
  ==> "Containerize_KboMutations"
  ==> "Containerize_Site"
  ==> "Containerize_Scheduler"
  ==> "Containerize"
// Possibly add more projects to containerize here

"Containerize"
  ==> "DockerLogin"
  ==> "PushContainer_Api"
  ==> "PushContainer_AgentschapZorgEnGezondheid"
  ==> "PushContainer_VlaanderenBeNotifier"
  ==> "PushContainer_ElasticSearch"
  ==> "PushContainer_Delegations"
  ==> "PushContainer_Reporting"
  ==> "PushContainer_KboMutations"
  ==> "PushContainer_Site"
  ==> "PushContainer_Scheduler"
  ==> "Push"
// Possibly add more projects to push here

// By default we build & test
Target.runOrDefault "Test"
