using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Data;
using Service.Models;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://+:{port}");
}

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("TaxCertificateFunction", client =>
{
    var functionUrl = builder.Configuration["TaxCertificateFunctionUrl"];
    if (!string.IsNullOrWhiteSpace(functionUrl))
    {
        client.BaseAddress = new Uri(functionUrl);
    }
});

// SQL Server when an Azure-style connection string is present. Otherwise SQLite, so the
// app can be hosted without an Azure SQL subscription.
var conn = builder.Configuration.GetConnectionString("GiftOfTheGivers") ?? builder.Configuration["ConnectionStrings:GiftOfTheGivers"];
var useSqlServer = !string.IsNullOrWhiteSpace(conn)
    && conn.Contains("Server=", StringComparison.OrdinalIgnoreCase);
builder.Services.AddDbContext<GiftDbContext>(options =>
{
    if (useSqlServer)
    {
        options.UseSqlServer(conn);
        return;
    }

    var sqlite = string.IsNullOrWhiteSpace(conn)
        ? $"Data Source={Path.Combine(builder.Environment.ContentRootPath, "data", "gift.db")}"
        : conn;
    var dataSource = new SqliteConnectionStringBuilder(sqlite).DataSource;
    if (!string.IsNullOrWhiteSpace(dataSource)
        && !dataSource.Contains("mode=memory", StringComparison.OrdinalIgnoreCase))
    {
        var directory = Path.GetDirectoryName(dataSource);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    options.UseSqlite(sqlite);
});

// Simple cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// Authorization policy (default require authenticated)
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Password hasher for User
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GiftDbContext>();
    db.Database.EnsureCreated();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
    EnsureUser(db, hasher, "admin@giftofthegivers.org", "Gift", "Admin", "Admin", "Admin#12345");
    EnsureUser(db, hasher, "employee@giftofthegivers.org", "Amina", "Employee", "Employee", "Employee#12345");
    EnsureUser(db, hasher, "donor@giftofthegivers.org", "Demo", "Donor", "Donor", "Donor#12345");
    db.SaveChanges();

    if (!db.ReliefProjects.Any())
    {
        var adminId = db.Users.Single(u => u.Email == "admin@giftofthegivers.org").UserId;
        db.ReliefProjects.AddRange(
            new ReliefProject
            {
                ProjectName = "General Relief Fund",
                Location = "South Africa",
                Description = "Unrestricted gifts used wherever the need is greatest.",
                Status = "Active",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedByUserId = adminId,
                CreatedAt = DateTime.UtcNow
            },
            new ReliefProject
            {
                ProjectName = "KZN Flood Response",
                Location = "KwaZulu-Natal",
                Description = "Emergency shelter, food, and clean water after flooding.",
                Status = "Active",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedByUserId = adminId,
                CreatedAt = DateTime.UtcNow
            });
        db.SaveChanges();
    }
}

static void EnsureUser(GiftDbContext db, IPasswordHasher<User> hasher, string email, string firstName, string lastName, string role, string password)
{
    if (db.Users.Any(u => u.Email == email))
    {
        return;
    }

    var user = new User
    {
        Email = email,
        FirstName = firstName,
        LastName = lastName,
        Role = role,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };
    user.PasswordHash = hasher.HashPassword(user, password);
    db.Users.Add(user);
}

// Configure the HTTP request pipeline.
app.UseForwardedHeaders();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
// Serve static files (css, js, images)
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

public partial class Program
{
}
