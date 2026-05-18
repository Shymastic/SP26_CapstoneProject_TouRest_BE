using DotNetEnv;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PayOS;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using TouRest.Api.Extensions;
using TouRest.Api.Hubs;
using TouRest.Api.Middlewares;
using TouRest.Application;
using TouRest.Application.Common.Constants;
using TouRest.Application.Interfaces;
using TouRest.Application.Mappings;
using TouRest.Application.Services;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure;
using TouRest.Infrastructure.Persistence;
using TouRest.Infrastructure.Repositories;

Env.TraversePath().Load();
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MinRequestBodyDataRate = null;
});
// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("DATABASE_CONNECTION")));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.FormatterName = "simple";
});
builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
    options.SingleLine = true;
});

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddAutoMapper(
    typeof(UserProfile).Assembly
);

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = AuthConstants.JwtIssuer,
        ValidAudience = AuthConstants.JwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(AuthConstants.JwtSecret)),
        RoleClaimType = ClaimTypes.Role
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AgencyOrAdmin", policy => policy.RequireRole(RoleCodes.Admin, RoleCodes.Agency));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(RoleCodes.Admin));
    options.AddPolicy("AgencyOnly", policy => policy.RequireRole(RoleCodes.Agency));
    options.AddPolicy("ProviderOnly", policy => policy.RequireRole(RoleCodes.Provider));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole(RoleCodes.Customer));
    options.AddPolicy("ProviderOrAdmin", policy => policy.RequireRole(RoleCodes.Provider, RoleCodes.Admin));
});
builder.Services.AddApiServices();
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(Environment.GetEnvironmentVariable("DATABASE_CONNECTION")));
builder.Services.AddHangfireServer();
builder.Services.AddSignalR();
var emailSettings = new EmailSettings
{
    Host = Environment.GetEnvironmentVariable("EMAIL_HOST")!,
    Port = int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT")!),
    Username = Environment.GetEnvironmentVariable("EMAIL_USERNAME")!,
    Password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD")!,
    FromName = Environment.GetEnvironmentVariable("EMAIL_FROM_NAME")!
};
var payOS = new PayOSClient(
    Environment.GetEnvironmentVariable("PAYOS_CLIENT_ID")!,
    Environment.GetEnvironmentVariable("PAYOS_API_KEY")!,
    Environment.GetEnvironmentVariable("PAYOS_CHECKSUM_KEY")!
);
builder.Services.AddSingleton(payOS);
builder.Services.AddSingleton(emailSettings);
var app = builder.Build();
app.UseMiddleware<GlobalExceptionHandler>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<AppHub>("/appHub");

app.Run();
