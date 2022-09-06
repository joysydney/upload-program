// See https://aka.ms/new-console-template for more information
 
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using upload_program;
using upload_program.Configuration;
using upload_program.Services;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, configuration) =>
    {
        configuration.AddJsonFile("appsettings.json");
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHttpClient();
        services.AddScoped<Authenticate>(); 
        services.AddScoped<CSAccess>(); 
        services.AddScoped<LogManagerCustom>(); 
        services.AddScoped<CSV>(); 
        services.AddScoped<Main>(); 
        services.AddScoped<IAuthenticate, Authenticate>(); 
        services.AddScoped<IMain, Main>(); 
        services.AddScoped<ICSV, CSV>(); 
        services.AddScoped<ICSAccess, CSAccess>();
        services.AddScoped<ILogManagerCustom, LogManagerCustom>();
        services.Configure<BaseConfiguration>(context.Configuration.GetSection(nameof(BaseConfiguration.APIConnection)));
        services.Configure<LogConfiguration>(context.Configuration.GetSection(nameof(LogConfiguration.Logger)));
        services.Configure<UploadConfiguration>(context.Configuration.GetSection(nameof(UploadConfiguration.Upload)));
    })
    .Build();

var timer = new Stopwatch();

timer.Start();
Main m = host.Services.GetRequiredService<Main>();
await m.ProcessFiles();
timer.Stop();

LogManagerCustom log = host.Services.GetRequiredService<LogManagerCustom>();
log.debug("total time taken: "+ timer.Elapsed.TotalSeconds);

await host.RunAsync();