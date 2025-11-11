
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton<JwtService>();

/*
// Add CORS services
builder.Services.AddCors(options =>
{
    // For production, add your production Angular app URL
    options.AddPolicy("AllowProduction", policy =>
    {
        policy.WithOrigins(
               "http://localhost:4200",    
               "https://localhost:4200",
                "https://admin.kirtilondon.co.uk",      // Your Angular app
                "https://www.admin.kirtilondon.co.uk"  // WWW version    
           )
           .AllowAnyHeader()
           .AllowAnyMethod()
           .AllowCredentials();  // This works with specific origins
    });
});
*/


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
//app.UseCors("AllowProduction");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();