# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY NuGet.Config ./
COPY Ouro.nuget ./Ouro.nuget

COPY . ./

RUN dotnet restore "IntDorSys.Web.Api/IntDorSys.Web.Api.csproj" --configfile NuGet.Config && \
    dotnet publish "IntDorSys.Web.Api/IntDorSys.Web.Api.csproj" -c Release -o /out --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
ARG BACKEND_PORT=5050
WORKDIR /app

RUN apt-get update && \
    apt-get install -y --no-install-recommends curl && \
    rm -rf /var/lib/apt/lists/*

COPY --from=build /out .

EXPOSE ${BACKEND_PORT}

HEALTHCHECK --interval=30s --timeout=3s --retries=3 \
    CMD curl -f http://localhost:${BACKEND_PORT}/health || exit 1

ENTRYPOINT ["dotnet", "IntDorSys.Web.Api.dll"]
