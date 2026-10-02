using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ServiceFlow.Api.Tests;

public class ServiceFlowApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting(
            "ConnectionStrings:DefaultConnection", 
            "Host=localhost;Port=5432;Database=serviceflow_tests;Username=postgres;Password=postgres;");
    }
    
}