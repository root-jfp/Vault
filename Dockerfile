# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Vault.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Copy static files served via WebRootPath="." that publish may skip
COPY --from=build /src/*.html ./
COPY --from=build /src/*.css ./
COPY --from=build /src/*.js ./

RUN mkdir -p /app/data

ENV ASPNETCORE_URLS=http://+:8080
ENV VAULT_DB_PATH=/app/data/vault.db
EXPOSE 8080

ENTRYPOINT ["dotnet", "Vault.dll"]
