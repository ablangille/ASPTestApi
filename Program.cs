using TestApi.Data;
using TestApi.Interface;
using TestApi.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// user secrets
// see https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows
var dbConnectionString = builder.Configuration["Database:ConnectionString"];

// injecting database to services
// Dependency injection
// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-6.0
// Migrations
// https://www.entityframeworktutorial.net/efcore/entity-framework-core-migration.aspx
builder.Services.AddDbContext<TestApiDbContext>(options => options.UseNpgsql(@dbConnectionString));

// link and add repository & interface to services
builder.Services.AddTransient<IUsers, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
