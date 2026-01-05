using Core.Entities.OrederAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkiShop.API.DTOs;
using SkiShop.API.Extentions;

namespace SkiShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController(IUnitOfWork unit,IPaymentService paymentService) : BaseApiController
    {
        [HttpGet("orders")]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrders([FromQuery] OrderSpecParams specParams)
        {
            var spec = new OrderSpecification(specParams);
            return await CreatePageResult(unit.Repository<Order>(), spec, specParams.PageIndex,
                specParams.PageSize, o => o.ToDto());
        }

        [HttpGet("orders/{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var spec = new OrderSpecification(id);
            var order = await unit.Repository<Order>().GetEntityWithSpec(spec);
            if (order == null) return BadRequest("No Order with that id");
            return order.ToDto();
        }

        [HttpPost("orders/refund/{id:int}")]
        public async Task<ActionResult<OrderDto>> RefundOrder(int id)
        {
            var spec = new OrderSpecification(id);
            var order = await unit.Repository<Order>().GetEntityWithSpec(spec);
            if (order == null) return BadRequest("No Order with that id");
            
            if(order.Status != OrderStatus.paymentReceived)
                return BadRequest("Only paid orders can be refunded");

            var result = await paymentService.RefundPayment(order.PaymentIntentId);
            if(result =="succeeded")
            {
               order.Status = OrderStatus.Refunded;
                await unit.Complete();
                return order.ToDto();
            }
            return BadRequest("Problem Refund order");
        }
    }
}
