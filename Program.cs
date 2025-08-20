using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using RESTful.Context;
using RESTful.Middleware;
using RESTful.Service.Implementation;
using RESTful.Service.Interface;

var builder = WebApplication.CreateBuilder(args);

// Database config
var connectionString = builder.Configuration.GetConnectionString("RESTful_DB");

builder.Services.AddDbContext<BackendDBContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Add services to the container.
// Dependency Injection
builder.Services.AddScoped<IUserService, UserService>();

// Logging config
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middlerware exception handler
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();