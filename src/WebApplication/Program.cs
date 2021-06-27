using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

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
