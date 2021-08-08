using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(BarakaBg.Web.Areas.Identity.IdentityHostingStartup))]

namespace BarakaBg.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}
