// -----------------------------------------------------------------------
// <copyright file="HttpFilesMiddlewareIntegrationTests.cs" company="HttpFilesMW">
// Copyright © HttpFilesMW. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace HttpFilesMW.Core.IntegrationTests;

public class HttpFilesMiddlewareIntegrationTests : IDisposable
{
    private readonly TestServer server;
    private readonly HttpClient client;

    public HttpFilesMiddlewareIntegrationTests()
    {
        var hostBuilder = new WebHostBuilder()
            .UseTestServer()
            .ConfigureServices(services =>
            {
                // Add MVC services which includes API Explorer
                services.AddControllers();
                services.AddEndpointsApiExplorer();

                // Add your HTTP files processing
                services.AddHttpFilesProcessing();
            })
            .Configure(app =>
            {
                // Configure the HTTP request pipeline
                app.UseHttpFilesProcessing();
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            });

        this.server = new TestServer(hostBuilder);
        this.client = this.server.CreateClient();
    }

    public void Dispose()
    {
        this.client?.Dispose();
        this.server?.Dispose();
    }

    [Fact]
    public async Task GivenInvocationToMiddlewarePath_WhenGetHttpFiles_ThenReturnsGeneratedHttpFiles()
    {
        // Act
        var response = await this.client.GetAsync("/http-files");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.Content.Headers.ContentType?.MediaType.Should().Be("text/plain");
        response.Content.Headers.ContentDisposition?.DispositionType.Should().Be("inline");

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Be("""
            GET api/Test

            ###

            GET api/Test/{id}

            ###

            POST api/Test

            ###
            """);
    }
}
