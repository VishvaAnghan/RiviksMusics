using System.ComponentModel.DataAnnotations.Schema;

namespace RiviksMusics.Models
{
    public class PayNow
    {
        public int PlanId { get; set; }
        public string? PlanName { get; set; }
        public int Rupees { get; set; }

        public int Days { get; set; }
        public string? Duration { get; set; }

        public string? Customername { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }

        [NotMapped]
        public string? TransactionId { get; set; }
        [NotMapped]
        public string? OrderId { get; set; }

        public int? PaymentId { get; set; }
        public virtual Payment? Payment { get; set; }
        public string? Style { get;  set; }
        
    }
}
