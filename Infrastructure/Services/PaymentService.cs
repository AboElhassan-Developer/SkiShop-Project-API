using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Product = Core.Entities.Product;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ICartService cartService;
        private readonly IUnitOfWork unit;

        public PaymentService(IConfiguration config, ICartService cartService,
         IUnitOfWork unit)
        {
            this.cartService = cartService;
            this.unit = unit;
            StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];
        }


        public async Task<string> RefundPayment(string paymentIntentId)
        {
            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId
                
            };
            var refundService = new RefundService();
            var result = await refundService.CreateAsync(refundOptions);
            return result.Status;
        }


        public async Task<ShoppingCart?> ApplyCouponToCart(string cartId, string couponCode)
        {
            var cart = await cartService.GetCartAsync(cartId);
            if (cart == null) return null;

            var promotionCodeService = new PromotionCodeService();
            var promotionCodes = await promotionCodeService.ListAsync(new PromotionCodeListOptions
            {
                Code = couponCode,
                Active = true,
                Expand = ["data.coupon"]
            });

            var promotionCode = promotionCodes.FirstOrDefault();
            if (promotionCode == null) throw new Exception("Invalid coupon code");

            cart.CouponId = promotionCode.Id;

           
            var couponId = promotionCode.RawJObject["promotion"]?["coupon"]?.ToString();
            if (couponId == null) throw new Exception("Invalid coupon");

            var couponService = new CouponService();
            var coupon = await couponService.GetAsync(couponId);

            if (coupon.PercentOff.HasValue)
            {
                var subtotal = cart.Items.Sum(i => i.Quantity * i.Price);
                cart.Discount = subtotal * (decimal)(coupon.PercentOff.Value / 100);
            }
            else if (coupon.AmountOff.HasValue)
            {
                cart.Discount = (decimal)(coupon.AmountOff.Value / 100);
            }

            await cartService.SetCartAsync(cart);
            return cart;
        }
        public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
        {
            var cart = await cartService.GetCartAsync(cartId);
            if (cart == null) return null;
            var shippingPrice = 0m;
            if (cart.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await unit.Repository<DeliveryMethod>().GetByIdAsync((int)cart.DeliveryMethodId);
                shippingPrice = deliveryMethod.Price;
            }
            foreach (var item in cart.Items)
            {
                var productItem = await unit.Repository<Product>().GetByIdAsync(item.ProductId);
                if (productItem == null) return null;
                if (item.Price != productItem.Price)
                {
                    item.Price = productItem.Price;
                }
            }

            var discount = (long)(cart.Discount * 100); 

            var service = new PaymentIntentService();
            PaymentIntent? intent = null;

            if (string.IsNullOrEmpty(cart.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)cart.Items.Sum(i => i.Quantity * (i.Price * 100))
                            + (long)(shippingPrice * 100)
                            - discount, 
                    Currency = "usd",
                    PaymentMethodTypes = ["card"]
                };
                intent = await service.CreateAsync(options);
                cart.PaymentIntentId = intent.Id;
                cart.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)cart.Items.Sum(i => i.Quantity * (i.Price * 100))
                            + (long)(shippingPrice * 100)
                            - discount, 
                };
                intent = await service.UpdateAsync(cart.PaymentIntentId, options);
            }
            await cartService.SetCartAsync(cart);
            return cart;
        }


    }
}
