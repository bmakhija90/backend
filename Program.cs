using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
               "https://localhost:4200"
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