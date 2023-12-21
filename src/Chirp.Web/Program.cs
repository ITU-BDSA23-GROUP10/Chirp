using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Models;
using Chirp.Core;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Chirp.Infrastructure.ChirpRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the builder. This is used login and authentication (using Azure AD B2C)
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"));
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

// Adds all the repositories to the builder using scopes
// It also adds the database context (SQL server) to the builder (using the connection string from dotnet user-secrets)

builder.Services.AddOptions();
builder.Services.AddRazorPages();
builder.Services.AddScoped<ICheepRepository<Cheep, Author>, CheepRepository>();
builder.Services.AddScoped<IAuthorRepository<Author, Cheep, User>, AuthorRepository>();
builder.Services.AddScoped<IUserRepository<User>, UserRepository>();
builder.Services.AddScoped<IFollowsRepository<Follows>, FollowsRepository>();
builder.Services.AddScoped<IReactionRepository<Reaction>, ReactionRepository>();
builder.Services.AddDbContext<ChirpDBContext>(
    options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));

// Creates database html exception pages if there is an error 
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
// This is split into development and production
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

// This is used to migrate the database and seed it with data if it is empty
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

// This is used to get https redirects (You can see the http and https ports in the launchSettings.json file)
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

// Works in tandem with Azure AD B2C
app.UseAuthorization();

// This is used to map the razor pages and controllers to the app (this is how the pages are displayed)
app.MapRazorPages();
app.MapControllers();

app.Run();
public partial class Program { }