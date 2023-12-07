# This file is a mess it basically creates 2 containers one that builds and one that runs the app. The first container was mostly made from: 
#https://softchris.github.io/pages/dotnet-dockerize.html#create-a-dockerfile

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

RUN mkdir Chirp
RUN mkdir app
WORKDIR /Chirp

# Adds the source code to the container
ADD src ./
# installs sed and adds the connection string to the appsettings.json file after the first {
RUN apt-get install -y sed
RUN cd Chirp.Web && sed -ie '/^{/a "ConnectionStrings": { "ConnectionString": "Server=db,1433;Initial Catalog=master;User ID=sa;Password=YourGonnaBurnAlright1234;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" },' appsettings.json

# Restores the dependencies, builds the project and deletes the source code
RUN cd Chirp.Web && dotnet restore 
RUN cd Chirp.Web && dotnet build -c Release -o /app

RUN cd Chirp.Web && cp -r wwwroot /app
RUN rm -rf src


# This container was made from the same link as above plus my own knowledge
FROM build-env
WORKDIR /app/build


COPY --from=build-env /app .

ENTRYPOINT ["dotnet", "Chirp.Web.dll"]

EXPOSE 5273/tcp

