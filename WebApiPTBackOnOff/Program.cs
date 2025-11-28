using ApplicationCore.Interfaces.Auth;
using ApplicationCore.Interfaces.Task;
using Infrastructure.Data;
using Infrastructure.Services.Auth;
using Infrastructure.Services.Task;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// OpenApi ahora viene por defecto en .net9 para emplear swagger se tiene que llamar.
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Conexión a la Base de Datos SQL Server
var connectionSqlServerString = builder.Configuration.GetConnectionString("SQLServerConnection");

builder.Services.AddDbContext<DbContextPTOnOff>(options =>
    options.UseSqlServer(connectionSqlServerString, sqlServerOptions =>
    {
        sqlServerOptions.CommandTimeout(120);
    })
    .EnableSensitiveDataLogging()
);

//Conexión Front
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.WithOrigins("http://127.0.0.1:4200", "https://8ee1-190-68-49-101.ngrok-free.app", "http://localhost:4200")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());
});

// Autenticación
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero, // Para anular el desfase de tiempo de 5 min que tiene la expiración del token
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:Key"]))
        };
    });

builder.Services.AddTransient<ITokenGenerator, TokenGeneratorService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();

var app = builder.Build();

app.MapGet("/", (HttpContext context) =>
{
    context.Response.Redirect("/swagger/index.html", permanent: false);
}).ExcludeFromDescription();

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

app.UseCors("CorsPolicy");

app.Run();
