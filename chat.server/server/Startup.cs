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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using NHibernate;
using NHibernate.NetCore;


namespace server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940


        
        //Figure out how to add services.addhibernate(cfg); 

        
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
            services.AddSingleton<IDictionary<string, UserConnection>>(options => new Dictionary<string, UserConnection>());


            var cfg = new NHibernate.Cfg.Configuration().Configure();
            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App.config");
            services.AddHibernate(cfg);

            // new
            //services.AddControllers();
            //var key = "S3UtwpJ%^iMMl1qIcV@dNnEaO6f6F%ItC7XURDCQ!R0K";
            //services.AddAuthentication(x =>
            //{
            //    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //}).AddJwtBearer(x =>
            //{
            //    x.RequireHttpsMetadata = false;
            //    x.SaveToken = true;
            //    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        ValidateIssuer = false,
            //        ValidateAudience = false,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key))
            //    };
            //});

            //NHibernate Automated
            //var productionProps = new Dictionary<string, string>();
            //productionProps.Add("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
            //productionProps.Add("dialect", "NHibernate.Dialect.PostgreSQL83Dialect");
            //productionProps.Add("hibernate.connection.driver_class", "NHibernate.Driver.NpgsqlDriver");
            //productionProps.Add("connection.connection_string", "Server=jelani.db.elephantsql.com;Port=5432;Username=utzedayv;Password=VUeNGDd0UKc_tek3BRXRtYDBCeP0Z7_S;Database=utzedayv");
            //productionProps.Add("show_sql", "true");


            //var cfg = new NHibernate.Cfg.Configuration()
            //    .AddAssembly("server")
            //    //.SetProperties(productionProps)
            //    .Configure(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"hibernate.cfg.xml"));

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "client/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            app.UseDeveloperExceptionPage();
            //}

            app.UseRouting();

            app.UseCors();

            app.UseSpaStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chat");
                //endpoints.MapControllers(); // new jwt
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
