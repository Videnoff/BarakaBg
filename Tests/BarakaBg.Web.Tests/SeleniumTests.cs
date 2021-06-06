namespace BarakaBg.Web.Tests
{
    using System;
    using System.Linq;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Firefox;
    using Xunit;

    public class SeleniumTests : IClassFixture<SeleniumServerFactory<Startup>>, IDisposable
    {
        private readonly SeleniumServerFactory<Startup> server;
        private readonly IWebDriver browser;

        public SeleniumTests(SeleniumServerFactory<Startup> server)
        {
            this.server = server;
            server.CreateClient();
            var opts = new FirefoxOptions();
            opts.AddArguments("--headless");
            opts.AcceptInsecureCertificates = true;
            this.browser = new FirefoxDriver(opts);
        }

        [Fact(Skip = "Example test. Disabled for CI.")]
        public void FooterOfThePageContainsPrivacyLink()
        {
            this.browser.Navigate().GoToUrl(this.server.RootUri);
            Assert.EndsWith(
                "/Home/Privacy",
                this.browser.FindElements(By.CssSelector("footer a")).First().GetAttribute("href"));
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.server?.Dispose();
                this.browser?.Dispose();
            }
        }
    }
}
