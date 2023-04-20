#!/bin/bash

exdir=$(dirname `readlink -f "$0"`)

cd "$exdir"/shu
rm -fr bin
dotnet pack -c Release

# workaround (https://github.com/devel0/knowledge/blob/a41ed1972ce4831b3c9a8c708003f9c89e0b8f25/doc/dotnet-troubleshoot.md#datetimeoffset-packing-error)
# find -print | while read filename; do touch -d "now" "$filename"; done
# dotnet pack -c Release
#

#dotnet nuget push bin/Release/*.nupkg -k $(cat ~/security/nuget-api.key) -s https://api.nuget.org/v3/index.json

cd "$exdir"
