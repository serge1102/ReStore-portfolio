using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace API.Services
{
    public class PaymentService
    {
        private readonly IConfiguration _config;
        public PaymentService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<PaymentIntent> CreateOrUpdateIntent(Basket basket) //PaymentIntent は Stripe パッケージのクラス
        {
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];
            
            var service = new PaymentIntentService(); //PaymentIntentService は Stripe パッケージのメソッド

            var intent = new PaymentIntent();
            var subtotal = basket.Items.Sum(item => item.Quantity * item.Product.Price);
            var deliveryFee = subtotal > 10000 ? 0 : 500;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                // intent がない場合は新規作成
                var options = new PaymentIntentCreateOptions //PaymentIntentCreateOptions は Stripe パッケージのクラス
                {
                    Amount = subtotal + deliveryFee, // Amount は long 型で用意されている。price を long 型で定義したのはこのため
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> {"card"} // とりあえず card だけにする
                };
                intent = await service.CreateAsync(options);
            }
            else
            {
                // intent がある場合は更新
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = subtotal + deliveryFee,// Item を Basket に追加していたり、削除していたりする可能性があるので Amount を再計算
                };
                await service.UpdateAsync(basket.PaymentIntentId, options);
            }

            return intent;
        }
    }
}