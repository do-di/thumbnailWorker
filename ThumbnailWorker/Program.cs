using ThumbnailWorker.Infrastructure;
using ThumbnailWorker.Worker;
using Microsoft.EntityFrameworkCore;
using ThumbnailWorker.Config;
using ThumbnailWorker.Infrastructure.Interface;
using ThumbnailWorker.Worker.Interface;
using ThumbnailWorker.Worker.ThumbnailStategy;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")
));
builder.Services.Configure<S3Config>(builder.Configuration.GetSection("S3Config"));
builder.Services.Configure<RabbitMQConfig>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<IStorageProvider, StorageProvider>();

builder.Services.AddSingleton<SealThumbnailStrategy>();
builder.Services.AddSingleton<ContainerGateStrategy>();
builder.Services.AddSingleton<IReadOnlyDictionary<string, IThumbnailStrategy>>(sp =>
{
    var strategies = new IThumbnailStrategy[]
    {
        sp.GetRequiredService<SealThumbnailStrategy>(),
        sp.GetRequiredService<ContainerGateStrategy>()
    };
    return strategies.ToDictionary(s => s.Type.ToLower(), s => s);
});
var host = builder.Build();
host.Run();
