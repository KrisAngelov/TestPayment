using Microsoft.AspNetCore.Mvc;
using Stripe.BillingPortal;
using TestPayment.Models;
using Stripe.Checkout;

namespace TestPayment.Controllers
{
    public class CheckOutController : Controller
    {
        public IActionResult Index()
        {
            List<ProductEntity> productList = new List<ProductEntity>();
            productList = new List<ProductEntity>
            {
                new ProductEntity
                {
                    Product="Tommy",
                    Rate=1500,
                    Quanity=2,
                    ImagePath="img/Image1.png"
                },
                new ProductEntity
                {
                    Product="TimeWear",
                    Rate=1000,
                    Quanity=1,
                    ImagePath="img/Image2.png"
                }
            };
            return View(productList);
        }

        public IActionResult OrderConfirmation() 
        {
            var service =new Stripe.Checkout.SessionService();
            var session = service.Get(TempData["Session"].ToString());

            if(session.PaymentStatus == "paid")
            {
                var transaction=session.PaymentIntentId.ToString();

                return View("Success");
            }
            return View("Login");
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult CheckOut() 
        {
            List<ProductEntity> productList = new List<ProductEntity>();
            productList = new List<ProductEntity>
            {
                new ProductEntity
                {
                    Product="Tommy",
                    Rate=1500,
                    Quanity=2,
                    ImagePath="img/Image1.png"
                },
                new ProductEntity
                {
                    Product="TimeWear",
                    Rate=1000,
                    Quanity=1,
                    ImagePath="img/Image2.png"
                }
            };

            var domain = "http://localhost:7027/";

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl=domain+$"CheckOut/OrderConfirmation",
                CancelUrl = domain+$"CheckOut/Login",
                LineItems=new List<SessionLineItemOptions>(),
                Mode = "payment"
            };

            foreach(var item in productList)
            {
                var sessionListItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Rate * item.Quanity),
                        Currency = "bgn",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.ToString(),
                        }
                    },
                    Quantity=item.Quanity
                };
                options.LineItems.Add(sessionListItem);
            }
            var service = new Stripe.Checkout.SessionService();
            var session = service.Create(options);

            TempData["Session"] = session.Id;

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
}
