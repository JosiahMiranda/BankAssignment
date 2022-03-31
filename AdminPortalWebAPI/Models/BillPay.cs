using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McbaExample.Models;

public enum BillPayStatus
{
    InProgress = 1,
    Failed = 2,
    Blocked = 3,
}

public class BillPay
{
    public int BillPayID { get; set; }

    [ForeignKey("Account")]
    public int AccountNumber { get; set; }
    public virtual Account Account { get; set; }

    [ForeignKey("Payee")]
    public int PayeeID { get; set; }
    public virtual Payee Payee { get; set; }

    [Column(TypeName = "money")]
    public decimal Amount { get; set; }

    [Required]
    [Display(Name = "Scheduled Time")]
    public DateTime ScheduleTimeUtc { get; set; }

    [Required]
    public char Period { get; set; }

    [Display(Name = "Status")]
    public BillPayStatus BillPayStatus { get; set; }

    // Method to return a more readable version of the status
    public string GetStatusString()
    {
        string output = "";
        if (BillPayStatus == BillPayStatus.InProgress)
        {
            output = "In Progress";
        } else if (BillPayStatus == BillPayStatus.Failed)
        {
            output = "Failed";
        } else
        {
            output = "Blocked";
        }
        return output;
    }
}
