using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// Registrar aqui los servicios propios del ETL cuando se cree un proceso real.

using IHost host = builder.Build();

ILogger logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("ETL.NombreProceso");

logger.LogInformation("Template ETL NombreProceso inicializado.");

return 0;
