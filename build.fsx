#r "paket:
version 5.241.6
framework: netstandard20
source https://api.nuget.org/v3/index.json
nuget Be.Vlaanderen.Basisregisters.Build.Pipeline 4.0.6 //"

#load "packages/Be.Vlaanderen.Basisregisters.Build.Pipeline/Content/build-generic.fsx"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.JavaScript
open ``Build-generic``

// The buildserver passes in `BITBUCKET_BUILD_NUMBER` as an integer to version the results
// and `BUILD_DOCKER_REGISTRY` to point to a Docker registry to push the resulting Docker images.

// NpmInstall
// Run an `npm install` to setup Commitizen and Semantic Release.

// DotNetCli
// Checks if the requested .NET Core SDK and runtime version defined in global.json are available.
// We are pedantic about these being the exact versions to have identical builds everywhere.

// Clean
// Make sure we have a clean build directory to start with.

// Restore
// Restore dependencies for debian.8-x64 and win10-x64 using dotnet restore and Paket.

// Build
// Builds the solution in Release mode with the .NET Core SDK and runtime specified in global.json
// It builds it platform-neutral, debian.8-x64 and win10-x64 version.

// Test
// Runs `dotnet test` against the test projects.

// Publish
// Runs a `dotnet publish` for the debian.8-x64 and win10-x64 version as a self-contained application.
// It does this using the Release configuration.

// Pack
// Packs the solution using Paket in Release mode and places the result in the dist folder.
// This is usually used to build documentation NuGet packages.

// Containerize
// Executes a `docker build` to package the application as a docker image. It does not use a Docker cache.
// The result is tagged as latest and with the current version number.

// DockerLogin
// Executes `ci-docker-login.sh`, which does an aws ecr login to login to Amazon Elastic Container Registry.
// This uses the local aws settings, make sure they are working!

// Push
// Executes `docker push` to push the built images to the registry.

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
  Npm.exec "build" id

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
 // ==> "DotNetCli"
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
