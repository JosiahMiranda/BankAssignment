using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace McbaExample.Models;

public class Customer
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [RegularExpression(@"^\d{4}$")]
    public int CustomerID { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; }

    [StringLength(11)]
    public string TFN { get; set; }

    [StringLength(50)]
    public string Address { get; set; }

    [StringLength(40)]
    public string Suburb { get; set; }

    [NotMapped()]
    public string City { set { Suburb = value; } }

    [StringLength(3, MinimumLength = 3)]
    public string State { get; set; }

    [StringLength(4)]
    [RegularExpression(@"^\d{4}$")]
    public string PostCode { get; set; }

    [StringLength(12)]
    [RegularExpression(@"^04\d\d \d\d\d \d\d\d$")]
    public string Mobile { get; set; }

    public virtual List<Account> Accounts { get; set; }

    public virtual Login Login { get; set; }
}
