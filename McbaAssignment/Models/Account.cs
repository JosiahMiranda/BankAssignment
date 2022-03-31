using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McbaExample.Models;

public class Account
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Display(Name = "Account Number")]
    public int AccountNumber { get; set; }

    [Display(Name = "Type")]
    public char AccountType { get; set; }

    public int CustomerID { get; set; }
    public virtual Customer Customer { get; set; }

    [Column(TypeName = "money")]
    [DataType(DataType.Currency)]
    public decimal Balance { get; set; }

    // Set ambiguous navigation property with InverseProperty annotation or Fluent-API in the McbaContext.cs file.
    //[InverseProperty("Account")]
    public virtual List<Transaction> Transactions { get; set; }

    // method that returns the available balance
    public decimal GetAvailableBalance()
    {
        decimal availableBalance = 0;

        // Checking accounts must always minus 300 from available balance.
        if (AccountType.Equals('C'))
        {
            availableBalance = Balance - 300;
            if (availableBalance < 0)
            {
                availableBalance = 0;
            }
        } else
        {
            availableBalance = Balance;
        }

        return availableBalance;

    }

    // Method to return a more readable string for the account type
    public String GetAccountTypeString()
    {
        if (AccountType.Equals('S'))
        {
            return "Savings";
        } else
        {
            return "Checking";
        }
    }

    // Method to retrieve the service fee of a withdrawal/transfer transaction
    // First two withdrawal/transfer transactions are free.
    public decimal GetServiceFee(TransactionType transactionType) 
    {
        decimal serviceFee = 0;

        int withdrawalOrTransferCounter = 0;
        foreach (Transaction transaction in Transactions)
        {
            if (transaction.TransactionType == TransactionType.Withdraw || 
                transaction.TransactionType == TransactionType.Transfer && transaction.DestinationAccountNumber != null)
            {
                withdrawalOrTransferCounter++;
            }
        }
        if (withdrawalOrTransferCounter >= 2)
        {
            if (transactionType == TransactionType.Withdraw)
            {
                serviceFee = 0.05M;
            }
            else if (transactionType == TransactionType.Transfer)
            {
                serviceFee = 0.10M;
            }
        }

        return serviceFee;
    }
}
