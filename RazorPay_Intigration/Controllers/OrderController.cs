using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using RazorPay_Intigration.Models;

namespace RazorPay_Intigration.Controllers
{
    public class OrderController : Controller
    {
        [BindProperty]
        public EntityOrder orderDetails {  get; set; }
        public OrderController()
        {
            // Initialize orderDetails to avoid NullReferenceException
            orderDetails = new EntityOrder();
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateOrder()
        {
            string key = "[YOUR_KEY_ID]";
            string secret = "[YOUR_KEY_SECRET]";

            Random _random = new Random();
            string transactionId = _random.Next(0, 3000).ToString();

            // Convert the order amount to decimal
            decimal orderAmount = Convert.ToDecimal(orderDetails.Amount);

            // Ensure that the order amount is at least 1 INR
            if (orderAmount < 1.00m)
            {
                orderAmount = 1.00m;
            }

            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount", orderAmount * 100); // amount should be same as transaction amount
            input.Add("currency", "INR");
            input.Add("receipt", transactionId);

            RazorpayClient client = new RazorpayClient(key, secret);

            Razorpay.Api.Order order = client.Order.Create(input);
            ViewBag.orderId = order["id"].ToString();
            return View("Payment", orderDetails);

        }

        public ActionResult Payment(string razorpay_payment_id, string razorpay_order_id, string razorpay_signature)
        {
           

            Dictionary<string, string> attributes = new Dictionary<string, string>();

            attributes.Add("razorpay_payment_id", razorpay_payment_id);
            attributes.Add("razorpay_order_id", razorpay_order_id);
            attributes.Add("razorpay_signature", razorpay_signature);

            EntityOrder orderDetl = new EntityOrder();
            orderDetl.TransactionId = razorpay_payment_id;
            orderDetl.OrderId = razorpay_order_id;


            Utils.verifyPaymentSignature(attributes);

            return View("PaymentSuccess",orderDetl);
        }
    }
}
