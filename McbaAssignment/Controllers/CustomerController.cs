using Microsoft.AspNetCore.Mvc;
using McbaExample.Data;
using McbaExample.Models;
using McbaExample.Utilities;
using McbaExampleWithLogin.Filters;
using McbaAssignment.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace McbaExample.Controllers;

// Can add authorize attribute to controllers.
[AuthorizeCustomer]
public class CustomerController : Controller
{
    private readonly McbaContext _context;

    // ReSharper disable once PossibleInvalidOperationException
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public CustomerController(McbaContext context) => _context = context;

    // Can add authorize attribute to actions.
    //[AuthorizeCustomer]
    public async Task<IActionResult> Index()
    {
        // Lazy loading.
        // The Customer.Accounts property will be lazy loaded upon demand.
        var customer = await _context.Customers.FindAsync(CustomerID);

        // OR
        // Eager loading.
        //var customer = await _context.Customers.Include(x => x.Accounts).
        //    FirstOrDefaultAsync(x => x.CustomerID == _customerID);

        return View(customer);
    }

    public async Task<IActionResult> MyStatements(int id, int? page = 1)
    {
        // Ensures that the account being chosen belongs to the customer
        // TRY TO CHANGE THIS INTO AUTHORIZE WHEN YOU FIGURE OUT HOW
        bool validAccount = true;
        validAccount = validAccount.AccountNumberBelongsToCustomer(_context, id, CustomerID);
        if (!validAccount)
        {
            return RedirectToAction(nameof(Index));
        }

        // Retrieve complex object from the session via JSON deserialisation.
        var account = _context.Accounts.Find(id);
        ViewBag.Account = account;

        // Page the orders, maximum of 4 per page.
        const int pageSize = 4;
        var pagedList = await _context.Transactions.Where(x => x.AccountNumber == account.AccountNumber).
            OrderByDescending(x => x.TransactionID).ToPagedListAsync(page, pageSize);

        return View(pagedList);
    }

    // Endpoint to access the user's profile page.
    public async Task<IActionResult> MyProfile()
    {
        var customer = await _context.Customers.FindAsync(CustomerID);
        
        return View(customer);
    }

    // Method to edit the details of the customer
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MyProfile([Bind("CustomerID, Name, TFN, Address, Suburb, State, PostCode, Mobile")] Customer customer)
    {

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customer.CustomerID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(customer);
    }

    // Endpoint to access a bill where we can edit it
    public async Task<IActionResult> EditBill(int id)
    {
        var billPay = await _context.BillPays.FindAsync(id);
        // show up as local time.
        billPay.ScheduleTimeUtc = billPay.ScheduleTimeUtc.ToLocalTime();
        return View(billPay);
    }

    // Method to edit the details of a bill
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditBill(int id, int payeeID, decimal amount, DateTime scheduleTimeUtc,
        string period, [Bind("BillPayID,AccountNumber,PayeeID,Amount,ScheduleTimeUtc,Period,BillPayStatus")] BillPay billPay)
    {
        var payee = _context.Payees.Find(payeeID);
        if (payee == null)
            ModelState.AddModelError(nameof(payeeID), "This Payee does not exist.");
        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
        if (!period.ToLower().Equals("o") && !period.ToLower().Equals("m"))
            ModelState.AddModelError(nameof(period), "Must be 'o' for one-off or 'm' for monthly payments.");

        if (ModelState.IsValid)
        {
            try
            {
                // reset the status to in progress
                billPay.BillPayStatus = BillPayStatus.InProgress;
                billPay.ScheduleTimeUtc = billPay.ScheduleTimeUtc.ToUniversalTime();
                _context.Update(billPay);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillPayExists(billPay.BillPayID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(BillPays), "Customer", new { Id = billPay.AccountNumber});
        }
        return View(billPay);
    }

    public async Task<IActionResult> DeleteBill(int? id)
    {
        if (id == null)
            return NotFound();

        var billPay = await _context.BillPays.FirstOrDefaultAsync(m => m.BillPayID == id);
        if (billPay == null)
            return NotFound();
        _context.BillPays.Remove(billPay);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(BillPays), "Customer", new { id = billPay.AccountNumber });
    }

