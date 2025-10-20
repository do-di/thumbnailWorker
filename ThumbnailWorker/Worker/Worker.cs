using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using ThumbnailWorker.Config;
using ThumbnailWorker.Infrastructure.Interface;
using ThumbnailWorker.Worker.Interface;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using ThumbnailWorker.Model;

namespace ThumbnailWorker.Worker
{
    public class Worker : BackgroundService
    {
        private readonly IReadOnlyDictionary<string, IThumbnailStrategy> _thumnailStreategyDic;
        private readonly RabbitMQConfig _rabbitMQConfig;
        private readonly ILogger<Worker> _logger;

        public Worker(IReadOnlyDictionary<string, IThumbnailStrategy> thumnailStreategyDics, IOptions<RabbitMQConfig> option, 
            ILogger<Worker> logger)
        {
            _thumnailStreategyDic = thumnailStreategyDics;
            _rabbitMQConfig = option.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMQConfig.Host,
                UserName = _rabbitMQConfig.Username,
                Password = _rabbitMQConfig.Password,
            };
            var connection = await factory.CreateConnectionAsync().ConfigureAwait(false);
            var channel = await connection.CreateChannelAsync().ConfigureAwait(false);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var queueJob = JsonSerializer.Deserialize<QueueJob>(message);
                if (queueJob == null)
                {
                    throw new Exception("something went wrong");
                }

                var stategy = _thumnailStreategyDic[queueJob.Type.ToLower()];
                await stategy.CreateThumbnailAsync(queueJob.Payload).ConfigureAwait(false);
            };
            await channel.BasicConsumeAsync("ctl_thumbnail", true, consumer: consumer);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
