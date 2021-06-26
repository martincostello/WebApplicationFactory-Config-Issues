using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace WebApplication.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Application_Returns_Defaults()
        {
            // Arrange
            using var fixture = new WebApplicationFactory<FakeEntryPoint>();

            // Act
            using var client = fixture.CreateClient();
            using var document = JsonDocument.Parse(await client.GetStringAsync("/api"));

            // Assert
            Assert.Equal("Production", document.RootElement.GetProperty("environment").GetString());
            Assert.Equal("a", document.RootElement.GetProperty("value").GetString());
        }

        [Fact]
        public async Task Can_Override_Configuration()
        {
            // Arrange
            using var fixture = new WebApplicationFactory<FakeEntryPoint>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration(builder =>
                {
                    var config = new[]
                    {
                        KeyValuePair.Create("Value", "b"),
                    };

                    builder.AddInMemoryCollection(config);
                });
            });

            // Act
            using var client = fixture.CreateClient();
            using var document = JsonDocument.Parse(await client.GetStringAsync("/api"));

            // Assert
            Assert.Equal("b", document.RootElement.GetProperty("value").GetString());
        }

        [Fact]
        public async Task Can_Remove_Configuration()
        {
            // Arrange
            using var fixture = new WebApplicationFactory<FakeEntryPoint>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration(builder =>
                {
                    builder.Sources.Clear();
                });
            });

            // Act
            using var client = fixture.CreateClient();
            using var document = JsonDocument.Parse(await client.GetStringAsync("/api"));

            // Assert
            Assert.Equal("", document.RootElement.GetProperty("value").GetString());
        }

        [Fact]
        public async Task Can_Override_Configuration_When_Extra_Configuration_Removed()
        {
            // Arrange
            using var fixture = new WebApplicationFactory<FakeEntryPoint>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration(builder =>
                {
                    var config = new[]
                    {
                        KeyValuePair.Create("Value", "b"),
                    };

                    builder.AddInMemoryCollection(config);
                });

                builder.ConfigureServices(services =>
                {
                    var descriptors = services.Where(p => p.ServiceType == typeof(IConfiguration)).ToList();
                    services.Remove(descriptors[1]);
                });
            });

            // Act
            using var client = fixture.CreateClient();
            using var document = JsonDocument.Parse(await client.GetStringAsync("/api"));

            // Assert
            Assert.Equal("b", document.RootElement.GetProperty("value").GetString());
        }

        [Fact]
        public async Task Can_Override_ContentRoot()
        {
            // Arrange
            using var fixture = new WebApplicationFactory<FakeEntryPoint>().WithWebHostBuilder(builder =>
            {
                string contentRoot = GetType()
                    .Assembly
                    .GetCustomAttributes<AssemblyMetadataAttribute>()
                    .Where((p) => string.Equals(p.Key, "ContentRoot"))
                    .Select((p) => p.Value)
                    .First();

                builder.UseContentRoot(contentRoot);
            });

            // Act
            using var client = fixture.CreateClient();

            // Assert
            Assert.Equal("This is a test.", await client.GetStringAsync("/test.txt"));
        }

        [Fact]
        public async Task Can_Override_Environment()
        {
            // Arrange
            using var fixture = new WebApplicationFactory<FakeEntryPoint>().WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment(Environments.Development);
            });

            // Act
            using var client = fixture.CreateClient();
            using var document = JsonDocument.Parse(await client.GetStringAsync("/api"));

            // Assert
            Assert.Equal("Development", document.RootElement.GetProperty("environment").GetString());
        }
    }
}
