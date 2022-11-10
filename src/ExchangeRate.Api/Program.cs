using ExchangeRate.Abstraction.Data;
using ExchangeRate.Api.Auth;
using ExchangeRate.Api.Logging;
using ExchangeRate.Application.Services;
using ExchangeRate.Data.Data;
using ExchangeRate.Data.DataAccess;
using ExchangeRate.Data.Extensions;
using ExchangeRate.Infrastructure.Caching;
using ExchangeRate.Infrastructure.Exceptions;
using ExchangeRate.Infrastructure.ExternalServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Polly;
using Serilog;

//To make serilog as primary logging
Log.Logger = new LoggerConfiguration().CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers( config =>
{
    config.Filters.Add(new LimitActionFilter());
});

AddSwagger(builder);

RegisterDataServices(builder);

AddSeriLog(builder);

AddPolly(builder);

RegisterExchangeServices(builder);

AddCacheServices(builder);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

#region Middlewares
//Request Response logging with middleware
app.UseMiddleware<HttpLoggingMiddleware>();

app.UseExceptionMiddleware();
#endregion

CreateDbIfNotExists(app);

app.Run();

static void AddCacheServices(WebApplicationBuilder builder)
{
    builder.Services.AddSingleton(sp => new RedisServer(builder.Configuration["Redis:Hosts"], builder.Configuration["Redis:Password"]));
    builder.Services.AddSingleton<ICacheService, RedisCacheService>();

    builder.Services.AddMemoryCache();
}

static void AddPolly(WebApplicationBuilder builder)
{
    var httpCircuitBreakerPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
    .CircuitBreakerAsync(2, TimeSpan.FromSeconds(300), (ex, t) => Log.Logger.Fatal("Circuit Breaker is opened", ex), () => Log.Logger.Fatal("Circuit breaker closed!"));

    builder.Services.AddSingleton<IAsyncPolicy<HttpResponseMessage>>(httpCircuitBreakerPolicy);
}

static void RegisterExchangeServices(WebApplicationBuilder builder)
{
    //Todo: Make registration for AppSettings. SO that no need for the constructor seperately.
    builder.Services.AddSingleton<IExternalExchangeService>(sp => new ExternalExchangeService(builder.Configuration["Fixer:BaseUrl"], builder.Configuration["Fixer:ApiKey"],
        sp.GetService<IAsyncPolicy<HttpResponseMessage>>(), sp.GetService<ILogger<ExternalExchangeService>>()));

    builder.Services.AddSingleton<IExchangeService, ExchangeService>();
}

static void RegisterDataServices(WebApplicationBuilder builder)
{
    string connectionString = builder.Configuration["ConnectionString"];

    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

    builder.Services.AddDataContext<ExchangeRateDbContext>(options =>
    {
        options.UseSqlServer(connectionString);
    });
}

static void AddSeriLog(WebApplicationBuilder builder)
{
    var seqServerUrl = builder.Configuration["Logging:SeqServerUrl"];

    //Console Logging and seq logging enabled
    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
        .WriteTo.Console()
        .MinimumLevel.Information());
}

static void AddSwagger(WebApplicationBuilder builder)
{
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(c =>
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