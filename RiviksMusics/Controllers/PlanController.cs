using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using RiviksMusics.Data;
using RiviksMusics.Models;
using System.Numerics;


namespace RiviksMusics.Controllers
{
    public class PlanController : Controller
    {
        //[BindProperty]
        //public PayNow _PayNow { get; set; }

        private readonly ILogger<PlanController> _logger;
        private readonly ApplicationDbContext _context;
        public PlanController(ILogger<PlanController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            
            var plans = this.PlanList();

            return View("Plan", plans);
        }

        private List<PayNow> PlanList()
        {
            
            var plans = new List<PayNow>
            {
                new PayNow { PlanId = 1, PlanName = "Basic Plan", Rupees = 50, Duration = "1 Month" },
                new PayNow { PlanId = 2, PlanName = "Standard Plan", Rupees = 150 , Duration = "2 Month" },
                new PayNow { PlanId = 3, PlanName = "Premium Plan", Rupees = 250 , Duration = "3 Month"}
            };

            return plans;
        }


        private IActionResult GetPlanById(int id)
        {
            var plans = this.PlanList();

            var plan = plans.FirstOrDefault(x => x.PlanId == id);
            if (plan != null)
            {
                return Json(new { status = true, data = plan });
            }
            return Json(new { status = false, message = "Plan not found" });
        }

        public IActionResult InitiateOrder(int PlanId)
        {
            var plans = this.PlanList();

            var plan = plans.FirstOrDefault(x => x.PlanId == PlanId);
            if (plan == null)
            {
                return NotFound();
            }

            string key = "rzp_test_BW1SOqTcrdQTq3";
            string secret = "xmyEwzpav8yIIEgj2mWFpDfr";

            Random _random = new Random();
            string TransactionId = _random.Next(0, 1000000).ToString();

            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount", Convert.ToDecimal(plan.Rupees) * 100);
            input.Add("currency", "INR");
            input.Add("receipt", TransactionId);

            RazorpayClient client = new RazorpayClient(key, secret);
            Razorpay.Api.Order order = client.Order.Create(input);
            //ViewBag.orderid = order["id"].ToString();


            return Json(new { status = true, data = order["id"].ToString() });

            //return View("Payment", plan);
        }

        [HttpPost]
        public IActionResult Payment(string razorpay_payment_id, string razorpay_order_id, string razorpay_signature)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            attributes.Add("razorpay_payment_id", razorpay_payment_id);
            attributes.Add("razorpay_order_id", razorpay_order_id);
            attributes.Add("razorpay_signature", razorpay_signature);

            Utils.verifyPaymentSignature(attributes);

            PayNow orderdetails = new PayNow();
            orderdetails.TransactionId = razorpay_payment_id;
            orderdetails.OrderId = razorpay_order_id;

            return View(orderdetails);
        }
    }
}
