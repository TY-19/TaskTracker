using TaskTracker.Infrastructure;
using TaskTracker.WebAPI.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<SeedData>();
    await seeder.SeedDefaultRolesAndUsersAsync();
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