using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.OrederAggregate
{
   public enum OrderStatus
    {
        pending,
        paymentReceived,
        paymentFailed,
        PaymentMismatch,
        Refunded
    }
}
