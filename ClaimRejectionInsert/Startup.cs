using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ClaimRejectionInsert.Models.Configuration;
using ClaimRejectionInsert_DOTNET.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ClaimRejectionInsert_DOTNET
{
    public class Startup
    {
        public static string AdGroupAuthenticate = null;
        public static string SecretUsername = null;
        public static string SecretPassword = null;
        public static string ConnectionString = null;
        public static string ServiceAccount = null;

        public IConfiguration Configuration { get; }
        private readonly ILogger _logger;


        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            ConnectionString = Configuration["ConnectionStrings:DevConnection"];
            ConnectionString = Configuration["ConnectionStrings:DevConnection"];
            AdGroupAuthenticate = Configuration["AdGroupAuthenticate"];
            SecretUsername = Configuration["Secrets:SecretUsername"];
            SecretPassword = Configuration["Secrets:SecretPassword"];

            _logger.LogInformation("Added TodoRepository to services");
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                _logger.LogInformation("In Development environment...");
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
/**
{
  "AdGroupAuthenticate": "AA-DEV-USER-XCCLAIMREJECT",
  "AllowedHosts": "*",
 
  "ConnectionStrings": {
    "DevConnection": "Data Source=DEVFCTASE11;Port=47654;Database=APP_CONFIG;Uid=XXXXX;Pwd=XXXXX;",
    "QualConnection": "Data Source=QAEFCTDBS0101;Port=XXXXX;Database=APP_CONFIG;Uid=XXXXX;Pwd=XXXXX;",
    "ProdConnection": "Data Source=PRDFCTASE01;Port=XXXXX;Database=APP_CONFIG;Uid=XXXXX;Pwd=XXXXX;",
    "DefaultConnection1": "Data Source=DEVFCTASE11;Port=47654;Database=APP_CONFIG;Uid=FD179893;Pwd=Ready4gocaleb!;",
    "DefaultConnection2": "Data Source=DEVPTLASE01;Port=4300;Database=tempdb;Integrated Security=SSPI,8; ConnectionIdleTimeout=2"
  },
 
  "Secrets": {
 
    "SecretUsername": "srvc_rejclaims",
    "SecretPassword": "sc0Mii*psEXP&#5Bf(z!4YF9yw4%4C"
 
  },
 
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
    });
});

**/
