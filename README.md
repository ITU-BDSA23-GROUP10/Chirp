# Chirp
This is our twitter clone for the BDSA project

# A publicly hosted version of this application can be found on Azure
You can access it by using this link:
https://bdsagroup10chirprazor.azurewebsites.net/

# How to run
## Run using VS code dev containers
This is by far the easiest option. First ensure you have the docker and dev container extension installed on your VS code installation. Afterwards you may get a small popup in the bottom right corner telling you that you can open the project as a dev container. Press that button. If this is not the case open the VS code command pallete (Ctrl + shift + p or cmd + shift + p) and dev containers and select either "Rebuild and reopen in Container" or "Reopen in a Container" 

After VS code is done opening the container you should be able to go to the left hand sidebar in VS code and select the run and debug option and press the small green play button.

## For all of the below you must have a dockerized container with Microsoft SQLServer running in the background. This can be done by downloading docker and using the following in a terminal (remember to set a password):
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=yourStrong(!)Password" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

You can check if the docker container is running by using this in a terminal. Look for wether it shows a container running:
docker ps

If the container is not running it may because the password is too weak so try to run the above command again with a different password

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
Unzip the folder using your unzipper of choice then enter the directory using
cd ./Chirp-vX.X.X-osx-x64
Then you have to allow the executable to run by running this command in your terminal in the unzipped directory:
chmod +x ./Chirp.Web
Since the app is unsigned you have to strip the xattr attribute from it using this command
xattr -dr com.apple.quarantine "Chirp.Web"
(Read more about this here https://apple.stackexchange.com/questions/202169/how-can-i-open-an-app-from-an-unidentified-developer-without-using-the-gui)
Now you can run the application using:
./Chirp.Web

### Windows Example:
*Coming soon*

## Run from source code
To run the program from source you can build it using the source code on the repo and use dotnet run to run the program locally on your machine. This should be done from the ./Chirp/src/Chirp.Web directory 

If you would like to run the tests you can do so from the ./Chirp directory using dotnet test or you can run some of the tests from their respective test directories

# Legacy versions of the Chirp app
There are currently two major versions of the chirp application. There is the current Chirp Razor version of the app which can be found in the majority of the latest releases and then the old Chirp CLI version which starts from version 1.0.0 and going back. The source code for Chirp CLI can be found on the Chirp_CLI branch inside the github repo.
