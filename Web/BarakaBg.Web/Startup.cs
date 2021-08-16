﻿namespace BarakaBg.Web
{
    using System;
    using System.Reflection;

    using Azure.Storage.Blobs;
    using BarakaBg.Data;
    using BarakaBg.Data.Common;
    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;
    using BarakaBg.Data.Repositories;
    using BarakaBg.Data.Seeding;
    using BarakaBg.Services;
    using BarakaBg.Services.Data;
    using BarakaBg.Services.Mapping;
    using BarakaBg.Services.Messaging;
    using BarakaBg.Web.Hubs;
    using BarakaBg.Web.ViewModels;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Stripe;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication()
                .AddFacebook(fbOptions =>
                    {
                        fbOptions.AppId = "123456789";
                        fbOptions.AppSecret = "987654321";
                    })
                .AddGoogle(ggOptions =>
                {
                    IConfigurationSection googleAuthSection = this.configuration.GetSection("123456789");

                    ggOptions.ClientId = "123456789";
                    ggOptions.ClientSecret = "987654321";
                })
                .AddMicrosoftAccount(msOptions =>
                {
                    msOptions.ClientId = "123456789";
                    msOptions.ClientSecret = "987654321";
                })
                .AddTwitter(twOptions =>
                {
                    twOptions.ConsumerKey = "123456789";
                    twOptions.ConsumerSecret = "987654321";
                    twOptions.RetrieveUserDetails = true;
                });

            services.Configure<CookiePolicyOptions>(
                options =>
                    {
                        options.CheckConsentNeeded = context => true;
                        options.MinimumSameSitePolicy = SameSiteMode.None;
                    });

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(60);
            });

            StripeConfiguration.ApiKey = this.configuration["Stripe:SecretKey"];

            services.AddSignalR(x => x.EnableDetailedErrors = true);

            services.AddControllersWithViews(
                options =>
                    {
                        options
                            .Filters
                            .Add(new AutoValidateAntiforgeryTokenAttribute());
                    }).AddRazorRuntimeCompilation();

            services.AddRazorPages();
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
            });

            services.AddSingleton(this.configuration);
            services.AddSingleton(x =>
                new BlobServiceClient(this.configuration.GetValue<string>("BlobConnectionString")));

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            // Application services
            services.AddTransient<IEmailSender>(x => new SendGridEmailSender(this.configuration.GetValue<string>("SendGrid:ApiKey")));
            services.AddTransient<IGetCountsService, GetCountsService>();
            services.AddTransient<ICategoriesService, CategoriesService>();
            services.AddTransient<IProductsService, ProductsService>();
            services.AddTransient<IShoppingBagService, ShoppingBagService>();
            services.AddTransient<IWishListService, WishListService>();
            services.AddTransient<IAddressesService, AddressesService>();
            services.AddTransient<IChatService, ChatService>();
            services.AddTransient<IOrdersService, OrdersService>();
            services.AddTransient<ICountriesService, CountriesService>();
            services.AddTransient<IVotesService, VotesService>();
            services.AddTransient<ITextService, TextService>();
            services.AddTransient<IImagesService, ImagesService>();
            services.AddTransient<IIngredientsService, IngredientsService>();
            services.AddTransient<IBarakaBgScraperService, BarakaBgScraperService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

            // Seed data on application startup
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
                new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                    {
                        endpoints.MapHub<ChatHub>("/chat");
                        endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapRazorPages();
                    });
        }
    }
}
