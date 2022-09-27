using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class User : IdentityUser<int> // 主キーは string から int になるが、role は別途 int にする必要
    {
        public UserAddress Address { get; set; }
    }
}