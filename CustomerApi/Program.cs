using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CustomerApi.Data;
using System.IO;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Ensure DB folder exists and move existing DB files to new location
var contentRoot = builder.Environment.ContentRootPath;
var dbFolder = Path.Combine(contentRoot, "DB");
Directory.CreateDirectory(dbFolder);

foreach (var name in new[] { "customerinfo.db", "customerinfo.db-wal", "customerinfo.db-shm" })
{
    var oldPath = Path.Combine(contentRoot, name);
    var newPath = Path.Combine(dbFolder, name);
    if (File.Exists(oldPath) && !File.Exists(newPath))
    {
        File.Move(oldPath, newPath);
    }
}

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure JWT Authentication
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
var key = Encoding.ASCII.GetBytes(jwtSecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = "CustomerApi",
        ValidateAudience = true,
        ValidAudience = "CustomerApiUsers",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Configure DbContext for SQLite
// Build absolute path for SQLite file so EF uses the DB in the `DB` folder
var configured = builder.Configuration.GetConnectionString("Default") ?? "Data Source=DB/customerinfo.db";
string sqliteConnectionString;
if (configured.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
{
    var relative = configured.Substring("Data Source=".Length).Trim();
    // If relative path, make it absolute under content root
    if (!Path.IsPathRooted(relative))
    {
        // If the configured path is relative (for example "DB/customerinfo.db"),
        // make it absolute relative to the application's content root so we don't
        // accidentally produce paths like ".../DB/DB/customerinfo.db".
        sqliteConnectionString = $"Data Source={Path.Combine(contentRoot, relative)}";
    }
    else
    {
        sqliteConnectionString = configured;
    }
}
else
{
    // fallback
    sqliteConnectionString = $"Data Source={Path.Combine(dbFolder, "customerinfo.db")}";
}

builder.Services.AddDbContext<CustomerContext>(options =>
    options.UseSqlite(sqliteConnectionString)
);

// Register application services
builder.Services.AddScoped<CustomerApi.Services.IRegisterService, CustomerApi.Services.RegisterService>();
builder.Services.AddScoped<CustomerApi.Services.ILoginService, CustomerApi.Services.LoginService>();
builder.Services.AddScoped<CustomerApi.Services.IJwtTokenService, CustomerApi.Services.JwtTokenService>();

var app = builder.Build();

// Apply EF Core migrations at startup (if any)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CustomerContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
