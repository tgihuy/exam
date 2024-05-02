using Exam.Application.DTOs;
using Exam.Application.Entities;
using Exam.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Exam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderServices _services;
        private readonly ILogger<OrderController> _logger;
        public OrderController(IOrderServices services, ILogger<OrderController> logger) 
        {
            _services = services;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<Order>> GetAllAsync()
        {
            try {
                var item = await _services.GetAllAsync();
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }

        }
        [HttpPost]
        public async Task<ActionResult<UpsertOrderResponseDTO>> PostOrder(UpsertOrderDTO upsertOrderDTO)
        {
            var item = await _services.AddAsync(upsertOrderDTO);
            if(item == null)
            {
                return StatusCode(404,"Sản phẩm không tồn tại");
            }else
            return Ok(item);
        }
    }
}
