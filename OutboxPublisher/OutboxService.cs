using Azure.Messaging.ServiceBus;
using System.Text;

namespace OutboxPublisher
{
    public class OutboxService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly OutboxRepository _repository;
        private readonly ILogger _logger;
        public OutboxService(ILogger logger, OutboxRepository repository, IConfiguration configuration)
        {
            _logger = logger;
            _repository = repository;
            _configuration = configuration; 
        }

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    IEnumerable<OutboxMessage> messages = _repository.OutboxMessages.Where(e=>e.Processed!=true).OrderBy(o=>o.Id).ToList();

                    foreach(var message in messages)
                    {
                        SendMessageAsync(message);
                        SetMessageAsProcessed(message);
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError($"Error occurred in sending message: {ex.Message}");
                }
                finally
                {
                    await Task.Delay(5000, cancellationToken);
                }
            }
        }
        public async Task SetMessageAsProcessed(OutboxMessage outboxMessage)
        {
            try 
            {
                outboxMessage.ProcessedOn = DateTime.UtcNow;
                outboxMessage.Processed = true;
                _repository.OutboxMessages.Update(outboxMessage);
                await _repository.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error occurred in updating message: {ex.Message}");
            }

        }
        public async Task<bool> SendMessageAsync(OutboxMessage outboxMessage)
        {
            bool sentToASB = false;
            try
            {
                var connectionString = _configuration["AzureBus:ConnectionString"];
                var topicName = _configuration["AzureBus:TopicName"];

                var bytes = Encoding.UTF8.GetBytes(outboxMessage.MessageContent);
                var message = new ServiceBusMessage(bytes)
                {
                    ContentType = "application/json",
                    CorrelationId = outboxMessage.Id.ToString(),
                };
                ServiceBusClientOptions options = new() { TransportType = ServiceBusTransportType.AmqpWebSockets };

                var client = new ServiceBusClient(connectionString, options);
                var sender = client.CreateSender(topicName);
                await sender.SendMessageAsync(message);
                sentToASB = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while sending the message : {outboxMessage.Id} to service bus. ");
            }
            return sentToASB;
        }
    }
}
