using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RiviksMusics.Data;
using RiviksMusics.Models;


namespace RiviksMusics.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public DashboardController(ILogger<DashboardController> logger , ApplicationDbContext context , UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }
       
        public async Task<IActionResult> Index(string Day)
        {
            ViewBag.isdashbord = "active";

            int currentMonth = DateTime.Today.Month;
            int currentmonthCount = _context.Music.Count(i => i.UploadDate.Month == currentMonth);
            ViewBag.Currentmonth = currentmonthCount;

            int currentmonthAlbum = _context.Album.Count(i => i.UploadDate.Month == currentMonth);
            ViewBag.CurrentAlbum = currentmonthAlbum;

           
                DateTime today = DateTime.Today;
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            var roles = await _userManager.GetRolesAsync(user);
            bool isAdmin = roles.Contains("Admin");
            List<Payment> todaypayment;
            if (isAdmin)
            {
                 todaypayment = _context.Payment.Where(p => p.PaymentDate.Date == today).ToList();
            }
            else
            {
                todaypayment = _context.Payment.Where(p => p.PaymentDate.Date == today && p.UserId == user.Id).ToList();
            }
            
                decimal TodayPayment = todaypayment.Sum(p => p.Amount ?? 0);
                ViewBag.Todaypayment = TodayPayment;

                DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
                DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            List<Payment> monthPayments;
            if (isAdmin)
            {
                 monthPayments = _context.Payment.Where(p => p.PaymentDate >= startOfMonth && p.PaymentDate <= endOfMonth).ToList();
            }
            else
            {
                monthPayments = _context.Payment.Where(p => p.PaymentDate >= startOfMonth && p.PaymentDate <= endOfMonth && p.UserId == user.Id).ToList();
            }
            decimal monthTotalPayment = monthPayments.Sum(p => p.Amount ?? 0);
                ViewBag.MonthTotalPayment = monthTotalPayment;


            var currentmonth = today.Month;
            List<PayDto> monthlypayment;
            if (isAdmin)
            {

                monthlypayment = _context.Payment.Where(p => p.PaymentDate.Month == currentmonth)
                   .GroupBy(p => p.PaymentDate.Day)
                   .Select(m => new PayDto
                   { Day = m.Key, Count = m.Count() }).ToList();
            }
            else
            {
                monthlypayment = _context.Payment.Where(p => p.PaymentDate.Month == currentmonth && p.UserId == user.Id)
                   .GroupBy(p => p.PaymentDate.Day)
                   .Select(m => new PayDto
                   { Day = m.Key, Count = m.Count() }).ToList();
            }
                var daysInCurrentMonth = DateTime.DaysInMonth(today.Year, currentmonth);
                var monthdate = new int[daysInCurrentMonth];
                foreach (var mp in monthlypayment)
                {
                    monthdate[mp.Day - 1] = mp.Count;
                }

                ViewBag.MonthlyPaymentCount = monthdate;
            return View();
            }
        }
    }


