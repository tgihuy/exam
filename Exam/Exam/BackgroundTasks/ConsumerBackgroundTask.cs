using System.Text.Json;
using Confluent.Kafka;
using Exam.Application.DTOs;
using Exam.Application.Repositories;
using Exam.Application.Services;
using Manonero.MessageBus.Kafka;
using Manonero.MessageBus.Kafka.Abstractions;

namespace Exam.BackgroundTasks
{
    public class ConsumerBackgroundTask:IConsumingTask<string, string>
    {
        private readonly ILogger<ConsumerBackgroundTask> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IKafkaConsumer<string, string> _consumer;
        private readonly IKafkaProducerManager _kafkaProducer;

        public ConsumerBackgroundTask(ILogger<ConsumerBackgroundTask> logger, IServiceProvider serviceProvider, IKafkaConsumerManager consumer, IKafkaProducerManager kafkaProducer)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _consumer = consumer.GetConsumer<string, string>("Order");
            _kafkaProducer = kafkaProducer;
        }

        public void Execute(ConsumeResult<string, string> result)
        {
            try
            {
                var message = result.Message.Value;
                // Process the Kafka message here
                ConsumerCallbackAsync( message);
                _consumer.Commit(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error consuming message: {ex}");
            }
        }

        private async void ConsumerCallbackAsync(string message)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    // Deserialize the message to extract information
                    var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepositories>();
                    var orderStatusMessage = JsonSerializer.Deserialize<OrderStatusMessage>(message);
                    var order = await orderRepository.GetByIdAsync(orderStatusMessage.RefId);
                    if (orderStatusMessage.BusinessType == 1)
                    {
                        // Get the order from the database based on RefId
                        if (order != null)
                        {
                            // Update order status based on ErrorCode
                            if (orderStatusMessage.ErrorCode == 0)
                            {
                                order.Status = "Accepted";
                            }
                            else
                            {
                                order.Status = "Rejected";
                                order.ErrorCode = orderStatusMessage.ErrorCode;
                                order.ErrorMessage = orderStatusMessage.ErrorMessage;
                            }

                            // Send a message to release hold amount to the input-topic
                            var releaseHoldAmountMessage = new ReleaseHoldAmount()
                            {
                                RefId = order.Id,
                                BusinessType = 2, // Indicates ReleaseHoldAmount message
                                ProductId = order.ProductId
                            };
                            var releaseHoldAmountJson = JsonSerializer.Serialize(releaseHoldAmountMessage);
                            var releaseHoldAmountKafkaMessage = new Message<string, string>()
                            {
                                Key = order.Id,
                                Value = releaseHoldAmountJson
                            };
                            var kafkaProducer = _kafkaProducer.GetProducer<string, string>("Order");
                            kafkaProducer.Produce(releaseHoldAmountKafkaMessage);
                            _logger.LogInformation($"Produced ReleaseHoldAmount message: {releaseHoldAmountJson}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}
