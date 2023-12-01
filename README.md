# Chirp
This is our twitter clone for the BDSA project

# A publicly hosted version of this application can be found on Azure
You can access it by using this link:
https://bdsagroup10chirprazor.azurewebsites.net/

# How to run
## Run from release
You can also download the latest release build for your platform and use the included executables (e.g Chirp-vX.X.X-linux-x64 for linux based systems). You can run these through your terminal or CommandLine tools depending on your OS or by double clicking the executables in the folder if your OS allows for that.

### Linux Example:
First you must unzip the directory using your unzipper of choice and go into that directory using
cd Chirp-vX.X.X-linux-x64
Then you have to allow the executable to run by running this command in your terminal in the unzipped directory:
chmod +x ./Chirp.Web
then you can run the program using:
./Chirp.Web

### MacOS Example:
*Coming soon*

### Windows Example:
*Coming soon*

## Run from source code
To run the program from source you can build it using the source code on the repo and use dotnet run to run the program locally on your machine. This should be done from the ./Chirp/src/Chirp.Web directory 

If you would like to run the tests you can do so from the ./Chirp directory using dotnet test or you can run some of the tests from their respective test directories

# Legacy versions of the Chirp app
There are currently two major versions of the chirp application. There is the current Chirp Razor version of the app which can be found in the majority of the latest releases and then the old Chirp CLI version which starts from version 1.0.0 and going back. The source code for Chirp CLI can be found on the Chirp_CLI branch inside the github repo.
