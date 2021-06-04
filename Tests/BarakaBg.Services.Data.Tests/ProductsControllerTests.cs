namespace BarakaBg.Services.Data.Tests
{
    using BarakaBg.Data;
    using BarakaBg.Data.Models;
    using BarakaBg.Web.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class ProductsControllerTests
    {
        [Fact]
        public void ByIdShouldReturnNotFoundIfProductsDoesNotExists()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);

            var controller = new ProductsController(dbContext);

            var result = controller.ById(3);
            var okResult = result as OkObjectResult;

            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}
