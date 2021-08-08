namespace BarakaBg.Web.Controllers
{
    using System.IO;
    using System.Threading.Tasks;

    using BarakaBg.Services.Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Stripe;

    [Route("api/[controller]")]
    [ApiController]
    [IgnoreAntiforgeryToken]
    public class StripeWebHookController : BaseController
    {
        private readonly IOrdersService ordersService;
        private readonly IConfiguration configuration;

        public StripeWebHookController(
            IOrdersService ordersService,
            IConfiguration configuration)
        {
            this.ordersService = ordersService;
            this.configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(this.HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var striperEvent = EventUtility.ConstructEvent(
                    json,
                    this.Request.Headers["Stripe-Signature"],
                    this.configuration["Stripe:WebHookKey"]);

                if (striperEvent.Type == Events.CheckoutSessionCompleted)
                {
                    var session = striperEvent.Data.Object as Stripe.Checkout.Session;

                    if (session.PaymentStatus == "paid")
                    {
                        await this.ordersService.FulfillOrderById(session.Metadata["order_id"], session.PaymentIntentId);
                    }
                }

                return this.Ok();
            }
            catch (StripeException)
            {
                return this.BadRequest();
            }
        }
    }
}
