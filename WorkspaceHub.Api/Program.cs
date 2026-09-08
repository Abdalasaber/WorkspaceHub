using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using WorkspaceHub.Api.Middleware;
using WorkspaceHub.Application.DependencyInjection;
using WorkspaceHub.Infrastructure.Extensions;
using WorkspaceHub.Infrastructure.Persistence.Seed;

namespace WorkspaceHub.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "WorkspaceHub API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization", Type = SecuritySchemeType.Http, Scheme = "Bearer", BearerFormat = "JWT",
                In = ParameterLocation.Header, Description = "Enter: Bearer {your token}"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
            });
        });

        builder.Services.AddApplication().AddInfrastructure(builder.Configuration);
        var frontendOrigin = builder.Configuration["Frontend:Origin"];
        if (!string.IsNullOrWhiteSpace(frontendOrigin))
        {
            builder.Services.AddCors(options => options.AddPolicy("Frontend", policy =>
                policy.WithOrigins(frontendOrigin).AllowAnyHeader().AllowAnyMethod()));
        }

        var app = builder.Build();
        app.UseMiddleware<GlobalExceptionMiddleware>();

        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await IdentitySeeder.SeedRolesAsync(roleManager);
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        if (!string.IsNullOrWhiteSpace(frontendOrigin)) app.UseCors("Frontend");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
