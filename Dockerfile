# ================= BUILD =================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Template.sln .
COPY API/API.csproj API/
COPY Application/Application.csproj Application/
COPY Domain/Domain.csproj Domain/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY Shared/Shared.csproj Shared/

RUN dotnet restore Template.sln

COPY . .
RUN dotnet publish API/API.csproj -c Release -o /app/publish

# ================= RUNTIME =================
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080
ENTRYPOINT ["dotnet", "API.dll"]
