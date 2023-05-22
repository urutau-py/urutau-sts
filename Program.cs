using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using urutau.Data;
using urutau.Entities;
using urutau.Workers;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{builder.Configuration["Azure:KeyVaultName"]}.vault.azure.net/"),
        new ClientSecretCredential(builder.Configuration["Azure:TenantId"], 
            builder.Configuration["Azure:ClientId"], builder.Configuration["Azure:ClientSecret"]));
}

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // Configure Entity Framework Core to use Microsoft SQL Server.
    if (!builder.Environment.IsProduction())
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));    
    }
    else
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));    
    }
    
    
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
        // Enable the authorization, introspection and token endpoints.
        options.SetAuthorizationEndpointUris("authorize")
            .SetIntrospectionEndpointUris("introspect")
            .SetTokenEndpointUris("/connect/token");
        
        options
            .AllowPasswordFlow()
            .AllowRefreshTokenFlow()
            .AllowClientCredentialsFlow();
        
        // Register the signing and encryption credentials.
        if (builder.Environment.IsDevelopment())
        {
            options
                .AddDevelopmentEncryptionCertificate()
                .AddDevelopmentSigningCertificate();   
        }
        else
        {
            var encryptionKey = builder.Configuration["OpenIdDict:EncryptionKey"];
            var signingKey = builder.Configuration["OpenIdDict:SigningKey"];

            if (string.IsNullOrWhiteSpace(encryptionKey) || string.IsNullOrWhiteSpace(signingKey))
                throw new ArgumentException("Invalid keys");
            
            options
                .AddEncryptionKey(new SymmetricSecurityKey(Convert.FromBase64String(encryptionKey)))
                .AddSigningKey(new SymmetricSecurityKey(Convert.FromBase64String(signingKey)));

            // options.AddSigningCertificate("");
            // options.AddEncryptionCertificate("");
        }

        // Enable this for production
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
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllers();
app.MapDefaultControllerRoute();

app.Run();
