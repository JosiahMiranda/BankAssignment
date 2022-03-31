using McbaExample.Models;

namespace McbaAssignment.ViewModels;
public class ConfirmTransactionViewModel
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }
    public int? DestinationAccountNumber { get; set; }

    public string Comment { get; set; }
}
