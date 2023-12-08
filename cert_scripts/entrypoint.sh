#!/bin/bash

cd /https

filepath = "./cert_password.txt"

if test -f ./cert_password.txt 
then
    $pass = cat $filepath
else 
    $pass = "YourGonnaBurnAlright123456"
fi

cd ..

env =$pass

exec dotnet Chirp.Web.dll

