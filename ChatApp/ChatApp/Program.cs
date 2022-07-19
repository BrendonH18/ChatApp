using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.IdentityModel.Tokens;
using NHibernate.NetCore;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
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
builder.Services.AddMvc();
builder.Services.AddControllers();
builder.Services.AddSignalR(x => x.EnableDetailedErrors = true);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:3000") // add url
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
//builder.Services.AddSingleton<IConnectionService, ConnectionService>(options => new ConnectionService());

//NHibernate Automated
var productionProps = new Dictionary<string, string>();
productionProps.Add("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
productionProps.Add("dialect", "NHibernate.Dialect.PostgreSQL83Dialect");
productionProps.Add("hibernate.connection.driver_class", "NHibernate.Driver.NpgsqlDriver");
//productionProps.Add("connection.connection_string", "Server = 127.0.0.1; Port = 5432; Database = postgres; User Id = postgres; Password = password");
productionProps.Add("connection.connection_string", "Server=jelani.db.elephantsql.com;Port=5432;Username=utzedayv;Password=VUeNGDd0UKc_tek3BRXRtYDBCeP0Z7_S;Database=utzedayv");
productionProps.Add("show_sql", "true");

//var cfg = new NHibernate.Cfg.Configuration()
    //.AddFile("Mappings/Channel.hbm.xml")
    //.AddFile("Mappings/Message.hbm.xml")
    //.AddFile("Mappings/User.hbm.xml")
    //.SetProperties(productionProps);
//builder.Services.AddHibernate(cfg);

//builder.Services.AddSpaStaticFiles(configuration =>
//{
//    configuration.RootPath = "client/build";
//});
     

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseAuthorization();
app.UseCors();
//app.UseSpaStaticFiles();

app.UseEndpoints(endpoints =>
{
    //endpoints.MapHub<ChatHub>("/chat");
    endpoints.MapControllers();
});

//app.UseSpa(spa =>
//{
//    spa.Options.SourcePath = "client";
//    if (app.Environment.IsDevelopment())
//        spa.UseReactDevelopmentServer(npmScript: "start");
//});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