    // Endpoint to show page where they choose an account for a specific transaction
    public async Task<IActionResult> ChooseAccount(int id)
    {
        
        var accounts = _context.Accounts.Select(x => x).Where(x => x.CustomerID == CustomerID).OrderBy(x => x);
        var accountNumbers = accounts.Select(x => x.AccountNumber);
        // if it doesn't change, then the choose account is not for a transaction, but rather to send them to
        // My Statements page.
        TransactionType transactionType = TransactionType.ServiceCharge;
        if (id == 1)
        {
            transactionType = TransactionType.Deposit;
        }
        else if (id == 2)
        {
            transactionType = TransactionType.Withdraw;
        } else if (id == 3)
        {
            transactionType = TransactionType.Transfer;
        } else if (id == 5)
        {
            transactionType = TransactionType.BillPay;
        }

        return View(new AccountTransactionViewModel
        {
            Accountnumbers = new SelectList(await accountNumbers.ToListAsync()),
            Id = id,
            TransactionType = transactionType
        });

    }

    // Send the user to the transaction page with the account number
    [HttpPost]
    public async Task<IActionResult> ChooseAccount(int Id, int transactionId)
    {
        // Ensures that the account being chosen belongs to the customer
        // TRY TO CHANGE THIS INTO AUTHORIZE WHEN YOU FIGURE OUT HOW
        bool validAccount = true;
        validAccount = validAccount.AccountNumberBelongsToCustomer(_context, Id, CustomerID);
        if (!validAccount)
        {
            return RedirectToAction(nameof(Index));
        }
        var accounts = _context.Accounts.Select(x => x).Where(x => x.CustomerID == CustomerID).OrderBy(x => x);
        var accountNumbers = accounts.Select(x => x.AccountNumber);
        if (transactionId == 1)
        {
            return RedirectToAction(nameof(Deposit), "Customer", new { id = Id });
        }
        else if (transactionId == 2)
        {
            return RedirectToAction(nameof(Withdraw), "Customer", new { id = Id });
        }
        else if (transactionId == 3)
        {
            return RedirectToAction(nameof(Transfer), "Customer", new { id = Id });
        } else if (transactionId == 4)
        {
            return RedirectToAction(nameof(MyStatements), "Customer", new { id = Id });
        } else
        {
            return RedirectToAction(nameof(BillPays), "Customer", new { id = Id });
        }

    }

    // Endpoint to access the user's BillPay page
    public async Task<IActionResult> BillPays(int id)
    {
        var billPays = await _context.BillPays.Where(x => x.AccountNumber == id).ToListAsync();
        var account = _context.Accounts.Find(id);
        ViewBag.Account = account;
        return View(billPays);
    }

    // Endpoint to create a new billpay
    public async Task<IActionResult> CreateNewBill(int id)
    {
        ViewBag.AccountNumber = id;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateNewBill(int id, int payeeID, decimal amount, 
        DateTime scheduleTimeUtc, string period, [Bind("BillPayID,AccountNumber,PayeeID,Amount,ScheduleTimeUtc,Period")] BillPay billPay)
    {
        var payee = _context.Payees.Find(payeeID);
        if (payee == null)
            ModelState.AddModelError(nameof(payeeID), "This Payee does not exist.");
        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
        if (!period.ToLower().Equals("o") && !period.ToLower().Equals("m"))
            ModelState.AddModelError(nameof(period), "Must be 'o' for one-off or 'm' for monthly payments.");
        if (ModelState.IsValid)
        {
            billPay.BillPayStatus = BillPayStatus.InProgress;
            billPay.ScheduleTimeUtc = billPay.ScheduleTimeUtc.ToUniversalTime();
            _context.BillPays.Add(billPay);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(BillPays), "Customer", new { Id = id });
        }
        ViewBag.AccountNumber = id;
        return View();
    }

