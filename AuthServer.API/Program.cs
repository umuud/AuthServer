using System.Text;
using AuthServer.Application.Services;
using AuthServer.Core.Entities;
using AuthServer.Core.Interfaces;
using AuthServer.Infrastructure.Data;
using AuthServer.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Debugging;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

// 0) Serilog
SelfLog.Enable(msg => Console.Error.WriteLine($"[Serilog Error] {msg}"));
builder.Host.UseSerilog((ctx, lc) =>
{
    lc
      .WriteTo.Console()
      .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
      {
          AutoRegisterTemplate           = true,
          AutoRegisterTemplateVersion    = AutoRegisterTemplateVersion.ESv8, // ES8 için
          IndexFormat                    = "authserver-logs-{0:yyyy.MM.dd}",
          TypeName                       = null,                            // _type’ı tamamen kaldır
          EmitEventFailure               = EmitEventFailureHandling.RaiseCallback,
          FailureCallback                = e => Console.Error.WriteLine($"[ES Error] {e.MessageTemplate}")
      })
      .ReadFrom.Configuration(ctx.Configuration);
});

// 1) DbContext
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2) Identity
builder.Services
    .AddIdentity<User, IdentityRole<Guid>>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 3) JWT
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// 4) DI
builder.Services
    .AddScoped<IUserRepository, UserRepository>()
    .AddScoped<IPasswordHasher, AspNetCorePasswordHasher>()
    .AddScoped<ITokenService, JwtTokenService>()
    .AddScoped<IAuthService, AuthService>();

// 5) MVC + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title   = "AuthServer API",
        Version = "v1"
    });
});

var app = builder.Build();

// --- PIPELINE ---

// A) Logging
app.UseSerilogRequestLogging();

// B) Swagger (hiçbir koşula bağlı değil)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthServer API v1");
        // RoutePrefix boş bırakılırsa Swagger kök URL'de çalışır
        // c.RoutePrefix = string.Empty;
    });
}
// C) Routing + Auth
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// D) Controllers
app.MapControllers();

app.Run();
