using TaskTracker.Infrastructure;
using TaskTracker.WebAPI.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices(builder.Configuration);

//builder.Services.AddDbContext<TrackerDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("TrackerConnection")));
//builder.Services.AddScoped<ITrackerDbContext>(provider => provider.GetRequiredService<TrackerDbContext>());

//builder.Services.AddIdentity<User, IdentityRole>(options =>
//    {
//        options.Password.RequireUppercase = false; 
//        options.Password.RequireLowercase = false; 
//        options.Password.RequireNonAlphanumeric = false;
//        options.Password.RequireDigit = false;
//        options.Password.RequiredLength = 8;
//    })
//    .AddEntityFrameworkStores<TrackerDbContext>();

//builder.Services.AddAuthentication(options =>
//    {
//        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    })
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            RequireExpirationTime = true,
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
//            ValidAudience = builder.Configuration["JwtSettings:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(
//                System.Text.Encoding.UTF8.GetBytes(
//                    builder.Configuration["JwtSettings:SecurityKey"]))
//        };
//    });

//builder.Services.AddControllers()
//    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "Task Tracker",
//        Version = "v1"
//    });
//    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        In = ParameterLocation.Header,
//        Description = "Please enter a valid token",
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        BearerFormat = "JWT",
//        Scheme = "Bearer"
//    });
//    options.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});
//builder.Services.AddScoped<SeedData>();
//builder.Services.AddScoped<JwtHandlerService>();
//builder.Services.AddScoped<IAccountService, AccountService>();

//builder.Services.AddCors(options => options.AddPolicy(name: "ConfiguredPolicy",
//    cfg =>
//    {
//        cfg.AllowAnyHeader();
//        cfg.AllowAnyMethod();
//        cfg.WithOrigins(builder.Configuration["AllowedCORS"]);
//    }
//    ));

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