FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
#/home/slug/Documents/GitRepos/BDSA_repo/Chirp/.devcontainer
RUN mkdir Chirp
RUN mkdir app
WORKDIR /Chirp


ADD src ./
ADD GlobalUsings.cs ./

#RUN dotnet tool install --global dotnet-user-secrets && dotnet user-secrets init && dotnet user-secrets set 'ConnectionStrings:ConnectionString' 'Data Source=localhost,1433;Initial Catalog=Chirp;User ID=sa;Password=P@ssw0rd;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False' 

#WORKDIR /Chirp/src/Chirp.Web
RUN 
RUN cd Chirp.Web && dotnet restore 
RUN cd Chirp.Web && dotnet build -c Release -o /app
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app/build

COPY --from=build-env /app .
ENTRYPOINT ["dotnet", "Chirp.Web.dll"]

EXPOSE 5273/tcp

