using System.Text;
using Microsoft.AspNetCore.Mvc;
using McbaExample.Models;
using Newtonsoft.Json;
using McbaExampleWithLogin.Filters;

namespace MvcMovie.Controllers;

[AuthorizeAdmin]
public class TransactionsController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient();

    public TransactionsController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    // GET: Movies/Index
    public async Task<IActionResult> Index()
    {
        var response = await Client.GetAsync("api/transactions");

        if(!response.IsSuccessStatusCode)
            throw new Exception();

        // Storing the response details received from web api.
        var result = await response.Content.ReadAsStringAsync();

        // Deserializing the response received from web api and storing into a list.
        var transactions = JsonConvert.DeserializeObject<List<Transaction>>(result);

        return View(transactions);
    }

    // Endpoint to return the transactions for an account number passed into the route
    public async Task<IActionResult> GetForAccount(int? id, string toDate, string fromDate)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }


        var response = await Client.GetAsync($"api/transactions/{id}?toDate={toDate}&fromDate={fromDate}");

        if (!response.IsSuccessStatusCode)
            throw new Exception();

        ViewBag.AccountNumber = id;
        ViewBag.ToDate = toDate;
        ViewBag.FromDate = fromDate;

        // Storing the response details received from web api.
        var result = await response.Content.ReadAsStringAsync();

        // Deserializing the response received from web api and storing into a list.
        var transactions = JsonConvert.DeserializeObject<List<Transaction>>(result);

        return View(transactions);
    }

    // GET: Movies/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Movies/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Transaction transaction)
    {
        if(ModelState.IsValid)
        {
            var content = new StringContent(JsonConvert.SerializeObject(transaction), Encoding.UTF8, "application/json");

            var response = Client.PostAsync("api/transactions", content).Result;

            if(response.IsSuccessStatusCode)
                return RedirectToAction("Index");
        }

        return View(transaction);
    }
        
    // GET: Movies/Edit/1
    public async Task<IActionResult> Edit(int? id)
    {
        if(id == null)
            return NotFound();

        var response = await Client.GetAsync($"api/transactions/{id}");

        if(!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var transaction = JsonConvert.DeserializeObject<Transaction>(result);

        return View(transaction);
    }

    // POST: Movies/Edit/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Transaction transaction)
    {
        if(id != transaction.TransactionID)
            return NotFound();

        if(ModelState.IsValid)
        {
            var content = new StringContent(JsonConvert.SerializeObject(transaction), Encoding.UTF8, "application/json");

            var response = Client.PutAsync("api/transactions", content).Result;

            if(response.IsSuccessStatusCode)
                return RedirectToAction("Index");
        }

        return View(transaction);
    }

    // GET: Movies/Delete/1
    public async Task<IActionResult> Delete(int? id)
    {
        if(id == null)
            return NotFound();

        var response = await Client.GetAsync($"api/transactions/{id}");

        if(!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var transaction = JsonConvert.DeserializeObject<Transaction>(result);

        return View(transaction);
    }

    // POST: Movies/Delete/1
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var response = Client.DeleteAsync($"api/transactions/{id}").Result;

        if(response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        return NotFound();
    }
}
