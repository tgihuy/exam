using System.Text.Json;
using Confluent.Kafka;
using Exam.Application.DTOs;
using Exam.Application.Entities;
using Exam.Application.Repositories;
using Manonero.MessageBus.Kafka.Abstractions;

namespace Exam.Application.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly IOrderRepositories _repositories;
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrderServices> _logger;
        private readonly IConfiguration _configuration;
        private readonly IKafkaProducerManager _produceManager;

        public OrderServices(IOrderRepositories repositories, HttpClient httpClient, ILogger<OrderServices> logger, IConfiguration configuration, IKafkaProducerManager produceManager)
        {
            _repositories = repositories;
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            _produceManager = produceManager;
        }
        public async Task<UpsertOrderResponseDTO> AddAsync(UpsertOrderDTO upsertOrderDTO)
        {
            try
            {
                string productResponse = _configuration["HttpGetProduct"] + "/" + upsertOrderDTO.ProductId;
                Order order = new Order();
                UpsertOrderResponseDTO upsertOrderResponseDTO = new UpsertOrderResponseDTO("", order);
                HttpResponseMessage responseMessage = await _httpClient.GetAsync(productResponse);
                if (responseMessage.IsSuccessStatusCode)
                {
                    if (responseMessage.Content.Headers.ContentLength != 0)
                    {
                        var product = await responseMessage.Content.ReadFromJsonAsync<ProductDTO>();
                        if (product != null)
                        {
                            var orderAdd = new Order(upsertOrderDTO.ProductId, upsertOrderDTO.Amount);
                            var checkProductAmount = new CheckProductAmount()
                            {
                                RefId = orderAdd.Id,
                                BusinessType = 1,
                                ProductId = orderAdd.ProductId,
                                Amount = orderAdd.Amount,
                            };
                            if(product.RemainingAmount > orderAdd.Amount) {
                                ProduceOrderEvent(checkProductAmount);
                                var orderResult = await _repositories.AddAsync(orderAdd);
                                upsertOrderResponseDTO.Data = orderResult;
                                upsertOrderResponseDTO.Message = "Add thành công";
                                return upsertOrderResponseDTO;
                            }
                            else
                            {
                                upsertOrderResponseDTO.Data = null;
                                upsertOrderResponseDTO.Message = "Không đủ sản phẩm";
                                return upsertOrderResponseDTO;
                            }
                            
                        }
                    }
                }

                upsertOrderResponseDTO.Data = null;
                upsertOrderResponseDTO.Message = "Sản phẩm không tồn tại";
                return upsertOrderResponseDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        private void ProduceOrderEvent(CheckProductAmount checkProductAmount)
        {
            var json = JsonSerializer.Serialize(checkProductAmount);
            var message = new Message<string, string>()
            {
                Key = checkProductAmount.RefId,
                Value = json,
            };

            var kafkaProducer = _produceManager.GetProducer<string, string>("Order");
            kafkaProducer.Produce(message);
            _logger.LogInformation($"Received message: {message}");
        }
        public async Task<Order> DeleteAsync(string orderId)
        {
            return await _repositories.DeleteAsync(orderId);
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _repositories.GetAllAsync();
        }

        public async Task<Order> GetByIdAsync(string orderId)
        {
            return await _repositories.GetByIdAsync(orderId);
        }

        public async Task<Order> UpdateStreetAsync(string orderId, Order order)
        {
            return await _repositories.UpdateStreetAsync(orderId, order);
        }
    }
}
