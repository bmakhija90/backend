using EcommerceAPI.Repositories;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton<JwtService>();

// Add CORS services for Development only
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentCors", policy =>  // Renamed for clarity
    {
        policy.WithOrigins(
               "http://localhost:4200",
               "https://localhost:4200",
               "http://localhost:3000"
           )
           .AllowAnyHeader()
           .AllowAnyMethod()
           .AllowCredentials();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secret = builder.Configuration["Jwt:Secret"] ?? "super-secret-key-must-be-32-chars!";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "EcommerceApi",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "EcommerceClients",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Configure MongoDB
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDB");
    return new MongoClient(connectionString);
});

builder.Services.AddScoped(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<IMongoClient>();
    var databaseName = builder.Configuration["MongoDB:DatabaseName"];
    return client.GetDatabase(databaseName);
});

// Register repositories and services
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IProfileService, ProfileService>();


var app = builder.Build();

// CORS only in Development - Nginx handles it in Production
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevelopmentCors");  // Updated policy name
    Console.WriteLine(" CORS enabled for Development environment");
}
else
{
    Console.WriteLine(" CORS disabled - Nginx handles CORS in Production");
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();