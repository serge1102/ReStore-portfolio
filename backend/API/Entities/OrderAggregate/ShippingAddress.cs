using Microsoft.EntityFrameworkCore;

namespace API.Entities.OrderAggregate
{
    [Owned] // Order に own されている
    public class ShippingAddress : Address
    {
        
    }
}