using Core.Entities;
using Core.Entities.OrederAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SkiShop.API.Extentions;
using SkiShop.API.SignalR;
using Stripe;

namespace SkiShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController(IPaymentService paymentService,
       IUnitOfWork unit,ILogger<PaymentsController> logger,
       IConfiguration config ,IHubContext<NotificationHub> hubContext) : BaseApiController
    {
        private readonly string _whSecret = "";
        [Authorize]
        [HttpPost("{cartId}")]
        public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string cartId)
        {
            var cart = await paymentService.CreateOrUpdatePaymentIntent(cartId);
            if (cart == null) return BadRequest("Problem with your cart");
            return Ok(cart);

        }
        [HttpGet("delivery-methods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            var methods = await unit.Repository<DeliveryMethod>().ListAllAsync();
            return Ok(methods);

        }

        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent= ConstructStripeEvent(json);
                if(stripeEvent.Data.Object is not PaymentIntent intent)
                {
                    return BadRequest("Invalid event data");
                }
                await HandlePaymentIntentSuccessed(intent);
                return Ok();
            }
            catch (StripeException ex)
            {
                logger.LogError(ex, "Stripe webhook error");
                return StatusCode(StatusCodes.Status500InternalServerError, "webhook error");
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "An Unexpected error occured");
               return StatusCode(StatusCodes.Status500InternalServerError, "An Unexpected error occured");
            }
        }

        private async Task HandlePaymentIntentSuccessed(PaymentIntent intent)
        {
           if(intent.Status == "succeeded")
            {
                var spec=new OrderSpecification(intent.Id,true);
                var order=await unit.Repository<Core.Entities.OrederAggregate.Order>().GetEntityWithSpec(spec)
                    ??throw new Exception("Order not found");

                if((long)order.GetTotal() * 100 != intent.Amount )
                {
                    order.Status = OrderStatus.PaymentMismatch;
                }
                else
                {
                    order.Status = OrderStatus.paymentReceived;
                }
                await unit.Complete();
                var connectionId = NotificationHub.GetConnectionIDByEmail(order.BuyerEmail);
                if(!string.IsNullOrEmpty(connectionId))
                {
                    await hubContext.Clients.Client(connectionId)
                        .SendAsync("OrderCompleteNotification", order.ToDto());
                }

            }
        }

        private Event ConstructStripeEvent(string json)
        {
           try{
               return EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"],
                 _whSecret);
              
              }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to construct stripe event");
                throw new StripeException("Invalide signature");
             }
         
        }
    }
}
