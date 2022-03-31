using McbaExample.Data;
using Microsoft.EntityFrameworkCore;

namespace McbaAssignment.BackgroundServices;

public class BillBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<BillBackgroundService> _logger;

    public BillBackgroundService(IServiceProvider services, ILogger<BillBackgroundService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Bill Background Service is running.");

        while (!cancellationToken.IsCancellationRequested)
        {
            await ProcessBills(cancellationToken);

            _logger.LogInformation("Bill Background Service is waiting 60 seconds.");

            await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        }
    }

    // Algorithm to process bills:
    // IF ScheduledDateTime < CurrentDateTime THEN
    //     IF Account.AvailableBalance >= Amount THEN
    //         - PAY BILL -
    //         Account.Balance - Amount
    //         IF Period == O THEN
    //             DELETE BILL
    //         ELSE IF Period == M THEN
    //             Bill.ScheduledDateTime = ScheduledDateTime + 1 Month
    //     ELSE
    //         Bill.Status = FAILED
    //
    private async Task ProcessBills(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Bill Background Service is working.");

        using var scope = _services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<McbaContext>();

        var billPays = await context.BillPays.Select(x => x).Where(x => x.BillPayStatus != McbaExample.Models.BillPayStatus.Blocked).Where(x => x.ScheduleTimeUtc <= DateTime.Now)
            .Where(x => x.BillPayStatus == McbaExample.Models.BillPayStatus.InProgress).ToListAsync(cancellationToken);
        foreach (var billPay in billPays)
        {
            
            var account = context.Accounts.Find(billPay.AccountNumber);
            _logger.LogInformation($"Available Balance: {account.GetAvailableBalance()} and bill amount: {billPay.Amount}");
            if (account.GetAvailableBalance() >= billPay.Amount)
            {
                // remove balance and also make new transaction
                account.Balance -= billPay.Amount;
                // add new billpay transaction
                context.Transactions.Add(new McbaExample.Models.Transaction
                {
                    TransactionType = McbaExample.Models.TransactionType.BillPay,
                    AccountNumber = billPay.AccountNumber,
                    Amount = billPay.Amount,
                    TransactionTimeUtc = billPay.ScheduleTimeUtc

                });
                if (billPay.Period.Equals('o'))
                {
                    // Delete the bill.
                    context.BillPays.Remove(billPay);
                    _logger.LogInformation($"Bill from {billPay.AccountNumber} to {billPay.PayeeID} has been paid.");
                } else
                {
                    _logger.LogInformation($"Bill from {billPay.AccountNumber} to {billPay.PayeeID} has been paid.");
                    billPay.ScheduleTimeUtc = billPay.ScheduleTimeUtc.AddMonths(1);
                    _logger.LogInformation($"Adding one month");
                }

            } else
            {
                // the bill failed
                _logger.LogInformation($"FAILURE: Bill from {billPay.AccountNumber} to {billPay.PayeeID} has failed.");
                billPay.BillPayStatus = McbaExample.Models.BillPayStatus.Failed;
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Bill Background Service work complete.");
    }
}
