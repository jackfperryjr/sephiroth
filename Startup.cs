using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sephiroth.Authorization;
using Sephiroth.Services;
using Sephiroth.Data;

namespace Sephiroth
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        private IHostingEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            /*
            if (Environment.IsProduction())
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("SephirothConnection")));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            }
            */
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            
            var skipHTTPS = Configuration.GetValue<bool>("LocalTest:skipHTTPS");
            
            services.Configure<MvcOptions>(options =>
            {
                // Set LocalTest:skipHTTPS to true to skip SSL requrement in 
                // debug mode. This is useful when not using Visual Studio.
                if (Environment.IsDevelopment() && !skipHTTPS)
                {
                    options.Filters.Add(new RequireHttpsAttribute());
                }
            });

            services.AddSingleton<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);
            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            // Authorization handlers.
            services.AddScoped<IAuthorizationHandler,
                                  RequestIsOwnerAuthorizationHandler>();

            services.AddSingleton<IAuthorizationHandler,
                                  RequestAdministratorsAuthorizationHandler>();

            services.AddSingleton<IAuthorizationHandler,
                                  RequestManagerAuthorizationHandler>();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });        }
    }
}