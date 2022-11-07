using ExchangeRate.Application.ExternalServices;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var seqServerUrl = builder.Configuration["Logging:SeqServerUrl"];

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
    .WriteTo.Console()
    .MinimumLevel.Information());

builder.Services.AddScoped<IExternalExchangeService>(sp => new ExternalExchangeService(builder.Configuration["Fixer:BaseUrl"], builder.Configuration["Fixer:ApiKey"], sp.GetService<ILogger<ExternalExchangeService>>()));

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

app.Run();
