FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Apis/Auth.Api/Auth.Api.csproj", "Apis/Auth.Api/"]
COPY ["Application/Auth/Auth.Application/Auth.Application.csproj", "Application/Auth/Auth.Application/"]
COPY ["Application/Core/Auth/Core.Auth.Application/Core.Auth.Application.csproj", "Application/Core/Auth/Core.Auth.Application/"]
COPY ["Application/Core/Core.Users.Domain/Core.Users.Domain.csproj", "Application/Core/Core.Users.Domain/"]
COPY ["Application/Core/Core.Application/Core.Application.csproj", "Application/Core/Core.Application/"]
COPY ["Application/Auth/Auth.Domain/Auth.Domain.csproj", "Application/Auth/Auth.Domain/"]
COPY ["Application/Core/Auth/Core.Auth.Api/Core.Auth.Api.csproj", "Application/Core/Auth/Core.Auth.Api/"]
COPY ["Application/Core/Core.Api/Core.Api.csproj", "Application/Core/Core.Api/"]
COPY ["Infrastructure/Infrastructure.Persistence/Infrastructure.Persistence.csproj", "Infrastructure/Infrastructure.Persistence/"]
COPY ["Application/Todos/Todos.Domain/Todos.Domain.csproj", "Application/Todos/Todos.Domain/"]
RUN dotnet restore "Apis/Auth.Api/Auth.Api.csproj"
COPY . .
WORKDIR "/src/Apis/Auth.Api"
RUN dotnet build "Auth.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Auth.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Auth.Api.dll"]
