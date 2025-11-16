# =========================
# 1) Build + publish stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only csproj first (better layer cache)
COPY backend-aspnetcore.csproj backend-aspnetcore/
RUN dotnet restore backend-aspnetcore/backend-aspnetcore.csproj

# Now copy the rest of the source
COPY . backend-aspnetcore/

WORKDIR /src/backend-aspnetcore

# Publish to /app/publish (Release mode, no apphost exe)
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false


# =========================
# 2) Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Non-root user
RUN adduser --disabled-password --home /app appuser && chown -R appuser /app
USER appuser

# Copy published output from *build* stage
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5151 \
    ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5151

# Healthcheck endpoint
HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD curl --fail http://localhost:5151/health || exit 1

ENTRYPOINT ["dotnet", "backend-aspnetcore.dll"]