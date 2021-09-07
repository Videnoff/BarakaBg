namespace BarakaBg.Web.Areas.Identity.Pages.Account
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    using BarakaBg.Common;
    using BarakaBg.Data;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Data;
    using BarakaBg.Services.Messaging;
    using BarakaBg.Web.Infrastructure;
    using BarakaBg.Web.ViewModels.Products;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Logging;

    [AllowAnonymous]
#pragma warning disable SA1649 // File name should match first type name
    public class RegisterModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<RegisterModel> logger;
        private readonly IEmailSender emailSender;
        private readonly IShoppingBagService shoppingBagService;
        private readonly ApplicationDbContext dbContext;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IShoppingBagService shoppingBagService,
            ApplicationDbContext dbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.emailSender = emailSender;
            this.shoppingBagService = shoppingBagService;
            this.dbContext = dbContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Name { get; set; }

            [Required]
            public string Lastname { get; set; }

            [Required]
            public string Town { get; set; }

            [Required]
            public string Country { get; set; }

            [Required]
            public string Address { get; set; }
        }

#pragma warning disable SA1201 // Elements should appear in the correct order
        public async Task OnGetAsync(string returnUrl = null)
#pragma warning restore SA1201 // Elements should appear in the correct order
        {
            this.ReturnUrl = returnUrl;
            this.ExternalLogins = (await this.signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= this.Url.Content("~/");
            this.ExternalLogins = (await this.signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (this.ModelState.IsValid)
            {
                var shoppingBag = new ShoppingBag();
                var user = new ApplicationUser
                {
                    UserName = this.Input.Email,
                    Email = this.Input.Email,
                    ShoppingBag = shoppingBag,
                };

                var result = await this.userManager.CreateAsync(user, this.Input.Password);

                shoppingBag.User = user;
                await this.dbContext.SaveChangesAsync();

                if (result.Succeeded)
                {
                    this.logger.LogInformation("User created a new account with password.");

                    var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = this.Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new
                        {
                            area = "Identity",
                            userId = user.Id,
                            code = code,
                            returnUrl = returnUrl,
                        },
                        protocol: this.Request.Scheme);

                    await this.emailSender.SendEmailAsync(
                        "videnoff@students.softuni.bg",
                        GlobalConstants.SystemName,
                        this.Input.Email,
                        "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (this.userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return this.RedirectToPage("RegisterConfirmation", new { email = this.Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        if (this.User.IsInRole(GlobalConstants.AdministratorName))
                        {
                            return this.RedirectToAction("ListUsers", "Users", new { area = "Administration" });
                        }

                        //await this.signInManager.SignInAsync(user, isPersistent: false);

                        var bag = this.HttpContext.Session.GetObjectFromJson<List<ShoppingBagProductViewModel>>(
                            GlobalConstants.SessionShoppingBagKey);

                        if (bag != null)
                        {
                            foreach (var product in bag)
                            {
                                await this.shoppingBagService.AddProductAsync(true, this.HttpContext.Session, user.Id, product.ProductId, product.Quantity);
                            }

                            this.HttpContext.Session.Remove(GlobalConstants.SessionShoppingBagKey);
                        }

                        //return this.RedirectToPage("./RegisterConfirmation", new {ReturnUrl = returnUrl});

                        return this.RedirectToPage("./RegisterConfirmation", new { Email = this.Input.Email });

                        //return this.LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return this.Page();
        }
    }
}
