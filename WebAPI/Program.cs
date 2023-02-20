using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => options.AddPolicy(name: "ConfiguredPolicy",
    cfg =>
    {
        cfg.AllowAnyHeader();
        cfg.AllowAnyMethod();
        cfg.WithOrigins(builder.Configuration["AllowedCORS"]);
    }
    ));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ConfiguredPolicy");
app.UseHttpsRedirection();

app.MapGet("/api/test", () => new { Response = "The server return result"});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();