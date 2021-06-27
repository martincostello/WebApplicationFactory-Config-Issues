using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// HACK Locally it decides to sometimes forget where WebApplication.CreateBuilder is for some reason
var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

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
