using System.Globalization;
using Infrastructure.Settings;
using Infrastructure.Extensions;
using NLog.Extensions.Logging;
using NLog.Web;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration
    .AddJsonFile("appsettings.nlog.json")
    .AddEnvironmentVariables();

builder.Host.UseNLog();

NLog.LogManager.Configuration = new NLogLoggingConfiguration(builder.Configuration.GetSection("NLog"));

builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.Configure<KafkaTopics>(builder.Configuration.GetSection(nameof(KafkaTopics)));
builder.Services.Configure<SpreadCalculationJobSettings>(builder.Configuration.GetSection(nameof(SpreadCalculationJobSettings)));

builder.Services.AddDal(builder.Configuration);
builder.Services.AddExternalServices();
builder.Services.AddAppServices();
builder.Services.AddKafka(builder.Configuration);
builder.Services.AddBackgroundServices();
builder.Services.AddQuartzJobs(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();