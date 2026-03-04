FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

COPY ["Simplix.PortalClient.sln", "./"]
COPY ["src/Simplix.PortalClient.Web/Simplix.PortalClient.Web.csproj", "src/Simplix.PortalClient.Web/"]
COPY ["src/Simplix.PortalClient.Application/Simplix.PortalClient.Application.csproj", "src/Simplix.PortalClient.Application/"]
COPY ["src/Simplix.PortalClient.Domain/Simplix.PortalClient.Domain.csproj", "src/Simplix.PortalClient.Domain/"]
COPY ["src/Simplix.PortalClient.Infrastructure/Simplix.PortalClient.Infrastructure.csproj", "src/Simplix.PortalClient.Infrastructure/"]

RUN dotnet restore "src/Simplix.PortalClient.Web/Simplix.PortalClient.Web.csproj"

COPY . .
RUN dotnet publish "src/Simplix.PortalClient.Web/Simplix.PortalClient.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app

RUN apk add --no-cache wget

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Simplix.PortalClient.Web.dll"]

