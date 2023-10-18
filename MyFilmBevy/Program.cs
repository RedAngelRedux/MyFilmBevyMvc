using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyFilmBevy.Data;
using MyFilmBevy.Models.Settings;
using MyFilmBevy.Services;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//// BEGIN DEFAULT DB CONFIG
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));
//// END DEFAULT DB CONFIG
///
//// BEGIN MY DB CONFIG
//var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
//string? connectionString;

//if (string.IsNullOrEmpty(databaseUrl) == true)
//{
//    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//}
//else
//{
//    var databaseURI = new Uri("postgres://oswnaycnzmksbp:73a1d926d1f508a8b235b62cc3df951d7feae0ff7228ad232d8f5c9f6c33924e@ec2-34-236-199-229.compute-1.amazonaws.com:5432/d19grhiv4vn87\r\n");
//    var userInfo = databaseURI.UserInfo.Split(':');

//    connectionString = new NpgsqlConnectionStringBuilder()
//    {
//        Host = databaseURI.Host,
//        Port = databaseURI.Port,
//        Username = userInfo[0],
//        Password = userInfo[1],
//        Database = databaseURI.LocalPath.TrimStart('/'),
//        SslMode = SslMode.Prefer,
//        TrustServerCertificate = true
//    }.ToString();
//}

//// Replace Default Reference to SQL Server with our Postgres Database
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseNpgsql(connectionString));
//// END MY DB CONFIG
///
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

// Custom Services Go Here
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddTransient<SeedService>();

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
