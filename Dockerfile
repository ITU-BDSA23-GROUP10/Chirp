FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
#/home/slug/Documents/GitRepos/BDSA_repo/Chirp/.devcontainer

RUN apt-get install -y sed

FROM build-env as build

WORKDIR /source
# Adds the source code to the container

ADD src ./

# Restores the dependencies, builds the project and deletes the source code
WORKDIR /source/Chirp.Web
RUN sed -ie '/^{/a "ConnectionStrings": { "ConnectionString": "Server=db,1433;Initial Catalog=master;User ID=sa;Password=YourGonnaBurnAlright1234;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" },' appsettings.json
#RUN cd Chirp.Web &&  dotnet dev-certs https --clean && dotnet dev-certs https --trust
RUN dotnet restore 
RUN dotnet build -c Release -o /build
RUN cp -r wwwroot /build

FROM build-env as run
WORKDIR /app

COPY --from=build /build .
COPY ./scripts/entrypoint.sh .
RUN mkdir -p Chirp/Chirp.Web

CMD /bin/bash ./entrypoint.sh

# Exposes the port
EXPOSE 5273/tcp
