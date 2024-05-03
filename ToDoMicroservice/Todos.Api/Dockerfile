FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ToDoMicroservice/Todos.Api/Todos.Api.csproj", "ToDoMicroservice/Todos.Api/"]
COPY ["ToDoMicroservice/Todos.Application/Todos.Application.csproj", "ToDoMicroservice/Todos.Application/"]
COPY ["ToDoMicroservice/Todos.Domain/Todos.Domain.csproj", "ToDoMicroservice/Todos.Domain/"]
COPY ["ToDoMicroservice/Todos.Persistence/Todos.Persistence.csproj", "ToDoMicroservice/Todos.Persistence/"]
COPY ["ToDoMicroservice/Todos.ExternalProviders/Todos.ExternalProviders.csproj", "ToDoMicroservice/Todos.ExternalProviders/"]
RUN dotnet restore "ToDoMicroservice/Todos.Api/Todos.Api.csproj"
COPY . .
WORKDIR "/src/ToDoMicroservice/Todos.Api"
RUN dotnet build "Todos.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Todos.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Todos.Api.dll"]
