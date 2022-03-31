using McbaExample.Models;
using Newtonsoft.Json;

namespace McbaExample.Data;

public static class SeedData
{
    // Method to preload the data
    public static void PreloadData(IServiceProvider serviceProvider)
    {

        var context = serviceProvider.GetRequiredService<McbaContext>();
        // Look for customers.
        if (context.Customers.Any())
            return; // DB has already been seeded.

        const string url = "https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/";

        // Get in contact with the web service.
        using var client = new HttpClient();
        var json = client.GetStringAsync(url).Result;

        // Convert the JSON into objects
        var customers = JsonConvert.DeserializeObject<List<Customer>>(json, new JsonSerializerSettings
        {
            DateFormatString = "dd/MM/yyyy"
        });


        // for every customer, insert them.
        // All async methods have .Wait() applied to them. For this instance, we do NOT wants them to run in parallel because it could cause
        // issues reading into the database.
        foreach (var customer in customers)
        {
            // adding the customer to the db
            context.Customers.Add(new Customer
            {
                CustomerID = customer.CustomerID,
                Name = customer.Name,
                Address = customer.Address,
                Suburb = customer.Suburb,
                PostCode = customer.PostCode
            });

            // for every account in this customer, insert them.
            foreach (var account in customer.Accounts)
            {
                // Set account's customerID
                account.CustomerID = customer.CustomerID;

                //var accountType = AccountType.Checking;

                // turn the character into the account type enum
                //if (account.AccountType.Equals("S"))
                //{
                //    accountType = AccountType.Saving;
                //}

                // adding the account to the db
                context.Accounts.Add(new Account
                {
                    AccountNumber = account.AccountNumber,
                    CustomerID = account.CustomerID,
                    AccountType = account.AccountType
                });

                // Accumulate balance as iterates through transactions.
                decimal Balance = 0;
                // for every transaction in this account, insert them.
                foreach (var transaction in account.Transactions)
                {
                    // Set transaction's accountNumber
                    transaction.AccountNumber = account.AccountNumber;

                    Balance += transaction.Amount;

                    // adding the transaction the to the db
                    context.Transactions.Add(new Transaction
                    {
                        AccountNumber = transaction.AccountNumber,
                        Amount = transaction.Amount,
                        TransactionType = TransactionType.Deposit,
                        Comment = transaction.Comment,
                        TransactionTimeUtc = transaction.TransactionTimeUtc
                    });

                }

                // update the account's balance. 
                context.Accounts.Find(account.AccountNumber).Balance = Balance;
            }

            // insert this customer's login details.
            customer.Login.CustomerID = customer.CustomerID;
            context.Logins.Add(new Login
            {
                LoginID = customer.Login.LoginID,
                CustomerID = customer.CustomerID,
                PasswordHash = customer.Login.PasswordHash,
                LoginStatus = LoginStatus.Unlocked
            });
        }

        // Add Payee information.

        context.Payees.Add(new Payee
        {
            Name = "Telstra",
            Address = "123 Telstra Road",
            Suburb = "Werribee",
            State = "VIC",
            PostCode = "6780",
            Phone = "(04) 1111 1111"
        });

        context.Payees.Add(new Payee
        {
            Name = "Optus",
            Address = "456 Optus Street",
            Suburb = "Sunshine",
            State = "VIC",
            PostCode = "4567",
            Phone = "(04) 2222 2222"
        });

        context.Payees.Add(new Payee
        {
            Name = "Vodafone",
            Address = "789 Vodafone Way",
            Suburb = "Melbourne",
            State = "VIC",
            PostCode = "3000",
            Phone = "(04) 3333 3333"
        });

        // Save all of the changes
        context.SaveChanges();

    }
}
