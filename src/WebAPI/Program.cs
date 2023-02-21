using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Domain.Entities;
using TaskTracker.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TrackerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TrackerConnection")));
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<TrackerDbContext>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<SeedData>();

builder.Services.AddCors(options => options.AddPolicy(name: "ConfiguredPolicy",
    cfg =>
    {
        cfg.AllowAnyHeader();
        cfg.AllowAnyMethod();
        cfg.WithOrigins(builder.Configuration["AllowedCORS"]);
    }
    ));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<SeedData>();
    await seeder.SeedDefaultRolesAndUsers();
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ConfiguredPolicy");
app.UseHttpsRedirection();

app.MapGet("/api/test", () => new { Response = "The server has returned result" });

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();