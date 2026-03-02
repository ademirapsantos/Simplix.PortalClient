FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

COPY ["KMKKM.PortalClient.sln", "./"]
COPY ["src/KMKKM.PortalClient.Web/KMKKM.PortalClient.Web.csproj", "src/KMKKM.PortalClient.Web/"]
COPY ["src/KMKKM.PortalClient.Application/KMKKM.PortalClient.Application.csproj", "src/KMKKM.PortalClient.Application/"]
COPY ["src/KMKKM.PortalClient.Domain/KMKKM.PortalClient.Domain.csproj", "src/KMKKM.PortalClient.Domain/"]
COPY ["src/KMKKM.PortalClient.Infrastructure/KMKKM.PortalClient.Infrastructure.csproj", "src/KMKKM.PortalClient.Infrastructure/"]

RUN dotnet restore "src/KMKKM.PortalClient.Web/KMKKM.PortalClient.Web.csproj"

COPY . .
RUN dotnet publish "src/KMKKM.PortalClient.Web/KMKKM.PortalClient.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app

RUN apk add --no-cache wget

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "KMKKM.PortalClient.Web.dll"]