    // Show view of transaction confirmation.
    public async Task<IActionResult> TransactionConfirmation(ConfirmTransactionViewModel viewModel)
    {
        return View(viewModel);
    }

    // Complete the actual transaction when posted to this endpoint
    [HttpPost]
    public async Task<IActionResult> TransactionConfirmation(int id, decimal amount, int? destinationAccountNumber,
        TransactionType transactionType, string comment)
    {
        var account = await _context.Accounts.FindAsync(id);

        // Do something different based on the transaction type.
        // The code inside can be moved into a validator class for HD part.
        if (transactionType == TransactionType.Deposit)
        {
            account.Balance += amount;
            account.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    Amount = amount,
                    TransactionTimeUtc = DateTime.UtcNow,
                    Comment = comment
                });

        }
        else if (transactionType == TransactionType.Withdraw)
        {
            account.Balance -= (amount + account.GetServiceFee(TransactionType.Withdraw));
            var transactionTimeUTC = DateTime.UtcNow;
            account.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.Withdraw,
                    Amount = amount,
                    TransactionTimeUtc = transactionTimeUTC,
                    Comment = comment
                });
            // If a service fee exists, then it was charged. So add a service fee transaction.
            if (account.GetServiceFee(TransactionType.Withdraw) > 0)
            {
                account.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.ServiceCharge,
                    Amount = account.GetServiceFee(TransactionType.Withdraw),
                    TransactionTimeUtc = transactionTimeUTC
                });
            }
        } else
        {
            // database operations for transfer
            var destinationAccount = await _context.Accounts.FindAsync(destinationAccountNumber);
            account.Balance -= (amount + account.GetServiceFee(TransactionType.Transfer));
            destinationAccount.Balance += (amount);
            var transactionTimeUTC = DateTime.UtcNow;
            account.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.Transfer,
                    Amount = amount,
                    DestinationAccountNumber = destinationAccountNumber,
                    TransactionTimeUtc = transactionTimeUTC,
                    Comment = comment
                });
            // If a service fee exists, then it was charged. So add a service fee transaction.
            if (account.GetServiceFee(TransactionType.Transfer) > 0)
            {
                account.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.ServiceCharge,
                    Amount = account.GetServiceFee(TransactionType.Transfer),
                    TransactionTimeUtc = transactionTimeUTC,
                });
            }
            destinationAccount.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.Transfer,
                    Amount = amount,
                    TransactionTimeUtc = transactionTimeUTC,
                    Comment = comment
                });
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));

    }

    public async Task<IActionResult> Deposit(int id)
    {
        // Ensures that the account being chosen belongs to the customer
        bool validAccount = true;
        validAccount = validAccount.AccountNumberBelongsToCustomer(_context, id, CustomerID);
        if (!validAccount)
        {
            return RedirectToAction(nameof(Index));
        }
        return View(await _context.Accounts.FindAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> Deposit(int id, decimal amount, string comment)
    {
        // Ensures that the account being chosen belongs to the customer
        bool validAccount = true;
        validAccount = validAccount.AccountNumberBelongsToCustomer(_context, id, CustomerID);
        if (!validAccount)
        {
            return RedirectToAction(nameof(Index));
        }
        var account = await _context.Accounts.FindAsync(id);

        if (comment != null && comment.Trim().Length > 30)
            ModelState.AddModelError(nameof(comment), "Comment cannot be greater than 30 characters.");
        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
        if (!ModelState.IsValid)
        {
            ViewBag.Comment = comment;
            ViewBag.Amount = amount;
            return View(account);
        }

        var viewModel = new ConfirmTransactionViewModel {
            Id = id,
            Amount = amount,
            TransactionType = TransactionType.Deposit,
            Comment = comment
        };

        return RedirectToAction(nameof(TransactionConfirmation), "Customer", viewModel);
        
    }

    public async Task<IActionResult> Withdraw(int id)
    {
        // Ensures that the account being chosen belongs to the customer
        bool validAccount = true;
        validAccount = validAccount.AccountNumberBelongsToCustomer(_context, id, CustomerID);
        if (!validAccount)
        {
            return RedirectToAction(nameof(Index));
        }
        return View(await _context.Accounts.FindAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> Withdraw(int id, decimal amount, string comment)
    {
        // Ensures that the account being chosen belongs to the customer
        bool validAccount = true;
        validAccount = validAccount.AccountNumberBelongsToCustomer(_context, id, CustomerID);
        if (!validAccount)
        {
            return RedirectToAction(nameof(Index));
        }
        var account = await _context.Accounts.FindAsync(id);

        if (comment != null && comment.Trim().Length > 30)
            ModelState.AddModelError(nameof(comment), "Comment cannot be greater than 30 characters.");
        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        if (amount > account.GetAvailableBalance() - account.GetServiceFee(TransactionType.Withdraw))
            ModelState.AddModelError(nameof(amount), "Account has insufficient funds.");
        if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
        if (!ModelState.IsValid)
        {
            ViewBag.Comment = comment;
            ViewBag.Amount = amount;
            return View(account);
        }

        var viewModel = new ConfirmTransactionViewModel
        {
            Id = id,
            Amount = amount,
            TransactionType = TransactionType.Withdraw,
            Comment = comment
        };

        return RedirectToAction(nameof(TransactionConfirmation), "Customer", viewModel);

    }

    public async Task<IActionResult> Transfer(int id)
    {
        // Ensures that the account being chosen belongs to the customer
        bool validAccount = true;
        validAccount = validAccount.AccountNumberBelongsToCustomer(_context, id, CustomerID);
        if (!validAccount)
        {
            return RedirectToAction(nameof(Index));
        }
        return View(await _context.Accounts.FindAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> Transfer(int id, decimal amount, int destinationAccountNumber, string comment)
    {
        // Ensures that the account being chosen belongs to the customer
        bool validAccount = true;
        validAccount = validAccount.AccountNumberBelongsToCustomer(_context, id, CustomerID);
        if (!validAccount)
        {
            return RedirectToAction(nameof(Index));
        }
        var account = await _context.Accounts.FindAsync(id);
        var destinationAccount = await _context.Accounts.FindAsync(destinationAccountNumber);

        if (comment != null && comment.Trim().Length > 30)
            ModelState.AddModelError(nameof(comment), "Comment cannot be greater than 30 characters.");
        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        if (amount > account.GetAvailableBalance() - account.GetServiceFee(TransactionType.Transfer))
            ModelState.AddModelError(nameof(amount), "Account has insufficient funds.");
        if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
        if (destinationAccount == null)
        {
            ModelState.AddModelError(nameof(destinationAccountNumber), "That Account does not exist.");
        } else if (destinationAccount.AccountNumber == account.AccountNumber)
        {
            ModelState.AddModelError(nameof(destinationAccountNumber), "Cannot transfer to the same account.");
        }
        if (!ModelState.IsValid)
        {
            ViewBag.Comment = comment;
            ViewBag.Amount = amount;
            ViewBag.DestinationAccountNumber = destinationAccountNumber;
            return View(account);
        }

        var viewModel = new ConfirmTransactionViewModel
        {
            Id = id,
            Amount = amount,
            TransactionType = TransactionType.Transfer,
            DestinationAccountNumber = destinationAccountNumber,
            Comment = comment
        };

        return RedirectToAction(nameof(TransactionConfirmation), "Customer", viewModel);
    }

    // method to check a customer exists
    private bool CustomerExists(int id)
    {
        return _context.Customers.Any(e => e.CustomerID == id);
    }

    // method to check a billpay exists
    private bool BillPayExists(int id)
    {
        return _context.BillPays.Any(e => e.BillPayID == id);
    }
}
