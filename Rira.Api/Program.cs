using NLog;
using NLog.Web;
using Rira.Api;
using Rira.Api.Services;
using Rira.Application;
using Rira.Infrastructure;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Host.UseNLog();
    
    builder.Services.AddApplication();
    
    builder.Services.AddInfrastructure(builder.Configuration);
    
    builder.Services.AddSingleton(TimeProvider.System);
    
    builder.Services.AddGrpc(options => options.Interceptors.Add<ExceptionInterceptor>());
    builder.Services.AddGrpcReflection();
    
    var app = builder.Build();
    
    app.MapGrpcService<UserGrpcService>();
    
    if (app.Environment.IsDevelopment())
    {
        app.MapGrpcReflectionService();
    }

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Application stopped due to an exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}
