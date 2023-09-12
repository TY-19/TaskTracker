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

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("ConfiguredPolicy");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();