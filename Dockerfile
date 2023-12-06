FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
#/home/slug/Documents/GitRepos/BDSA_repo/Chirp/.devcontainer
RUN mkdir Chirp
RUN mkdir app
WORKDIR /Chirp

# Adds the source code to the container
ADD src ./

# Restores the dependencies, builds the project and deletes the source code
RUN apt-get install -y sed
RUN cd Chirp.Web && sed -ie '/^{/a "ConnectionStrings": { "ConnectionString": "Server=127.0.0.1,1433;Initial Catalog=master;User ID=sa;Password=P@ssw0rd;Connect Timeout=30;Encrypt=False;TrustServerCertificate=true;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" },' appsettings.json
RUN cd Chirp.Web && dotnet restore 
RUN cd Chirp.Web && dotnet build -c Release -o /app
RUN rm -rf src
#RUN dotnet tool install --global user-secrets &&

# Sets the working directory to the build output folder and runs the app
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app/build


COPY --from=build-env /app .
ENTRYPOINT ["dotnet", "Chirp.Web.dll"]

# Exposes the port
EXPOSE 5273/tcp
#EXPOSE 1433/tcp
