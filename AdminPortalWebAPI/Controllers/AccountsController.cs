using System.Text;
using Microsoft.AspNetCore.Mvc;
using McbaExample.Models;
using Newtonsoft.Json;
using McbaExampleWithLogin.Filters;

namespace MvcMovie.Controllers;

[AuthorizeAdmin]
public class AccountsController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient();

    public AccountsController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    // GET: Movies/Index
    public async Task<IActionResult> Index()
    {
        var response = await Client.GetAsync("api/accounts");

        if(!response.IsSuccessStatusCode)
            throw new Exception();

        // Storing the response details received from web api.
        var result = await response.Content.ReadAsStringAsync();

        // Deserializing the response received from web api and storing into a list.
        var accounts = JsonConvert.DeserializeObject<List<Account>>(result);

        return View(accounts);
    }

    // GET: Movies/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Movies/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Account account)
    {
        if(ModelState.IsValid)
        {
            var content = new StringContent(JsonConvert.SerializeObject(account), Encoding.UTF8, "application/json");

            var response = Client.PostAsync("api/accounts", content).Result;

            if(response.IsSuccessStatusCode)
                return RedirectToAction("Index");
        }

        return View(account);
    }
        
    // GET: Movies/Edit/1
    public async Task<IActionResult> Edit(int? id)
    {
        if(id == null)
            return NotFound();

        var response = await Client.GetAsync($"api/accounts/{id}");

        if(!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var account = JsonConvert.DeserializeObject<Account>(result);

        return View(account);
    }

    // POST: Movies/Edit/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Account account)
    {
        if(id != account.AccountNumber)
            return NotFound();

        if(ModelState.IsValid)
        {
            var content = new StringContent(JsonConvert.SerializeObject(account), Encoding.UTF8, "application/json");

            var response = Client.PutAsync("api/accounts", content).Result;

            if(response.IsSuccessStatusCode)
                return RedirectToAction("Index");
        }

        return View(account);
    }

    // GET: Movies/Delete/1
    public async Task<IActionResult> Delete(int? id)
    {
        if(id == null)
            return NotFound();

        var response = await Client.GetAsync($"api/accounts/{id}");

        if(!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var account = JsonConvert.DeserializeObject<Account>(result);

        return View(account);
    }

    // POST: Movies/Delete/1
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var response = Client.DeleteAsync($"api/accounts/{id}").Result;

        if(response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        return NotFound();
    }
}
