#!/bin/bash
#
folder="./.local"
cert_file="$folder/cert.pfx"
cert_password_file="$folder/cert_password.txt"

cert_password="YourGonnaBurnAlright123456"

# Create $folder folder if it doesn't exist
if [ ! -d "$folder" ]; then
  mkdir $folder
  echo "*" > $(echo "${folder}/.gitignore")
fi

# If password exists, read it from file
# Otherwise, write the generated password to file
if [ -f "$cert_password_file" ]; then
    cert_password=$(cat "$cert_password_file")
else
    echo "$cert_password" > $(echo "$cert_password_file")
fi

# Delete cert file if it exists
if [ -f "$cert_file" ]; then
    rm $cert_file
fi

dotnet dev-certs https -ep $(echo "${folder}/cert.pfx") -p $cert_password
dotnet dev-certs https --trust

