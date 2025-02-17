using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Wine.API.Middlewares;
using Wine.API.Validators;
using Wine.Application.Dtos;
using Wine.Application.Mappers;
using Wine.Application.Services;
using Wine.Application.Services.Contracts;
using Wine.Domain.Contracts;
using Wine.Infra.Persistence;
using Wine.Infra.Repositories;

namespace Wine.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        
        builder.Services.AddAuthorization(); 
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
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
                };
            });

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
                 options.UseSqlite("Data Source=winecollection.db"));
        builder.Services.AddCors(opt =>
        {
            opt.AddPolicy(name: "Policy",
                builder =>
                {
                    builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowAnyOrigin();
                });
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Wine Collection API", Version = "v1" });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, Array.Empty<string>() }
            });
        });
        builder.Services.AddControllers();
        
        builder.Services.AddScoped<IWineRepository, WineRepository>();
        builder.Services.AddScoped<IWineService, WineService>();
        builder.Services.AddScoped<IValidator<WineDto>, WineValidator>();
        builder.Services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<WineProfile>();
        });
        var app = builder.Build();
       
        app.UseMiddleware<ExceptionMiddleware>();
        // Configure the HTTP request pipeline.
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("Policy");

        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
        
        app.Run();
    }
}