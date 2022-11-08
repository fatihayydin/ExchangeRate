using ExchangeRate.Abstraction.Data;
using ExchangeRate.Api.Logging;
using ExchangeRate.Data.Data;
using ExchangeRate.Data.DataAccess;
using ExchangeRate.Data.Extensions;
using ExchangeRate.Infrastructure.Exceptions;
using ExchangeRate.Infrastructure.ExternalServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using Serilog;

//To make serilog as primary logging
Log.Logger = new LoggerConfiguration().CreateLogger();

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

AddSwagger(builder.Services);

string connectionString = builder.Configuration["ConnectionString"];

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddDataContext<ExchangeRateDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});


var seqServerUrl = builder.Configuration["Logging:SeqServerUrl"];

//Console Logging and seq logging enabled
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
    .WriteTo.Console()
    .MinimumLevel.Information());

builder.Services.AddSingleton<IExternalExchangeService>(sp => new ExternalExchangeService(builder.Configuration["Fixer:BaseUrl"], builder.Configuration["Fixer:ApiKey"], sp.GetService<ILogger<ExternalExchangeService>>()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<HttpLoggingMiddleware>();

app.UseExceptionMiddleware();
//Request Response logging with middleware

CreateDbIfNotExists(app);

app.Run();


static void CreateDbIfNotExists(IHost host)
{
    using (var scope = host.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ExchangeRateDbContext>();
            DbInitializer.Initialize(context);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogCritical(ex, "An error occurred creating the DB.");
        }
    }
}


static void AddSwagger(IServiceCollection serviceCollection){
    serviceCollection.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api Key Auth", Version = "v1" });
        c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
        {
            Description = "ApiKey must appear in header",
            Type = SecuritySchemeType.ApiKey,
            Name = "ApiKey",
            In = ParameterLocation.Header,
            Scheme = "ApiKeyScheme"
        });
        var key = new OpenApiSecurityScheme()
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "ApiKey"
            },
            In = ParameterLocation.Header
        };
        var requirement = new OpenApiSecurityRequirement
                    {
                             { key, new List<string>() }
                    };
        c.AddSecurityRequirement(requirement);
    });
}