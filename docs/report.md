---
title: _Chirp!_ Project Report
subtitle: ITU BDSA 2023 Group `10`
author:
- "Theis Per Holm <thph@itu.dk>"
# Add your names here guys
numbersections: true
---

# Design and Architecture of _Chirp!_

## Domain model

Here comes a description of our domain model.

![Illustration of the _Chirp!_ data model as UML class diagram.](docs/images/domain_model.png)

## Architecture — In the small

## Architecture of deployed application

## User activities

## Sequence of functionality/calls trough _Chirp!_

# Process

## Build, test, release, and deployment  
The group employed the use of Github Workflows/Actions to build, test, release and deploy the app to Azure. The UML activity diagrams below show how each of the Workflows work using UML activity and UML sequence diagrams. 

Via the use of these workflows the group thusly ensured that the program was always passing the tests when the anything got pushed to the main branch. More importantly it secured the main branch when trying to merge branches into main via Github Pull Requests.   They also ensured that the release build of the program was able to be built and passed most tests before getting put into a release build thus minimizing the chances that a release would have a broken program. 

Deployment was done on Microsoft Azure using their webapp and database features.


## Team work

## How to make _Chirp!_ work locally
For a full guide on how to run the project locally see the ReadMe.md on the public repository (This includes many other ways of running the program and more explanations): [Chirp ReadMe.md](https://github.com/ITU-BDSA23-GROUP10/Chirp/blob/main/README.md) 

This part will include the ways to run it locally fully using docker or running it locally with dotnet run / the release executables 

### How to Run the project using dotnet run and a MSQL docker image:
If you haven't got docker please follow the guide on this website for more information on how to do so [Install guide for Docker](https://docs.docker.com/get-docker/) //TODO: visited 19/12

First you will have to install a docker image of MSQL server this can be done running this in a terminal (Remember to set the password and the password has to be a strong password otherwise the server wont run)

```
$ docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=yourStrong(!)Password" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

After running the above you can check if the container is running 

``` 
$ docker ps
```

*If the container is not showing up on the list please refer to the ReadMe on possible ways to solve this issue*

After ensuring that the Docker container is indeed running you can create a user secret with the connection string to your container from the Chirp directory (please ensure you have the dotnet user-secrets tool [Nuget Page For Dotnet user-secrets tool install](https://www.nuget.org/packages/dotnet-user-secrets)):
```
$ cd ./src/Chirp.Web && dotnet user-secret init
```
Now you can set the user secrets
```
dotnet user-secrets set "ConnectionStrings:ConnectionString" "Data Source=localhost,1433;Initial Catalog=Chirp;User ID=sa;Password=yourStrong(!)Password;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
```
After this you can run the program using dotnet run from the Chirp.Web directory or the release latest release executable from the github repository (an OS specific guide on how to do this can be found in the full ReadMe): [Releases]()
```
$ dotnet run
```

### Running the program only using docker
The program has a Dockerfile as a well as a docker-compose file which will run the program and database fully within docker containers. To do so just run the following docker compose command in your terminal in the Chirp directory
```
$ docker compose up
```

## How to run test suite locally
For the playwright tests to work you will need to have installed docker refer to the run section for instructions on this. 

### Running all tests (Except playwright)
To run all tests except the playwright tests simply run this command in your terminal in the Chirp directory
```
$ dotnet test
```

### To run the playwright tests
To run the playwright tests its a bit of process. The program must be running the background for the playwright tests to function as it tries to access the application. Running the program through the docker setup will not work for this as it seems to have some problems with the certificates as well as the browsers. So please refer to how to run the program locally using either the release build or dotnet run. You can then run the playwright tests by running the following command in your terminal inside the Chirp/test/PlaywrightTests directory
```
$ dotnet test
```

# Ethics

## License  
The group has chosen the MIT open source software license. You can read a small summary from Github's license page below as well as see a very simple overview of the ... that this license provides


>MIT License
>
>Copyright (c) [year] [fullname]
>
>Permission is hereby granted, free of charge, to any person obtaining a copy
>of this software and associated documentation files (the "Software"), to deal
>in the Software without restriction, including without limitation the rights
>to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
>copies of the Software, and to permit persons to whom the Software is
>furnished to do so, subject to the following conditions:
>
>The above copyright notice and this permission notice shall be included in all
>copies or substantial portions of the Software.
>
>THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
>IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
>FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
>AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
>LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
>OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
>SOFTWARE.  

//TODO: figure out how we want to source from things
Source: [Github choose a license site](https://choosealicense.com/licenses/mit/)

The group chose this license as it was a good fit for the groups requirements of an open source license in that it basically has no restrictions for any end user or somebody who wants to work with the codebase. We also sign off any warranty or liability which is great for a small group project that we more than likely wont want to take further in the future.

## LLMs, ChatGPT, CoPilot, and others
The use of LLMs like ChatGPT and Copilot has been documented on github commits as a co-author when used. You can see the number of these commits on the page linked here: [ChatGPT Co-authored commits](https://github.com/ITU-BDSA23-GROUP10/Chirp/graphs/contributors). Sadly the page that shows the actual commits doesn't have the commits that it contributed on as these were done on separate branches whose commits seem to not carry over to the main branch's working tree. 


# Appendix


# 