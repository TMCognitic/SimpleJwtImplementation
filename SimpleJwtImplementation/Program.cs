/*
 * Activer le jwt token
 * 
 * 1. Ajouter le package nuget : Microsoft.AspNetCore.Authentication.JwtBearer
 * 2. Ajouter la génération du Token
 * 3. Ajouter de la sécurité du Token au niveau de l'api (Program.cs & [Authorize])
 * 
 *  
 */


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using SimpleJwtImplementation.Infrastructrure;
using SimpleJwtImplementation.Models;
using SimpleJwtImplementation.Workers;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddHostedService<Worker>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IList<User>>(sp => new List<User>()
{
    new User (1, "Doe", "Jane", "jane.doe@test.be", "Admin"),
    new User (2, "Doe", "John", "john.doe@test.be", "User")
});

//3.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://localhost:7079",
            ValidAudience = "https://localhost:7079",
            ClockSkew = TimeSpan.FromMinutes(0),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes("MaSuperCléPrivéeDeLaMortQuiTueOuPas!!!"))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
