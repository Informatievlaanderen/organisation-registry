#load "packages/Be.Vlaanderen.Basisregisters.Build.Pipeline/Content/build-generic.fsx"

open Fake
open Fake.NpmHelper
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

Target "CleanAll" (fun _ ->
  CleanDir buildDir
  CleanDir ("src" @@ "OrganisationRegistry.UI" @@ "wwwroot")
)

// Solution -----------------------------------------------------------------------

Target "Restore_Solution" (fun _ -> restore "OrganisationRegistry")

Target "Build_Solution" (fun _ ->
  setVersions "SolutionInfo.cs"
  build "OrganisationRegistry")

Target "Site_Build" (fun _ ->
  Npm (fun p ->
    { p with
        Command = (Run "build")
    })

  let dist = (buildDir @@ "OrganisationRegistry.UI" @@ "linux")
  let source = "src" @@ "OrganisationRegistry.UI"

  CopyDir (dist @@ "wwwroot") (source @@ "wwwroot") (fun _ -> true)
  CopyFile dist (source @@ "Dockerfile")
  CopyFile dist (source @@ "default.conf")
  CopyFile dist (source @@ "config.js")
  CopyFile dist (source @@ "init.sh")
)

Target "Test_Solution" (fun _ -> test "OrganisationRegistry")

Target "Publish_Solution" (fun _ ->
  [
    "OrganisationRegistry.Api"
    "OrganisationRegistry.AgentschapZorgEnGezondheid.FtpDump"
    "OrganisationRegistry.VlaanderenBeNotifier"
    "OrganisationRegistry.ElasticSearch.Projections"
    "OrganisationRegistry.Projections.Delegations"
    "OrganisationRegistry.Projections.Reporting"
  ] |> List.iter publish)

Target "Pack_Solution" (fun _ ->
  [
    "OrganisationRegistry.Api"
  ] |> List.iter pack)

Target "Containerize_Api" (fun _ -> containerize "OrganisationRegistry.Api" "api")
Target "PushContainer_Api" (fun _ -> push "api")

Target "Containerize_AgentschapZorgEnGezondheid" (fun _ -> containerize "OrganisationRegistry.AgentschapZorgEnGezondheid.FtpDump" "batch-agentschapzorgengezondheidftpdump")
Target "PushContainer_AgentschapZorgEnGezondheid" (fun _ -> push "batch-agentschapzorgengezondheidftpdump")

Target "Containerize_VlaanderenBeNotifier" (fun _ -> containerize "OrganisationRegistry.VlaanderenBeNotifier" "batch-vlaanderenbe")
Target "PushContainer_VlaanderenBeNotifier" (fun _ -> push "batch-vlaanderenbe")

Target "Containerize_ElasticSearch" (fun _ -> containerize "OrganisationRegistry.ElasticSearch.Projections" "projections-elasticsearch")
Target "PushContainer_ElasticSearch" (fun _ -> push "projections-elasticsearch")

Target "Containerize_Delegations" (fun _ -> containerize "OrganisationRegistry.Projections.Delegations" "projections-delegations")
Target "PushContainer_Delegations" (fun _ -> push "projections-delegations")

Target "Containerize_Reporting" (fun _ -> containerize "OrganisationRegistry.Projections.Reporting" "projections-reporting")
Target "PushContainer_Reporting" (fun _ -> push "projections-reporting")

Target "Containerize_Site" (fun _ -> containerize "OrganisationRegistry.UI" "ui")
Target "PushContainer_Site" (fun _ -> push "ui")

// --------------------------------------------------------------------------------

Target "Build" DoNothing
Target "Test" DoNothing
Target "Publish" DoNothing
Target "Pack" DoNothing
Target "Containerize" DoNothing
Target "Push" DoNothing

"NpmInstall"         ==> "Build"
"DotNetCli"          ==> "Build"
"CleanAll"           ==> "Build"
"Restore_Solution"   ==> "Build"
"Build_Solution"     ==> "Build"

"Build"              ==> "Test"
"Site_Build"         ==> "Test"
// "Test_Solution"      ==> "Test"

"Test"               ==> "Publish"
"Publish_Solution"   ==> "Publish"

"Publish"            ==> "Pack"
"Pack_Solution"      ==> "Pack"

"Pack"                                    ==> "Containerize"
"Containerize_Api"                        ==> "Containerize"
"Containerize_AgentschapZorgEnGezondheid" ==> "Containerize"
// "Containerize_VlaanderenBeNotifier"       ==> "Containerize"
// "Containerize_ElasticSearch"              ==> "Containerize"
// "Containerize_Delegations"                ==> "Containerize"
// "Containerize_Reporting"                  ==> "Containerize"
"Containerize_Site"                       ==> "Containerize"
// Possibly add more projects to containerize here

"Containerize"                             ==> "Push"
"DockerLogin"                              ==> "Push"
"PushContainer_Api"                        ==> "Push"
"PushContainer_AgentschapZorgEnGezondheid" ==> "Push"
// "PushContainer_VlaanderenBeNotifier"       ==> "Push"
// "PushContainer_ElasticSearch"              ==> "Push"
// "PushContainer_Delegations"                ==> "Push"
// "PushContainer_Reporting"                  ==> "Push"
"PushContainer_Site"                       ==> "Push"
// Possibly add more projects to push here

// By default we build & test
RunTargetOrDefault "Test"
