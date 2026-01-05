using Core.Entities.OrederAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class OrderSpecification : BaseSpecification<Order>
    {
        public OrderSpecification(string email) : base(o => o.BuyerEmail == email)
        {
            AddInclude(i => i.OrderItems);
            AddInclude(i => i.DeliveryMethod);
            AddOrderByDescending(o => o.OrderDate);
        }
        public OrderSpecification(int id, string email)
            : base(o => o.Id == id && o.BuyerEmail == email)
        {
            AddInclude("OrderItems");
            AddInclude("DeliveryMethod");
        }

        public OrderSpecification(string paymentIntentId, bool isPaymentIntent)
            : base(o => o.PaymentIntentId == paymentIntentId)
        {
            AddInclude("OrderItems");
            AddInclude("DeliveryMethod");
        }

        public OrderSpecification(OrderSpecParams specParams) : base(x => 
        string.IsNullOrEmpty(specParams.Status) || x.Status == ParseStatus(specParams.Status)
        )
        {
            AddInclude("OrderItems");
            AddInclude("DeliveryMethod");
            ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1),
                specParams.PageSize);
            AddOrderByDescending (o => o.OrderDate);
        }
        public OrderSpecification(int id) : base(o => o.Id == id)
        {
            AddInclude("OrderItems");
            AddInclude("DeliveryMethod");
        }

        private static OrderStatus? ParseStatus(string status)
        {
            if (Enum.TryParse<OrderStatus>(status, true, out var result)) return result; 
            return null;
        }
    }
}
