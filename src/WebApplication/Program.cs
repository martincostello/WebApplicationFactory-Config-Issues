using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseEndpoints((endpoints) =>
{
    endpoints.MapGet("/api", async (IConfiguration config, IHostEnvironment environment, IServiceProvider serviceProvider) =>
    {
        await Task.CompletedTask;

        return new
        {
            configurations = serviceProvider.GetServices<IConfiguration>().Count(),
            environment = environment.EnvironmentName,
            message = config["Message"],
            value = config["Value"],
        };
    });
});

app.Run();
