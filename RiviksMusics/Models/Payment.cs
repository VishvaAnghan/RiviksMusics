using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RiviksMusics.Models
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public int? PlanId { get; set; }
        //public PayNow? Plan { get; set; }

        public int? Amount { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Expired Plan")]
        public DateTime ExpiredPlanDate { get; set; }

        public string? TransactionId { get; set; }
        public string? OrderId { get; set; }
        public string? Status { get; set; }
    }
}
