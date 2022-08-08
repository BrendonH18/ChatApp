using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using server.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Models;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using NHibernate;
using NHibernate.NetCore;
using server.Hubs.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            if (string.IsNullOrEmpty(accessToken) == false)
                                context.Token = accessToken;
                            return Task.CompletedTask;
                        }
                    };
                });
            services.AddMvc();
            services.AddControllers();
            services.AddSignalR(x => x.EnableDetailedErrors = true);
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:3000") // add url
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            services.AddSingleton<IConnectionService, ConnectionService>(options => new ConnectionService());

            //NHibernate Automated
            var productionProps = new Dictionary<string, string>();
            productionProps.Add("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
            productionProps.Add("dialect", "NHibernate.Dialect.PostgreSQL83Dialect");
            productionProps.Add("hibernate.connection.driver_class", "NHibernate.Driver.NpgsqlDriver");
            productionProps.Add("connection.connection_string", Configuration["ConnectionStrings:Web"]);
            productionProps.Add("show_sql", "true");

            var cfg = new NHibernate.Cfg.Configuration()
                .AddFile("Mappings/Channel.hbm.xml")
                .AddFile("Mappings/Message.hbm.xml")
                .AddFile("Mappings/User.hbm.xml")
                .SetProperties(productionProps);
            services.AddHibernate(cfg);

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "client/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //not sure what to put here
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors();

            app.UseSpaStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chat");
                endpoints.MapControllers();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "client";
                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
