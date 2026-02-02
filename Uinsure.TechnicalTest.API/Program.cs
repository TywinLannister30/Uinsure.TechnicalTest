using Asp.Versioning;
using Asp.Versioning.Routing;
using Serilog;
using Uinsure.TechnicalTest.API.Extensions;
using Uinsure.TechnicalTest.Application.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Configuration"));
builder.Configuration.AddJsonFile("ConnectionStrings.json");
builder.Configuration.AddJsonFile("ApplicationSettings.json");
builder.Configuration.AddJsonFile("LogSettings.json");
builder.Configuration.AddJsonFile($"LogSettings.{builder.Environment.EnvironmentName}.json");
builder.Configuration.AddEnvironmentVariables();

builder.Host.UseSerilog((context, services, loggerConfig) =>
{
    loggerConfig
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});

var connectionString = builder.Configuration.GetConnectionString("PolicyContext");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDatabaseContext(connectionString);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();
builder.Services.Configure<PolicySettings>(builder.Configuration.GetSection("PolicySettings"));

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.Configure<RouteOptions>(options =>
{
    options.ConstraintMap["apiVersion"] = typeof(ApiVersionRouteConstraint);
});

builder.Services.AddOtlp("uinsure", builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
