using System.Threading.Tasks;
using Exam.Application.Entities;

namespace Exam.Application.Repositories
{
    public interface IOrderRepositories
    {
        Task<Order> AddAsync(Order order);
        Task<Order> DeleteAsync(string orderId);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> GetByIdAsync(string orderId);
        Task<Order> UpdateStreetAsync(string orderId, Order order);
    }
}
