FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Apis/Users.Api/Users.Api.csproj", "Apis/Users.Api/"]
COPY ["Application/Core/Auth/Core.Auth.Api/Core.Auth.Api.csproj", "Application/Core/Auth/Core.Auth.Api/"]
COPY ["Application/Core/Auth/Core.Auth.Application/Core.Auth.Application.csproj", "Application/Core/Auth/Core.Auth.Application/"]
COPY ["Application/Core/Core.Users.Domain/Core.Users.Domain.csproj", "Application/Core/Core.Users.Domain/"]
COPY ["Application/Core/Core.Api/Core.Api.csproj", "Application/Core/Core.Api/"]
COPY ["Application/Core/Core.Application/Core.Application.csproj", "Application/Core/Core.Application/"]
COPY ["Application/Users/Users.Application/Users.Application.csproj", "Application/Users/Users.Application/"]
COPY ["Infrastructure/Infrastructure.Persistence/Infrastructure.Persistence.csproj", "Infrastructure/Infrastructure.Persistence/"]
COPY ["Application/Auth/Auth.Domain/Auth.Domain.csproj", "Application/Auth/Auth.Domain/"]
COPY ["Application/Todos/Todos.Domain/Todos.Domain.csproj", "Application/Todos/Todos.Domain/"]
RUN dotnet restore "Apis/Users.Api/Users.Api.csproj"
COPY . .
WORKDIR "/src/Apis/Users.Api"
RUN dotnet build "Users.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Users.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Users.Api.dll"]
