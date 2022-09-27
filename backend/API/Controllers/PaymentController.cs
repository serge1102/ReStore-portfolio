using System.IO;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities.OrderAggregate;
using API.Extensions;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace API.Controllers
{
    public class PaymentController : BaseApiController
    {
        private readonly PaymentService _paymentService;
        private readonly StoreContext _context;
        private readonly IConfiguration _config;
        public PaymentController(PaymentService paymentService, StoreContext context, IConfiguration config)
        {
            _paymentService = paymentService;
            _context = context;
            _config = config;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BasketDto>> CreateOrUpdatePaymentIntent()
        {
            var basket = await _context.Baskets
                .RetrieveBasketWithItems(User.Identity.Name)
                .FirstOrDefaultAsync();

            if (basket == null) return NotFound();

            var intent = await _paymentService.CreateOrUpdateIntent(basket);

            if (intent == null) return BadRequest(new ProblemDetails { Title = "Problem Creating payment intent" }); // ユーザー用のメッセージではない

            basket.PaymentIntentId = basket.PaymentIntentId ?? intent.Id; // intent の新規作成時(basket.PaymentIntentId は null)のみ itent.id を代入
            basket.ClientSecret = basket.ClientSecret ?? intent.ClientSecret;

            _context.Update(basket);

            var result = await _context.SaveChangesAsync() > 0;
            if (!result) return BadRequest(new ProblemDetails { Title = "Problem updating basket with intent" }); // ユーザー用のメッセージではない

            return basket.MapBasketToDto();
        }

        [HttpPost("webhook")] // Stripe からのリクエスト用のため anonymous にする必要
        public async Task<ActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"],
                _config["StripeSettings:WhSecret"]); // Stripe から送られたシークレットと config のシークレットを比較

            var charge = (Charge)stripeEvent.Data.Object; // Charge は Stripe パッケージのクラス

            var order = await _context.Orders.FirstOrDefaultAsync(x =>
                x.PaymentIntentId == charge.PaymentIntentId);
            
            if (charge.Status == "succeeded") order.OrderStatus = OrderStatus.PaymentReceived;

            await _context.SaveChangesAsync();

            // Stripe に Stripe からのリクエストを受け取ることができたことを返す必要がある
            // 返さないと /payment/webhook に Stripe がリクエストを送り続けてしまう
            return new EmptyResult();
        }
    }
}