#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["EAuction.Buyer.Api/EAuction.Buyer.Api.csproj", "EAuction.Buyer.Api/"]
RUN dotnet restore "EAuction.Buyer.Api/EAuction.Buyer.Api.csproj"
COPY . .
WORKDIR "/src/EAuction.Buyer.Api"
RUN dotnet build "EAuction.Buyer.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EAuction.Buyer.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EAuction.Buyer.Api.dll"]