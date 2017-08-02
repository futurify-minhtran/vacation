using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AuthenticationServer.ServicesInterfaces;
using AuthenticationServer.Services;
using AuthenticationServer.Setup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Newtonsoft.Json.Serialization;
using AuthenticationServer.Models.BindingModels;
using App.Common.Core.Models;
using AuthenticationServer.Providers;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using App.Common.Core.Authentication;

namespace AuthenticationServer
{
    public class Startup
    {
        private string jwtSecretToken, jwtAudience, jwtIssuer;
        private int jwtExpiresInDays;

        private string _contentRootPath;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            var jwtConfigs = Configuration.GetSection("Authentication").GetSection("JWT");

            jwtSecretToken = jwtConfigs["SecretKey"];
            jwtAudience = jwtConfigs["Audience"];
            jwtIssuer = jwtConfigs["Issuer"];
            jwtExpiresInDays = int.Parse(jwtConfigs["ExpiresInDays"]);

            _contentRootPath = env.ContentRootPath;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<TokenProviderOptions>(options =>
            {
                // secretKey contains a secret passphrase only your server knows
                var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this.jwtSecretToken));
                options.Audience = jwtAudience;
                options.Issuer = jwtIssuer;
                options.Expiration = TimeSpan.FromDays(jwtExpiresInDays);
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            });

            services.Configure<EmailTemplate>(Configuration.GetSection("EmailTemplate"));
            services.Configure<ConfigSendEmail>(Configuration.GetSection("ConfigSendEmail"));

            services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
             
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IPermissionService, PermissionService>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAllOrigins"));
            });

            // Add framework services.
            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            AuthDbContext.UpdateDatabase(app);
            app.ConfigurePermissions();

            app.UseCors("AllowAllOrigins");

            //config using jwt authentication
            app.UseJwt(this.jwtSecretToken, this.jwtAudience, this.jwtIssuer);
            //config jwt provider middleware
            app.UseMiddleware<TokenProviderMiddleware>();

            app.UseMvc();
        }
    }
}
