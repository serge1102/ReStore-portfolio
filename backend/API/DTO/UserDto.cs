namespace API.DTO
{
    public class UserDto
    {
        public string Email { get; set; } // UI に表示するのは Email
        public string Token { get; set; }
        public BasketDto Basket { get; set; }
    }
}