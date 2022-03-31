using Microsoft.AspNetCore.Mvc;
using McbaExample.Models;
using MvcMovie.Models.DataManager;

namespace MvcMovie.Api.Controllers;

// See here for more information:
// https://docs.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-6.0

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly AccountManager _repo;

    public AccountsController(AccountManager repo)
    {
        _repo = repo;
    }

    // GET: api/accounts
    [HttpGet]
    public IEnumerable<Account> Get()
    {
        return _repo.GetAll();
    }

    // GET api/accounts/1
    [HttpGet("{id}")]
    public Account Get(int id)
    {
        return _repo.Get(id);
    }

    // POST api/accounts
    [HttpPost]
    public void Post([FromBody] Account account)
    {
        _repo.Add(account);
    }

    // PUT api/accounts
    [HttpPut]
    public void Put([FromBody] Account account)
    {
        _repo.Update(account.AccountNumber, account);
    }

    // DELETE api/accounts/1
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }
}
