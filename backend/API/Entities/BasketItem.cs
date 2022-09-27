using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("BasketsItems")] // 指定しないとテーブル名が BasketItem になってしまう
    public class BasketItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int BasketId { get; set; }
        public Basket Basket { get; set; }
    }
}