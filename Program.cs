using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RESTful.Auth;
using RESTful.Auth.Interface;
using RESTful.Context;
using RESTful.Entity;
using RESTful.Entity.Auth;
using RESTful.Middleware;
using RESTful.Options;
using RESTful.Service.Implementation;
using RESTful.Service.Interface;

var builder = WebApplication.CreateBuilder(args);

// Database config
var connectionString = builder.Configuration.GetConnectionString("RESTful_DB");

// Backend database
builder.Services.AddDbContext<BackendDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Auth database
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AuthDb")));

// JWT options
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
builder.Services.AddScoped<IUserService, UserService>();

// CORS configuration
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(corsOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// JWT Authentication
var jwt = builder.Configuration.GetSection("Jwt");
var keyBytes =
    Encoding.UTF8.GetBytes(jwt["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment(); // in Dev ok, Prod to true
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RESTful API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header. Example: 'eyJhbGciOiJIUzI1NiIsInR5cCI6Ik...' (ohne 'Bearer')",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    // Security for Swagger
    c.AddSecurityDefinition("Bearer", securityScheme);

    // Securtiy Requirement for Swagger
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

// Logging config
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole(UserRole.Admin.ToString()));
    options.AddPolicy("Moderator", p => p.RequireRole(UserRole.Admin.ToString(), UserRole.Moderator.ToString()));
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Caching configuration
var cacheOptions = builder.Configuration.GetSection("Cache");

builder.Services.Configure<CacheOptions>(cacheOptions);

builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = cacheOptions.GetValue<long>("SizeLimit", 10_000); // Max cache size
});

var app = builder.Build();

// Middlerware exception handler
app.UseMiddleware<ExceptionMiddleware>();

// CORS configuration
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();