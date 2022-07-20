using TestApi.Data;
using TestApi.Services;
using TestApi.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    //This is to generate the Default UI of Swagger Documentation
    swagger.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Version = "v1",
            Title = "ASP.NET Core 6 Web API",
            Description = "Test API with JWT Authentication"
        }
    );
    // To Enable authorization using Swagger (JWT)
    swagger.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
        }
    );
    swagger.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        }
    );
});

// ENV Variables
var dbConnectionString = Environment.GetEnvironmentVariable("TestApi_ConnectionString");
var jwtIssuer = Environment.GetEnvironmentVariable("TestApi_JWT_ISSUER");
var jwtAudience = Environment.GetEnvironmentVariable("TestApi_JWT_AUDIENCE");
var jwtSubject = Environment.GetEnvironmentVariable("TestApi_JWT_SUBJECT");
var jwtKey = Environment.GetEnvironmentVariable("TestApi_JWT_KEY");

// injecting database to services
// Dependency injection
// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-6.0
// Migrations
// https://www.entityframeworktutorial.net/efcore/entity-framework-core-migration.aspx
builder.Services.AddDbContext<TestApiDbContext>(options => options.UseNpgsql(@dbConnectionString!));

// link and add repository & interface to services
builder.Services.AddTransient<IUserService, UserRepository>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidIssuer = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
        };
    });

var app = builder.Build();

// Development nice-to-haves
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    Console.WriteLine("DB Connection: " + dbConnectionString);
    Console.WriteLine("JWT Issuer: " + jwtIssuer);
    Console.WriteLine("JWT Audience: " + jwtAudience);
    Console.WriteLine("JWT Subject: " + jwtSubject);
    Console.WriteLine("JWT Key: " + jwtKey);
}

// Configure the HTTP request pipeline.
app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
