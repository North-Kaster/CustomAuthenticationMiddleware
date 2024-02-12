using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using CustomMiddleware;
namespace Middleware.Tests;

public class AuthenticationMiddlewareTests : IAsyncLifetime
{
    IHost? host;
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
    public async Task InitializeAsync()
    {
        host = await new HostBuilder()
        .ConfigureWebHost(webBuilder =>
        {
            webBuilder
            .UseTestServer()
            .ConfigureServices(services =>
            {
            })
            .Configure(app =>
            {
                app.UseMiddleware<Authentication>();
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Authenticated!");
                });
            });
        })
        .StartAsync();
    }
    [Fact]
    public async Task AuthenticationMiddlewareTest_FailWhenNoQuery()
    {
        if (host == null)
        {
            throw new InvalidOperationException("Host is not initialized.");
        }

        var response = await host.GetTestClient().GetAsync("/");
        var result = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("Not authorized.", result);
    }
    [Fact]
    public async Task AuthenticationMiddlewareTest_FailWhenNoPassword()
    {
        if (host == null)
        {
            throw new InvalidOperationException("Host is not initialized.");
        }

        var response = await host.GetTestClient().GetAsync("/?username=user1");
        var result = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("Not authorized.", result);
    }
    [Fact]
    public async Task AuthenticationMiddlewareTest_FailWhenWrongUsernameAndPassword()
    {
        if (host == null)
        {
            throw new InvalidOperationException("Host is not initialized.");
        }

        var response = await host.GetTestClient().GetAsync("/?username=user5&password=password2");
        var result = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("Not authorized.", result);
    }
    [Fact]
    public async Task AuthenticationMiddlewareTest_AuthenticatedUser()
    {
        if (host == null)
        {
            throw new InvalidOperationException("Host is not initialized.");
        }
        
        var response = await host.GetTestClient().GetAsync("/?username=user1&password=password1");
        var result = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Authenticated!", result);
    }
}