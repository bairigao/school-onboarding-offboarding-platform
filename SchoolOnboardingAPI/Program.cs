using SchoolOnboardingAPI.Data;
using SchoolOnboardingAPI.Integrations.SnipeIT;
using SchoolOnboardingAPI.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for structured logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Add Swagger/Swashbuckle for API documentation
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "School Onboarding API", Version = "v1" });
    
    // Include XML documentation in Swagger
    var xmlFile = "SchoolOnboardingAPI.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Add Entity Framework Core with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

// Add AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add Snipe-IT Integration
builder.Services.AddHttpClient<ISnipeITClient, SnipeITClient>();
builder.Services.AddScoped<IAssetService, AssetService>();

// Add CORS if needed for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Enable Swagger in development
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "School Onboarding API v1");
        options.RoutePrefix = "swagger"; // Accessible at /swagger
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowAll");

// Map controller routes
app.MapControllers();

app.Run();
