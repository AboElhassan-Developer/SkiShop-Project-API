using Core.Entities;
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
    [Authorize]
    public class OrdersController(ICartService cartService, IUnitOfWork unit) : BaseApiController
    {
        [HttpPost]
        public async Task<ActionResult> CreateOrder(CreateOrderDto orderDto)
        {
            var email = User.GetEmail();
            var cart = await cartService.GetCartAsync(orderDto.CartId);
            if (cart == null) return BadRequest("Problem with your cart");

            if (cart.PaymentIntentId == null)
            {
                return BadRequest("Payment intent is missing");
            }
            var items = new List<OrderItem>();
            foreach (var item in cart.Items)
            {
                var productItem = await unit.Repository<Product>().GetByIdAsync(item.ProductId);
                if (productItem == null)
                {
                    return BadRequest("Problem with the order");
                }
                var itemOrdered = new ProductItemOrdered
                {
                    PictureUrl = item.PictureUrl,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName
                };
                var orderItem = new OrderItem
                {
                    ItemOrdered = itemOrdered,
                    Price = productItem.Price,
                    Quantity = item.Quantity
                };
                items.Add(orderItem);

            }
            var deliveryMethod = await unit.Repository<DeliveryMethod>().GetByIdAsync(orderDto.DeliveryMethodId);
            if (deliveryMethod == null)
            {
                return BadRequest("Problem with the delivery method");
            }
            var order = new Order
            {
                OrderItems = items,
                DeliveryMethod = deliveryMethod,
                BuyerEmail = email,
                ShippingAddress = orderDto.ShippingAddress,
                PaymentSummary = orderDto.PaymentSummary,
                Subtotal = items.Sum(x => x.Price * x.Quantity),
                PaymentIntentId = cart.PaymentIntentId




            };
            unit.Repository<Order>().Add(order);
            if (await unit.Complete())
            {
                return Ok(order);
            }
            return BadRequest("Problem creating order");
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrdersForUser()
        {
            var spec = new OrderSpecification(User.GetEmail());
            var orders = await unit.Repository<Order>().ListAsync(spec);

            var ordersToReturn = orders.Select(o=>o.ToDto()).ToList();
            return Ok(ordersToReturn);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var spec = new OrderSpecification(id, User.GetEmail());
            var order = await unit.Repository<Order>().GetEntityWithSpec(spec);


            if (order == null) return NotFound();
            return order.ToDto();

        }
    }
}
