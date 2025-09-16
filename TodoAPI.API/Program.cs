using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TodoAPI.Application;
using TodoAPI.Infrastructure;
using TodoAPI.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

try
{
    // Configure URLs from appsettings
    var urls = builder.Configuration["Urls"];
    if (!string.IsNullOrEmpty(urls))
    {
        builder.WebHost.UseUrls(urls.Split(';'));
    }

    // Add services to the container.
    builder.Services.AddControllers();

    // Add Infrastructure layer
    builder.Services.AddInfrastructure(builder.Configuration);

    // Add Application layer
    builder.Services.AddApplication();

    // JWT Configuration
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"] ?? throw new ArgumentNullException("JWT SecretKey not configured");

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

    builder.Services.AddAuthorization();

    // Configure Swagger from appsettings
    var swaggerConfig = builder.Configuration.GetSection("Swagger");
    var apiConfig = builder.Configuration.GetSection("ApiSettings");

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = swaggerConfig["Title"] ?? "Todo API",
            Version = swaggerConfig["Version"] ?? "v1",
            Description = swaggerConfig["Description"] ?? "A simple Todo API built with ASP.NET Core",
            Contact = new OpenApiContact
            {
                Name = swaggerConfig["ContactName"] ?? "Todo API Team",
                Email = swaggerConfig["ContactEmail"] ?? "support@todoapi.com"
            }
        });

        // Add JWT Authentication to Swagger
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
    });

    // Add CORS for development
    if (apiConfig.GetValue<bool>("EnableCors"))
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("DevelopmentPolicy", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });
    }

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    var enableSwagger = apiConfig.GetValue<bool>("EnableSwagger");

    if (app.Environment.IsDevelopment())
    {
        if (enableSwagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
                c.RoutePrefix = string.Empty; // Swagger at root URL
                c.DocumentTitle = "Todo API Documentation";
                c.DefaultModelsExpandDepth(-1); // Hide schemas section
            });
        }

        if (apiConfig.GetValue<bool>("EnableCors"))
        {
            app.UseCors("DevelopmentPolicy");
        }
    }

    app.UseHttpsRedirection();

    // Authentication & Authorization middleware
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // Ensure database is created and migrated
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            var context = services.GetRequiredService<TodoDbContext>();
            logger.LogInformation("Ensuring database is created and migrated...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migration completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw; // Re-throw to prevent app from starting with broken DB
        }
    }

    // Get configured URLs for display
    var baseUrl = apiConfig["BaseUrl"] ?? "https://localhost:7189";
    var httpUrl = "http://localhost:5050";

    Console.WriteLine("🚀 Todo API is starting...");
    Console.WriteLine($"📖 Swagger UI available at: {baseUrl}");
    Console.WriteLine($"🔗 Alternative HTTP URL: {httpUrl}");
    Console.WriteLine($"❤️ Health check at: {baseUrl}/api/test/health");
    Console.WriteLine("🔐 Authentication endpoints:");
    Console.WriteLine($"   POST {baseUrl}/api/auth/register");
    Console.WriteLine($"   POST {baseUrl}/api/auth/login");
    Console.WriteLine($"   GET  {baseUrl}/api/auth/me");

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Application failed to start: {ex.Message}");
    Console.WriteLine($"🔍 Stack trace: {ex.StackTrace}");
    throw;
}