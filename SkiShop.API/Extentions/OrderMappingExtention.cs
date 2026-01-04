using Core.Entities.OrederAggregate;
using SkiShop.API.DTOs;

namespace SkiShop.API.Extentions
{
    public static class OrderMappingExtention
    {
        public static OrderDto ToDto(this Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                BuyerEmail = order.BuyerEmail,
                ShippingAddress = order.ShippingAddress,
                DeliveryMethod = order.DeliveryMethod.Description,
                PaymentSummary = order.PaymentSummary,
                ShippingPrice = order.DeliveryMethod.Price,
                OrderItems =order.OrderItems.Select(x=>x.ToDto()).ToList(),
                Subtotal= order.Subtotal,
                Total= order.GetTotal(),
                Status = order.Status.ToString(),
                PaymentIntentId= order.PaymentIntentId,
            };

        }
        public static OrderItemDto ToDto(this OrderItem orderItem)
        {
            return new OrderItemDto
            {
               ProductId= orderItem.ItemOrdered.ProductId,
                ProductName= orderItem.ItemOrdered.ProductName,
                PictureUrl= orderItem.ItemOrdered.PictureUrl,
                Price= orderItem.Price,
                Quantity= orderItem.Quantity,

            };
        }

    }
}
