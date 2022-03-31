using McbaExample.Data;
using McbaExample.Models;
using MvcMovie.Models.Repository;
using System.Globalization;

namespace MvcMovie.Models.DataManager;

public class TransactionManager : IDataRepository<Transaction, int>
{
    private readonly McbaContext _context;

    public TransactionManager(McbaContext context)
    {
        _context = context;
    }

    public Transaction Get(int id)
    {
        return _context.Transactions.Find(id);
    }

    public IEnumerable<Transaction> GetAll()
    {
        return _context.Transactions.ToList();
    }

    // GetAll Method that returns transactions for only a specific account
    public IEnumerable<Transaction> GetAll(int id, string toDate, string fromDate)
    {
        var response = _context.Transactions.Select(x => x).Where(x => x.AccountNumber == id);
        DateTime? toDateTime = null;
        DateTime? fromDateTime = null;
        if (toDate != null)
        {
            toDateTime = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            response = response.Where(x => x.TransactionTimeUtc.Date >= toDateTime.Value.ToUniversalTime());
        }

        if (fromDate != null)
        {
            fromDateTime = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            response = response.Where(x => x.TransactionTimeUtc.Date <= fromDateTime.Value.ToUniversalTime());
        }

        return response.ToList();
    }

    public int Add(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        _context.SaveChanges();

        return transaction.TransactionID;
    }

    public int Delete(int id)
    {
        _context.Transactions.Remove(_context.Transactions.Find(id));
        _context.SaveChanges();

        return id;
    }

    public int Update(int id, Transaction transaction)
    {
        _context.Update(transaction);
        _context.SaveChanges();
            
        return id;
    }
}
