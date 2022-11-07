using ExchangeRate.Application.ExternalServices;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IExternalExchangeService>(sp => new ExternalExchangeService("https://api.apilayer.com", "gH771U2gKZDOgj4Sltcda78YCYktZiz7", sp.GetService<ILogger<ExternalExchangeService>>()));

var seqServerUrl = builder.Configuration["Logging:SeqServerUrl"];

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
    .WriteTo.Console()
    .MinimumLevel.Information());

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
