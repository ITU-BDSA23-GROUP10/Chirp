FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
#/home/slug/Documents/GitRepos/BDSA_repo/Chirp/.devcontainer
RUN mkdir Chirp
RUN mkdir app
WORKDIR /Chirp

# Adds the source code to the container
ADD src ./

# Restores the dependencies, builds the project and deletes the source code
RUN apt-get install -y sed
RUN cd Chirp.Web && sed -ie '/^{/a "ConnectionStrings": { "ConnectionString": "Server=db,1433;Initial Catalog=master;User ID=sa;Password=YourGonnaBurnAlright1234;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" },' appsettings.json
#RUN cd Chirp.Web &&  dotnet dev-certs https --clean && dotnet dev-certs https --trust
RUN cd Chirp.Web && dotnet restore 
RUN cd Chirp.Web && dotnet build -c Release -o /app
RUN rm -rf src
#RUN dotnet tool install --global user-secrets &&

# Sets the working directory to the build output folder and runs the app

FROM ubuntu:22.04
#FROM mcr.microsoft.com/dotnet/aspnet:7.0
#FROM mcr.microsoft.com/dotnet/sdk:7.0
WORKDIR /app/build



COPY --from=build-env /app .
RUN apt-get update && apt-get install -y dotnet-sdk-7.0 && apt-get install -y aspnetcore-runtime-7.0
RUN dotnet dev-certs https --clean && dotnet dev-certs https --trust
#RUN cd .. && dotnet dev-certs https --clean && dotnet dev-certs https --trust
ENTRYPOINT ["dotnet", "Chirp.Web.dll"]

# Exposes the port
EXPOSE 5273/tcp
#EXPOSE 1433/tcp
