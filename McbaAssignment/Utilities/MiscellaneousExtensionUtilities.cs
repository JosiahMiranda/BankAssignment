using McbaExample.Data;

namespace McbaExample.Utilities;

public static class MiscellaneousExtensionUtilities
{
    public static bool HasMoreThanNDecimalPlaces(this decimal value, int n) => decimal.Round(value, n) != value;
    public static bool HasMoreThanTwoDecimalPlaces(this decimal value) => value.HasMoreThanNDecimalPlaces(2);

    // method to ensure that an account belongs to a customer
    public static bool AccountNumberBelongsToCustomer(this bool value, McbaContext context, int accountNumber, int customerID) 
    {
        var accounts = context.Accounts.Select(x => x).Where(x => x.CustomerID == customerID).OrderBy(x => x);
        var accountNumbers = accounts.Select(x => x.AccountNumber);
        return accountNumbers.Contains(accountNumber);
    }
}
