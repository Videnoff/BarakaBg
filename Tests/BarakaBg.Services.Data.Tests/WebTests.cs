namespace BarakaBg.Services.Data.Tests
{
    using System.Net.Http;
    using System.Threading.Tasks;

    using BarakaBg.Web;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Xunit;

    public class WebTests
    {
        public async Task HomePageShouldContainTitleHeading()
        {
            var webApplicationFactory = new WebApplicationFactory<Startup>();
            HttpClient client = webApplicationFactory.CreateClient();

            var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("<h1 class=\"display-4\">Welcome to BarakaBg!</h1>", html);
        }
    }
}
