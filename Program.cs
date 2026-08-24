using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using DotNetEnv;
using RestaurantReservation.API.Endpoints;
using RestaurantReservation.API.Services;
using RestaurantReservation.API.Services.Auth;
using RestaurantReservation.API.Services.Employees;
using RestaurantReservation.API.Services.Reservations;
using RestaurantReservation.Db.Context;
using RestaurantReservation.Db.Repositories;

Env.Load();

var builder = WebApplication.CreateBuilder(System.Environment.GetCommandLineArgs());

builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
    });
});

builder.Services.AddDbContext<RestaurantReservationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();

builder.Services.AddMemoryCache();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,      
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = ""
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IRevokedTokenRepository, RevokedTokenRepository>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuth, AuthService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var secretKey = Environment.GetEnvironmentVariable("JWT_KEY") 
                        ?? throw new InvalidOperationException("JWT_KEY missing.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            RoleClaimType = ClaimTypes.Role 
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
                var dbContext = context.HttpContext.RequestServices.GetRequiredService<RestaurantReservationDbContext>();

                var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                if (!string.IsNullOrEmpty(jti) && cache.TryGetValue($"revoked_{jti}", out _))
                {
                    context.Fail("This token has been revoked.");
                    return;
                }

                var userIdStr = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                             ?? context.Principal?.FindFirst("sub")?.Value;

                if (int.TryParse(userIdStr, out var userId))
                {
                    var employee = await dbContext.Employees.FindAsync(userId);
                    var tokenIssueTime = context.SecurityToken.ValidFrom;

                    if (employee?.TokensValidFrom != null && tokenIssueTime < employee.TokensValidFrom)
                    {
                        context.Fail("Token invalidated due to global logout.");
                        return;
                    }
                }
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"JWT rejected: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine($"JWT challenge: {context.Error}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // gRPC Reflection
    app.MapGrpcReflectionService();
}

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapReservationEndpoints();
app.MapEmployeeEndpoints();

app.MapGrpcService<ReservationGrpcService>();

app.Run();