using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyFilmBevy.Data;
using MyFilmBevy.Models.Settings;
using MyFilmBevy.Services;
using MyFilmBevy.Services.Interfaces;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// BEGIN CF DB CONFIG
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(ConnectionService.GetConnectionString(builder.Configuration)));
// END CF DB CONFIG

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// // Custom Services Go Here

// The AppSettigs Service
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// The Seed Service
builder.Services.AddTransient<SeedService>();

// Utility Services
builder.Services.AddSingleton<IImageService,BasicImageService>();

// The Remove Movie Serivce
builder.Services.AddHttpClient();
builder.Services.AddScoped<IRemoteMovieService,TMDBMovieService>();

// The Mapping Services
builder.Services.AddScoped<IDataMappingService,TMDBMappingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Run the Seed Service
var dataService = app.Services
    .CreateScope()
    .ServiceProvider
    .GetRequiredService<SeedService>();
await dataService.ManageDataAsync();


app.Run();
