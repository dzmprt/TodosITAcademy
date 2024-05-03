using System.Reflection;
using Serilog;
using Serilog.Events;
using Todos.Api;
using Todos.Api.Middlewares;
using Todos.Application;
using Todos.ExternalProviders;
using Todos.Persistence;

try
{
    const string version = "v1";
    const string appName = "Todos API v1";

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.File($"{builder.Configuration["Logging:LogsFolder"]}/Information-.txt", LogEventLevel.Information,
            rollingInterval: RollingInterval.Day, retainedFileCountLimit: 3, buffered: true)
        .WriteTo.File($"{builder.Configuration["Logging:LogsFolder"]}/Warning-.txt", LogEventLevel.Warning,
            rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14, buffered: true)
        .WriteTo.File($"{builder.Configuration["Logging:LogsFolder"]}/Error-.txt", LogEventLevel.Error,
            rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30, buffered: true));

    builder.Services
        .AddSwaggerWidthJwtAuth(Assembly.GetExecutingAssembly(), appName, version, appName)
        .AddCoreApiServices()
        .AddCoreAuthApiServices(builder.Configuration)
        .AddPersistenceServices(builder.Configuration)
        .AddAllCors()
        .AddExternalProviders()
        .AddTodosApplicationsServices()
        .AddHttpClient();

    var app = builder.Build();

    app.RunDbMigrations().RegisterApis(Assembly.GetExecutingAssembly(), $"api/{version}");

    app.UseCoreExceptionHandler()
        .UseAuthExceptionHandler()
        .UseSwagger(c => { c.RouteTemplate = "swagger/{documentname}/swagger.json"; })
        .UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/swagger/{version}/swagger.json", version);
            options.RoutePrefix = "swagger";
        })
        .UseAuthentication()
        .UseAuthorization()
        .UseHttpsRedirection()
        .UseCors();

    app.Run();
}
catch (Exception ex)
{
    var appSettingsFile = $"{Directory.GetCurrentDirectory()}/appsettings.json";
    var settingsJson = File.ReadAllText(appSettingsFile);
    var appSettings = System.Text.Json.JsonDocument.Parse(settingsJson);
    var logsPath = appSettings.RootElement.GetProperty("Logging").GetProperty("LogsFolder").GetString();
    var logger = new LoggerConfiguration()
        .WriteTo.File($"{logsPath}/Log-Run-Error-.txt", LogEventLevel.Error, rollingInterval: RollingInterval.Hour,
            retainedFileCountLimit: 30)
        .CreateLogger();
    logger.Fatal(ex.Message, ex);
}