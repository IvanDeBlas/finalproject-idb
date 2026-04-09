using System.Text;
using System.Threading.RateLimiting;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WePlayRises.BuildingBlocks.Caching.Extensions;
using WePlayRises.Crowdfunding.Infra;
using WePlayRises.Core.Infra;
using WePlayRises.Core.Infra.Context;
using WePlayRises.Crowdfunding.Infra.Context;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Templates.Queries;
using WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Commands;
using WePlayRises.Crowdpromotion.Infra;
using WePlayRises.Crowdpromotion.Infra.Context;
using WePlayRises.Crowdsourcing.Infra;
using WePlayRises.Crowdsourcing.Infra.Context;
using WePlayRises.UserAccess.Application.Features.Auth.Commands;
using WePlayRises.UserAccess.Application.Mapping;
using WePlayRises.UserAccess.Domain.Constants;
using WePlayRises.UserAccess.Infra;
using WePlayRises.UserAccess.Infra.Context;
using WePlayRises.BuildingBlocks.Kernel.Http.Filters;
using WePlayRises.BuildingBlocks.Kernel.Services;
using WePlayRises.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<HttpExceptionFilter>();
});
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WePlay Rises API",
        Version = "v1",
        Description = "API for WePlay Rises - Musical Crowdfunding Platform"
    });

    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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

// DbContexts
builder.Services.AddDbContext<UserAccessContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<CrowdfundingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<CoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<CrowdsourcingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<CrowdpromotionContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<UserAccessContext>()
.AddDefaultTokenProviders();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "WePlayRisesDefaultSecretKey123456789";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "WePlayRises";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "WePlayRisesUsers";

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
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Application assemblies (one per module)
var applicationAssemblies = new[]
{
    typeof(RegisterCommand).Assembly,        // UserAccess.Application
    typeof(CreateCampaniaCommand).Assembly,  // Crowdfunding.Application
    typeof(GetPlantillasProyectoQuery).Assembly, // Crowdsourcing.Application
    typeof(SolicitarInscripcionCommand).Assembly // Crowdpromotion.Application
};

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(applicationAssemblies));

// FluentValidation
builder.Services.AddValidatorsFromAssemblies(applicationAssemblies);

// AutoMapper
builder.Services.AddAutoMapper(applicationAssemblies);

// Caching
builder.Services.AddMemoryCacheProvider(builder.Configuration);
builder.Services.AddRequestCacheService();

// Current user service
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<UserAccessContext>("useraccess-db")
    .AddDbContextCheck<CrowdfundingContext>("crowdfunding-db")
    .AddDbContextCheck<CoreContext>("core-db")
    .AddDbContextCheck<CrowdsourcingContext>("crowdsourcing-db")
    .AddDbContextCheck<CrowdpromotionContext>("crowdpromotion-db");

// Response Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("public-api", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    options.AddPolicy("public-mutation", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

// Register module services
builder.Services.AddUserAccessServices();
builder.Services.AddCrowdfundingServices();
builder.Services.AddCoreServices();
builder.Services.AddCrowdsourcingServices();
builder.Services.AddCrowdpromotionServices();

// CORS - read allowed origins from configuration, fallback to localhost for dev
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:3000", "http://localhost:3001" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Apply pending migrations and seed data (Docker/container/Azure environments)
if (app.Configuration.GetValue<bool>("ApplyMigrations"))
{
    using var migrationScope = app.Services.CreateScope();
    app.Logger.LogInformation("Applying pending migrations...");

    var userAccessDb = migrationScope.ServiceProvider.GetRequiredService<UserAccessContext>();
    await userAccessDb.Database.MigrateAsync();
    app.Logger.LogInformation("UserAccess migrations applied");

    var crowdfundingDb = migrationScope.ServiceProvider.GetRequiredService<CrowdfundingContext>();
    await crowdfundingDb.Database.MigrateAsync();
    app.Logger.LogInformation("Crowdfunding migrations applied");

    var coreDb = migrationScope.ServiceProvider.GetRequiredService<CoreContext>();
    await coreDb.Database.MigrateAsync();
    app.Logger.LogInformation("Core migrations applied");

    var crowdsourcingDb = migrationScope.ServiceProvider.GetRequiredService<CrowdsourcingContext>();
    await crowdsourcingDb.Database.MigrateAsync();
    app.Logger.LogInformation("Crowdsourcing migrations applied");

    var crowdpromotionDb = migrationScope.ServiceProvider.GetRequiredService<CrowdpromotionContext>();
    await crowdpromotionDb.Database.EnsureCreatedAsync();
    app.Logger.LogInformation("Crowdpromotion database ensured");

    // Seed roles (after migrations ensure tables exist)
    var roleManager = migrationScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in Roles.All)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
            app.Logger.LogInformation("Role {Role} created", role);
        }
    }
    app.Logger.LogInformation("Roles seeded");
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseStatusCodePages();

// Swagger enabled in all environments for demo/evaluation purposes
app.UseSwagger();
app.UseSwaggerUI();

app.UseResponseCompression();
app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
