# This file was partly inspired by what Albert Rise Nielsen : https://github.com/albe2669 had shown me with their own script
# This script is used to create a certificate for the server
# The powershell version of this script was translated from the bash script by copilot chat <3
$pss = $args[0]

New-Item -ItemType Directory -Force -Path .\.local

Set-Location .\.local



# Removes the .gitignore file if it exists
if (Test-Path .gitignore) 
{
    Remove-Item .gitignore
}

# Creates a .gitignore file
New-Item .gitignore -type file
Add-Content .gitignore "*"

# Creates a password file
if (Test-Path .\cert_password.txt) 
{
    Remove-Item .\cert_password.txt
}

# Inspired by this: https://stackoverflow.com/questions/3601515/how-to-check-if-a-variable-is-set-in-bash
New-Item .\cert_password.txt -type file
if ([string]::IsNullOrEmpty($pss))
{
    Add-Content .\cert_password.txt "YourGonnaBurnAlright123456"
}
else
{
    Add-Content .\cert_password.txt $pss
}

# Removes the cert.pfx file if it exists
if (Test-Path .\cert.pfx) 
{
    Remove-Item .\cert.pfx
}

# ~Creates a dev certificate~

# Cleans the current dev certificate
dotnet dev-certs https --clean

# Creates a new dev certificate
dotnet dev-certs https -ep .\cert.pfx -p YourGonnaBurnAlright123456

# Trusts the certificate
dotnet dev-certs https --trust