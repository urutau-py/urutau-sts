using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using urutau.Data;
using urutau.Entities;
using urutau.Workers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // Configure Entity Framework Core to use Microsoft SQL Server.
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    
    // Register the entity sets needed by OpenIddict.
    // Note: use the generic overload if you need to replace the default OpenIddict entities.
    options.UseOpenIddict();
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddOpenIddict()

    // Register the OpenIddict core components.
    .AddCore(options =>
    {
        // Configure OpenIddict to use the Entity Framework Core stores and models.
        // Note: call ReplaceDefaultEntities() to replace the default entities.
        options.UseEntityFrameworkCore()
            .UseDbContext<ApplicationDbContext>();
    })

    // Register the OpenIddict server components.
    .AddServer(options =>
    {
        // Enable the token endpoint.
        options.SetTokenEndpointUris("connect/token");

        // Enable the client credentials flow.
        options.AllowClientCredentialsFlow();

        // Register the signing and encryption credentials.
        options
            .AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();

        options.DisableAccessTokenEncryption(); 
        
        // Register the ASP.NET Core host and configure the ASP.NET Core options.
        options
            .UseAspNetCore()
            .EnableTokenEndpointPassthrough();
    })

    // Register the OpenIddict validation components.
    .AddValidation(options =>
    {
        // Import the configuration from the local OpenIddict server instance.
        options.UseLocalServer();

        // Register the ASP.NET Core host.
        options.UseAspNetCore();
    });

builder.Services.AddHostedService<SeedWorker>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();

app.Run();
