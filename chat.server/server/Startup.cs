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

namespace server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        
        public void ConfigureServices(IServiceCollection services)
        {
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
            //productionProps.Add("connection.connection_string", "Server = 127.0.0.1; Port = 5432; Database = postgres; User Id = postgres; Password = password");
            productionProps.Add("connection.connection_string", "Server=jelani.db.elephantsql.com;Port=5432;Username=utzedayv;Password=VUeNGDd0UKc_tek3BRXRtYDBCeP0Z7_S;Database=utzedayv");
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

            app.UseRouting();

            app.UseCors();

            app.UseSpaStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chat");
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
