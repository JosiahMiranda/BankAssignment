using Microsoft.AspNetCore.Mvc;
using McbaExample.Models;
using MvcMovie.Models.DataManager;

namespace MvcMovie.Api.Controllers;

// See here for more information:
// https://docs.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-6.0

[ApiController]
[Route("api/[controller]")]
public class LoginsController : ControllerBase
{
    private readonly LoginManager _repo;

    public LoginsController(LoginManager repo)
    {
        _repo = repo;
    }

    // GET: api/logins
    [HttpGet]
    public IEnumerable<Login> Get()
    {
        return _repo.GetAll();
    }

    // GET api/logins/1
    [HttpGet("{id}")]
    public Login Get(string id)
    {
        return _repo.Get(id);
    }

    // POST api/logins
    [HttpPost]
    public void Post([FromBody] Login login)
    {
        _repo.Add(login);
    }

    // PUT api/logins
    [HttpPut]
    public void Put([FromBody] string id)
    {
        _repo.LockOrUnlock(id);
    }

    // DELETE api/logins/1
    [HttpDelete("{id}")]
    public string Delete(string id)
    {
        return _repo.Delete(id);
    }
}
