FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
#/home/slug/Documents/GitRepos/BDSA_repo/Chirp/.devcontainer
RUN mkdir Chirp
RUN mkdir app
WORKDIR /Chirp

# Adds the source code to the container
ADD src ./

# Restores the dependencies, builds the project and deletes the source code
RUN apt-get install -y sed
RUN sed -ie '/^{/a "ConnectionStrings": { "ConnectionString": "Data Source=localhost,5000;User ID=sa;Password=P@ssw0rd;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" },' Chirp.Web/appsettings.json
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
EXPOSE 5000/tcp
