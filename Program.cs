using DotnetDynamicExpressionsDemo.Data;
using DotnetDynamicExpressionsDemo.Handlers;
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

// Services
builder.Services.AddScoped<IQueryService, QueryService>();
builder.Services.AddScoped<IUserService, UserService>();

// Controllers
builder.Services.AddControllers();

// HTTP Exceptions
builder.Services.AddExceptionHandler<HttpExceptionHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

// Controllers
app.MapControllers();

// HTTP Exceptions
app.UseExceptionHandler(_ => {});

app.Run();
