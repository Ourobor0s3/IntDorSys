# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем NuGet.Config и локальный feed (если нужен)
COPY NuGet.Config ./
COPY Ouro.nuget ./Ouro.nuget

# Копируем проект
COPY . ./

# Восстанавливаем пакеты
RUN dotnet restore "IntDorSys.Web.Api/IntDorSys.Web.Api.csproj" --configfile NuGet.Config && \
    dotnet publish "IntDorSys.Web.Api/IntDorSys.Web.Api.csproj" -c Release -o /out --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /out .

EXPOSE 8080