using Microsoft.AspNetCore.Mvc;
using McbaExample.Models;
using MvcMovie.Models.DataManager;

namespace MvcMovie.Api.Controllers;

// See here for more information:
// https://docs.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-6.0

[ApiController]
[Route("api/[controller]")]
public class BillPaysController : ControllerBase
{
    private readonly BillPayManager _repo;

    public BillPaysController(BillPayManager repo)
    {
        _repo = repo;
    }

    // GET: api/billPays
    [HttpGet]
    public IEnumerable<BillPay> Get()
    {
        return _repo.GetAll();
    }

    // GET api/billPays/1
    [HttpGet("{id}")]
    public BillPay Get(int id)
    {
        return _repo.Get(id);
    }

    // POST api/billPays
    [HttpPost]
    public void Post([FromBody] BillPay billPay)
    {
        _repo.Add(billPay);
    }

    // PUT api/billPays
    [HttpPut]
    public void Put([FromBody] int id)
    {
        _repo.BlockOrUnblock(id);
    }

    // DELETE api/billPays/1
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }
}
