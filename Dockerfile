FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Movement.sln .
COPY Movement.API/Movement.API.csproj Movement.API/
RUN dotnet restore

COPY Movement.API/ Movement.API/
RUN dotnet publish Movement.API/Movement.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Movement.API.dll"]
