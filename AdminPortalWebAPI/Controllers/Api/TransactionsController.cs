using Microsoft.AspNetCore.Mvc;
using McbaExample.Models;
using MvcMovie.Models.DataManager;

namespace MvcMovie.Api.Controllers;

// See here for more information:
// https://docs.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-6.0

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly TransactionManager _repo;

    public TransactionsController(TransactionManager repo)
    {
        _repo = repo;
    }

    // GET: api/transactions
    [HttpGet]
    public IEnumerable<Transaction> Get()
    {
        return _repo.GetAll();
    }

    // GET api/transactions/1
    [HttpGet("{id}")]
    public IEnumerable<Transaction> Get(int id, string fromDate, string toDate)
    {
        return _repo.GetAll(id, fromDate, toDate);
    }

    // POST api/transactions
    [HttpPost]
    public void Post([FromBody] Transaction transaction)
    {
        _repo.Add(transaction);
    }

    // PUT api/transactions
    [HttpPut]
    public void Put([FromBody] Transaction transaction)
    {
        _repo.Update(transaction.TransactionID, transaction);
    }

    // DELETE api/transactions/1
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }
}
