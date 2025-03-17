using Infrastructure.Core.Entities;
using Infrastructure.Repositories;
using IT_Automation.API;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);


// Build the app
var app = builder.Build();

app.UseAuthentication(); // Optional
app.UseAuthorization();  // Optional

app.MapControllers();

// call Seeder
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    await Seeder.SeedRolesAndAdminAsync(userManager, roleManager);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/DashboardApp/swagger.json", "Dashboard App API");
        c.SwaggerEndpoint("/swagger/UserApp/swagger.json", "User App API");
    });
}

app.Run();