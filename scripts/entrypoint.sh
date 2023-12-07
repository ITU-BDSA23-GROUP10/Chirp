#!/bin/bash

set -e

# Ripped from https://medium.com/@adrian.gheorghe.dev/using-docker-secrets-in-your-environment-variables-7a0609659aab 
file_env() {
   local var="$1"
   local fileVar="${var}_FILE"

   # Resolve file from variable 
   local file=${!fileVar:-}

   export "$var"="$(cat $file)"
   unset "$fileVar"
}

file_env "ASPNETCORE_Kestrel__Certificates__Default__Password"

exec dotnet Chirp.Web.dll
