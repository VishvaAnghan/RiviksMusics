using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using RiviksMusics.Data;
using RiviksMusics.Models;
using System;
using System.Numerics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace RiviksMusics.Controllers
{
    public class PlanController : Controller
    {
        //[BindProperty]
        //public PayNow _PayNow { get; set; }

        private readonly ILogger<PlanController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        

        public PlanController(ILogger<PlanController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewBag.isplan = "active";
            var plans = this.PlanList();

            return View("Plan", plans);
        }

        private List<PayNow> PlanList()
        {

            var plans = new List<PayNow>
            {
                new PayNow { PlanId = 1, PlanName = "Basic Plan", Rupees = 50, Days = 30, Duration = "1 Month" },
                new PayNow { PlanId = 2, PlanName = "Standard Plan", Rupees = 250 , Days = 60 , Duration = "6 Month" },
                new PayNow { PlanId = 3, PlanName = "Premium Plan", Rupees = 150 ,Days = 90 , Duration = "3 Month"}
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
        public async Task<IActionResult> Payment(string razorpay_payment_id, string razorpay_order_id, string razorpay_signature, int planId)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            attributes.Add("razorpay_payment_id", razorpay_payment_id);
            attributes.Add("razorpay_order_id", razorpay_order_id);
            attributes.Add("razorpay_signature", razorpay_signature);

            Utils.verifyPaymentSignature(attributes);

            var currentUserId = (await _userManager.GetUserAsync(HttpContext.User)).Id;

            var Payment = new Models.Payment
            {
                //PaymentId = payment.PaymentId,
                UserId = currentUserId,
                //PlanId = payment.PlanId,
                //Amount = payment.Amount,
                PaymentDate = DateTime.Now,
                //ExpiredPlan = payment.ExpiredPlan,
                TransactionId = razorpay_payment_id,
                OrderId = razorpay_order_id,
                Status = "Success"
            };

            var plans = this.PlanList();

            var plan = plans.FirstOrDefault(x => x.PlanId == planId);
            if (plan != null)
            {
                Payment.PlanId = plan.PlanId;
                Payment.Amount = plan.Rupees;
                Payment.ExpiredPlanDate = DateTime.Now.AddDays(plan.Days);
            }

            _context.Payment.Add(Payment);
            await _context.SaveChangesAsync();

            //PayNow orderdetails = new PayNow();
            //orderdetails.TransactionId = razorpay_payment_id;
            //orderdetails.OrderId = razorpay_order_id;

            return RedirectToAction("Payments");
        }

        public async Task<IActionResult> Payments(List<PaymentDto> model)
        {
            ViewBag.ispayment = "active";
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            var roles = await _userManager.GetRolesAsync(user);
            
            if (roles.Contains("Artist"))
            {
                var plans = this.PlanList();
                var data = await _context.Payment.Include(x => x.User).ToListAsync();

                 model = data.Where(m => m.UserId == user.Id).Select(p => new PaymentDto
                {
                    PaymentId = p.PaymentId,
                    User = p.User,
                    PlanId = p.PlanId,
                    Plan = plans.Where(x => x.PlanId == Convert.ToInt32(p.PlanId ?? 0)).FirstOrDefault(),
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    ExpiredPlanDate = p.ExpiredPlanDate,
                    TransactionId = p.TransactionId,
                    OrderId = p.OrderId,
                    Status = p.Status
                }).OrderByDescending(x => x.PaymentId).ToList();
            }
            else if(roles.Contains("Admin"))
            {
                var plans = this.PlanList();
                var data = await _context.Payment.Include(x => x.User).ToListAsync();

                 model = data.Select(p => new PaymentDto
                {
                    PaymentId = p.PaymentId,
                    User = p.User,
                    PlanId = p.PlanId,
                    Plan = plans.Where(x => x.PlanId == Convert.ToInt32(p.PlanId ?? 0)).FirstOrDefault(),
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    ExpiredPlanDate = p.ExpiredPlanDate,
                    TransactionId = p.TransactionId,
                    OrderId = p.OrderId,
                    Status = p.Status
                }).OrderByDescending(x => x.PaymentId).ToList();
            }
            else 
            {
                model = new List<PaymentDto>();
            }
            return View("Paysuccess", model);
        }
    
        public IActionResult PaymentPrint(int id)
        {
            var plans = this.PlanList();

            var payment = new PaymentDto();
            payment = _context.Payment.Where(p => p.PaymentId == id).Select(a => new PaymentDto
            {
                PaymentId= a.PaymentId,
                User= a.User,
                PlanId= a.PlanId,
                //Plan = plans.Where(x => x.PlanId == Convert.ToInt32(a.PlanId ?? 0)).FirstOrDefault(),
                Amount = a.Amount,
                PaymentDate = a.PaymentDate,
                ExpiredPlanDate = a.ExpiredPlanDate,
                TransactionId = a.TransactionId,
                OrderId = a.OrderId,
                Status = a.Status
            }).FirstOrDefault();

            if (payment != null)
            {
                payment.Plan = plans.Where(x => x.PlanId == Convert.ToInt32(payment.PlanId ?? 0)).FirstOrDefault();
            }

            return View(payment);
        }
    }
}
