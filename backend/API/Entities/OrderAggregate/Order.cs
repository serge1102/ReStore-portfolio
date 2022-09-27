using System;
using System.Collections.Generic;

namespace API.Entities.OrderAggregate
{
    public class Order
    {
        public int Id { get; set; }
        public string BuyerId { get; set; } // User の Username になるはず
        public ShippingAddress ShippingAddress { get; set; } // ShippingAddress を own している
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public List<OrderItem> OrderItems { get; set; }
        public long Subtotal { get; set; }
        public long DeliveryFee { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public string PaymentIntentId { get; set; }
        public long GetTotal()
        {
            return Subtotal + DeliveryFee;
        }
    }
}