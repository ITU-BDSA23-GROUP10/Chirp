#!/bin/bash
# This file was partly inspired by what Albert Rise Nielsen : https://github.com/albe2669 had shown me with their own script
# This script is used to create a certificate for the server
pss=$1

mkdir ./.local

cd ./.local

# Removes the .gitignore file if it exists
if test -f .gitignore 
then
    rm .gitignore
fi

# Creates a .gitignore file
touch .gitignore
echo "*" >> .gitignore

# Creates a password file
if test -f ./cert_password.txt 
then
    rm ./cert_password.txt
fi

# Inspired by this: https://stackoverflow.com/questions/3601515/how-to-check-if-a-variable-is-set-in-bash
touch ./cer_password.txt
if [ -z "$pss+x" ]
then
    echo "$pss" >> ./cer_password.txt
else
    echo "YourGonnaBurnAlright123456" >> ./cer_password.txt
fi

# Removes the cert.pfx file if it exists
if test -f ./cert.pfx 
then
    rm ./cert.pfx
fi

# ~Creates a dev certificate~

# Cleans the current dev certificate
dotnet dev-certs https --clean

# Creates a new dev certificate
dotnet dev-certs https -ep ./cert.pfx -p YourGonnaBurnAlright123456

# Trusts the certificate
dotnet dev-certs https --trust
