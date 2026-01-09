using Microsoft.AspNetCore.Mvc.Testing;

namespace Rext.ApiSimulator
{
    public class ApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Optional: Configure test-specific settings, e.g., in-memory DB
            builder.ConfigureServices(services =>
            {
                // Example: Override services for testing
                // services.AddDbContext<YourDbContext>(options => options.UseInMemoryDatabase("TestDb"));
            });
        }
    }
}