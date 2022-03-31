using System.Text;
using Microsoft.AspNetCore.Mvc;
using McbaExample.Models;
using Newtonsoft.Json;
using McbaExampleWithLogin.Filters;

namespace MvcMovie.Controllers;

[AuthorizeAdmin]
public class CustomersController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient();

    public CustomersController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    // GET: Movies/Index
    public async Task<IActionResult> Index()
    {
        var response = await Client.GetAsync("api/customers");

        if(!response.IsSuccessStatusCode)
            throw new Exception();

        // Storing the response details received from web api.
        var result = await response.Content.ReadAsStringAsync();

        // Deserializing the response received from web api and storing into a list.
        var customers = JsonConvert.DeserializeObject<List<Customer>>(result);

        return View(customers);
    }

    // GET: Movies/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Movies/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Customer customer)
    {
        if(ModelState.IsValid)
        {
            var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");

            var response = Client.PostAsync("api/customers", content).Result;

            if(response.IsSuccessStatusCode)
                return RedirectToAction("Index");
        }

        return View(customer);
    }
        
    // GET: Movies/Edit/1
    public async Task<IActionResult> Edit(int? id)
    {
        if(id == null)
            return NotFound();

        var response = await Client.GetAsync($"api/customers/{id}");

        if(!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var customer = JsonConvert.DeserializeObject<Customer>(result);

        return View(customer);
    }

    // POST: Movies/Edit/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Customer customer)
    {
        if (id != customer.CustomerID)
            return NotFound();

        if(ModelState.IsValid)
        {
            var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");

            var response = Client.PutAsync("api/customers", content).Result;

            if(response.IsSuccessStatusCode)
                return RedirectToAction("Index");
        }

        return View(customer);
    }

    // GET: Movies/Delete/1
    public async Task<IActionResult> Delete(int? id)
    {
        if(id == null)
            return NotFound();

        var response = await Client.GetAsync($"api/customers/{id}");

        if(!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var customer = JsonConvert.DeserializeObject<Customer>(result);

        return View(customer);
    }

    // POST: Movies/Delete/1
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var response = Client.DeleteAsync($"api/customers/{id}").Result;

        if(response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        return NotFound();
    }
}
