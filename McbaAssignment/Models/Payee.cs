using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace McbaExample.Models;

public class Payee
{
    public int PayeeID { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; }

    [Required, StringLength(50)]
    public string Address { get; set; }

    [Required, StringLength(40)]
    public string Suburb { get; set; }

    [Required, StringLength(3, MinimumLength = 3)]
    public string State { get; set; }

    [Required, StringLength(4)]
    [RegularExpression(@"^\d{4}$")]
    public string PostCode { get; set; }

    [Required, StringLength(14)]
    [RegularExpression(@"^(0\d) \d\d\d\d \d\d\d\d$")]
    public string Phone { get; set; }
}
