# See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
# Build this Dockerfile at the repo's root level

# FROM node:slim as node_modules
# WORKDIR /dotflik/src/Dotflik.WebApp.Client
# COPY ./src/Dotflik.WebApp.Client/package.json package.json
# COPY ./src/Dotflik.WebApp.Client/package-lock.json package-lock.json
# RUN npm install

####################################################################
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /dotflik/

RUN apt-get update && apt-get install -y nodejs

COPY ./Dotflik.sln .
COPY ./src ./src
COPY ./tests ./tests

# RUN dotnet build "Dotflik.WebApp.${SERVICE}/Dotflik.WebApp.${SERVICE}.csproj" -c Release -o /app/build
RUN dotnet build "Dotflik.sln" -c Release -o /app/build

####################################################################
FROM build AS publish
RUN dotnet publish "Dotflik.sln" -c Release -o /app/publish

####################################################################
FROM mcr.microsoft.com/dotnet/aspnet:5.0
# ENV ASPNETCORE_ENVIRONMENT "Development"

WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet"]
