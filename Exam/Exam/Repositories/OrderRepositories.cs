using Exam.Application.Database;
using Exam.Application.Entities;
using Exam.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Exam.Repositories
{
    public class OrderRepositories : IOrderRepositories
    {
        private readonly OrderDbContext _context;
        private readonly ILogger<OrderRepositories> _logger;
        public OrderRepositories(OrderDbContext context, ILogger<OrderRepositories> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Order> AddAsync(Order order)
        {
            try
            {
                if (order == null)
                {
                    return null;
                }
                if (string.IsNullOrEmpty(order.ErrorMessage))
                {
                    order.ErrorMessage = "No error message"; // hoặc bất kỳ giá trị mặc định nào bạn muốn
                }
                var result = await _context.orders.AddAsync(order);
                await _context.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<Order> DeleteAsync(string orderId)
        {
            try
            {
                var deleteitem = await _context.orders.FindAsync(orderId);
                if (deleteitem != null)
                {
                    _context.orders.Remove(deleteitem);
                    await _context.SaveChangesAsync();
                    return await Task.FromResult<Order>(deleteitem);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {
                return await _context.orders.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<Order> GetByIdAsync(string orderId)
        {
            try
            {
                var item = await _context.orders.FindAsync(orderId);
                if (item == null)
                {
                    return null;
                }
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<Order> UpdateStreetAsync(string orderId, Order order)
        {
            try
            {
                var updateitem = await _context.orders.FindAsync(orderId);
                if (updateitem != null)
                {
                    updateitem.Id = order.Id;
                    updateitem.ProductId = order.ProductId;
                    updateitem.Amount = order.Amount;
                    updateitem.Status = order.Status;
                    updateitem.CreatedAt = order.CreatedAt;

                    _context.Entry(updateitem).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    return updateitem;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }
    }
}
