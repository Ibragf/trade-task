using Infrastructure.Settings;
using NLog.Extensions.Logging;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration
    .AddJsonFile("appsettings.nlog.json")
    .AddEnvironmentVariables();

builder.Logging.ClearProviders();
builder.Host.UseNLog();

NLog.LogManager.Configuration = new NLogLoggingConfiguration(builder.Configuration.GetSection("NLog"));

builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.Configure<KafkaTopics>(builder.Configuration.GetSection(nameof(KafkaTopics)));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();