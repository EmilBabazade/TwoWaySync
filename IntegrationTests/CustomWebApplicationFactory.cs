using Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IntegrationTests;
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            //services.
            services.RemoveAll<DataContext>();
            services.RemoveAll<DbContext>();
            services.RemoveAll<DbContextOptions<DataContext>>();
            services.AddDbContext<DataContext>((container, options) =>
            {
                options.UseInMemoryDatabase("testDb");
            });
            services.AddScoped<DbContext, DataContext>();
        });

        builder.UseEnvironment("Development");
    }
}
