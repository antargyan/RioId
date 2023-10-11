using Duende.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using RioId.Data;
using RioId.Models;
using Serilog;
using System.Globalization;

namespace RioId
{
    internal static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });

            builder.Services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            builder.Services.AddRazorPages();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;

                    // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                    options.EmitStaticAudienceClaim = true;
                })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddAspNetIdentity<ApplicationUser>();

            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to https://localhost:5001/signin-google
                    options.ClientId = "copy client ID from Google here";
                    options.ClientSecret = "copy client secret from Google here";
                })
                .AddFacebook(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.AppId = "copy app id";
                    options.AppSecret = "copy app secret";
                })
                .AddTwitter(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ConsumerKey = "key";
                    options.ConsumerSecret = "secret";
                })
                .AddMicrosoftAccount(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = "ClientId";
                    options.ClientSecret = "clientsecret";
                });

            builder.Services.AddMvc()
                .AddViewLocalization(options =>
                {
                    options.ResourcesPath = "Resources";
                })
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(AppResources));
                })
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Manage");
                    options.Conventions.AuthorizePage("/Logout");
                });
            return builder.Build();
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            var cultures = GetCultures(app.Configuration);

            app.UseRequestLocalization(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(cultures[0]);
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.MapControllers();
            app.MapRazorPages()
                .RequireAuthorization();

            return app;
        }

        private static List<CultureInfo> GetCultures(IConfiguration configuration)
        {
            var result = new List<CultureInfo>();

            var cultures = configuration.GetValue<string>("app:cultures");

            if (!string.IsNullOrWhiteSpace(cultures))
            {
                foreach (var culture in cultures.Split(','))
                {
                    try
                    {
                        result.Add(CultureInfo.GetCultureInfo(culture.Trim()));
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            if (result.Count == 0)
            {
                result.Add(CultureInfo.GetCultureInfo("en"));
            }

            return result;
        }
    }
}