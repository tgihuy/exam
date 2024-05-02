using Exam.Application.DTOs;
using Exam.Application.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Exam.Application.Services
{
    public interface IOrderServices
    {
        Task<UpsertOrderResponseDTO> AddAsync(UpsertOrderDTO upsertOrderDTO);
        Task<Order> DeleteAsync(string orderId);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> GetByIdAsync(string orderId);
        Task<Order> UpdateStreetAsync(string orderId, Order order);
    }
}
