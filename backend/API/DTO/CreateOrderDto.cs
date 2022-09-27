using API.Entities.OrderAggregate;

namespace API.DTO
{
    public class CreateOrderDto
    {
        public bool SaveAddress { get; set; } // Orderで送られた Address を保存したいかどうか。True なら新規作成か上書き
        public ShippingAddress ShippingAddress { get; set; }
    }
    // ユーザーはトークンからわかるので必要ない
    // バスケットも DB に保存されているのでクライアントから送る必要はない
}