using McbaExample.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace McbaAssignment.ViewModels;
public class AccountTransactionViewModel
{
    public SelectList Accountnumbers { get; set; }
    public decimal Amount { get; set; }
    public int Id { get; set; }
    public TransactionType TransactionType { get; set; }
    public int? DestinationAccountNumber { get; set; }
}
