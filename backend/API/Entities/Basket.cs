using System.Collections.Generic;
using System.Linq;

namespace API.Entities
{
    public class Basket
    {
        public int Id { get; set; }
        public string BuyerId { get; set; } // ログインしていなくてもバスケットには商品を入れれるようにするため、ログインしていないユーザーを識別する
        public List<BasketItem> Items { get; set; } = new(); // null にならないように初期化する
        public string PaymentIntentId { get; set; } // Stripe 用
        public string ClientSecret { get; set; } // Stripe 用

        public void AddItem(Product product, int quantity)
        {
            // バスケットに既に入っている商品か確認
            // 入っていなければバスケットに加える
            if  (Items.All(item => item.ProductId != product.Id)) // Items の全ての要素が条件に当てはまるかチェック
            {
                Items.Add(new BasketItem{Product = product, Quantity = quantity});
            }

            // バスケットに既に入っている商品を取得し、数量を加算
            var existingItem = Items.FirstOrDefault(item => item.ProductId == product.Id);
            if (existingItem != null) existingItem.Quantity += quantity;
        }

        public void RemoveItem(int productId, int quantity)
        {
            var item = Items.FirstOrDefault(item => item.ProductId == productId);
            if (item == null) return;
            item.Quantity -= quantity;
            if (item.Quantity == 0) Items.Remove(item);
        }
    }
}