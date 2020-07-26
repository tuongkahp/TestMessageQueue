#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["TestMessageQueue.Api/TestMessageQueue.Api.csproj", "TestMessageQueue.Api/"]
RUN dotnet restore "TestMessageQueue.Api/TestMessageQueue.Api.csproj"
COPY . .
WORKDIR "/src/TestMessageQueue.Api"
RUN dotnet build "TestMessageQueue.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestMessageQueue.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestMessageQueue.Api.dll"]