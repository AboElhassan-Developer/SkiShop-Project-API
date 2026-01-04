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
            AddInclude("DeliveryMethod ");
        }

        public OrderSpecification(string paymentIntentId, bool isPaymentIntent)
            : base(o => o.PaymentIntentId == paymentIntentId)
        {
            AddInclude("OrderItems");
            AddInclude("DeliveryMethod ");
        }
    }
}
