using DotnetDynamicExpressionsDemo.Data;
using DotnetDynamicExpressionsDemo.Queries;
using DotnetDynamicExpressionsDemo.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services
    .AddDbContext<AppDbContext>(opts =>
        opts.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString)
        )
    );

// QueryBuilder
builder.Services.AddTransient<IQueryBuilder, QueryBuilder>();

// QueryService
builder.Services.AddScoped<IQueryService, QueryService>();

// Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Controllers
app.MapControllers();

app.Run();
