using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Service.Data;
using Disaster_Relief_Management_System.Services.Interfaces;
using Disaster_Relief_Management_System.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Disaster Relief API", Version = "v1" });
});

// Read connection string from configuration (appsettings, user-secrets, env vars)
var conn = builder.Configuration.GetConnectionString("GiftOfTheGivers");
builder.Services.AddDbContext<GiftDbContext>(options => options.UseSqlServer(conn));

// Register service
builder.Services.AddScoped<IReliefService, ReliefService>();

var app = builder.Build();

// Enable middleware
app.UseHttpsRedirection();

app.UseAuthorization();

// Enable Swagger and serve UI at application root
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Disaster Relief API V1");
    c.RoutePrefix = string.Empty; // Serve Swagger UI at root
});

// Redirect root to swagger UI (safety)
app.MapGet("/", (Microsoft.AspNetCore.Http.HttpContext ctx) =>
{
    ctx.Response.Redirect("/swagger/index.html");
    return System.Threading.Tasks.Task.CompletedTask;
});

app.MapControllers();

app.Run();

