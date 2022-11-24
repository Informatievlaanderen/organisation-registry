#!/usr/bin/env bash
set -e

dotnet tool restore
dotnet paket restore
chmod +x packages/Be.Vlaanderen.Basisregisters.Build.Pipeline/Content/*

FAKE_ALLOW_NO_DEPENDENCIES=true dotnet fake build -t "CleanAll"

if [ $# -eq 0 ]
then
  FAKE_ALLOW_NO_DEPENDENCIES=true dotnet fake build --parallel 8
else
  FAKE_ALLOW_NO_DEPENDENCIES=true dotnet fake build --parallel 8 -t "$@"
fi
