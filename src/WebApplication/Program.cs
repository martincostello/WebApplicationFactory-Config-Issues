using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Fix naming conflict
using WebApp = Microsoft.AspNetCore.Builder.WebApplication;
var builder = WebApp.CreateBuilder(args);

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapGet("/api", (IConfiguration config, IHostEnvironment environment, IServiceProvider serviceProvider) =>
{
    return new
    {
        configurations = serviceProvider.GetServices<IConfiguration>().Count(),
        environment = environment.EnvironmentName,
        message = config["Message"],
        value = config["Value"],
    };
});

app.Run();
