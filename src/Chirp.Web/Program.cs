using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Chirp.Web;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Models;
using Chirp.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Chirp.Infrastructure.ChirpRepository;

var builder = WebApplication.CreateBuilder(args);

//builder.Logging.AddConsole();
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"));
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

builder.Services.AddOptions();

// Set up the database path
/*var DbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ??
    Path.Combine(Path.GetTempPath(), "chirp.db");
var connectionString = $"Data Source={DbPath}";*/

// Add services to the container.

/*builder.Services.AddDbContext<ChirpDBContext>((serviceProvider, options) =>
{
    var dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ??
    Path.Combine(Path.GetTempPath(), "chirp.db");
    options.UseSqlite($"Data Source={dbPath}"); 
}, ServiceLifetime.Scoped);*/
builder.Services.AddRazorPages();
builder.Services.AddScoped<ICheepRepository<Cheep, Author>, CheepRepository>();
builder.Services.AddScoped<IAuthorRepository<Author, Cheep, User>, AuthorRepository>();
builder.Services.AddScoped<IUserRepository<User>, UserRepository>();
//builder.Services.AddScoped<IReactionRepository<Reaction>, ReactionRepository>();
builder.Services.AddDbContext<ChirpDBContext>(
    options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ChirpDBContext>();
        context.Database.Migrate();
        DbInitializer.SeedDatabase(context);
        

    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
public partial class Program { }