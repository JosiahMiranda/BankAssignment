using System.Text;
using Microsoft.AspNetCore.Mvc;
using McbaExample.Models;
using Newtonsoft.Json;
using McbaExampleWithLogin.Filters;

namespace MvcMovie.Controllers;

[AuthorizeAdmin]
public class BillPaysController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient();

    public BillPaysController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    // GET: Movies/Index
    public async Task<IActionResult> Index()
    {
        var response = await Client.GetAsync("api/billPays");

        if(!response.IsSuccessStatusCode)
            throw new Exception();

        // Storing the response details received from web api.
        var result = await response.Content.ReadAsStringAsync();

        // Deserializing the response received from web api and storing into a list.
        var billPays = JsonConvert.DeserializeObject<List<BillPay>>(result);

        return View(billPays);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult BLockOrUnblock(int id)
    {
        //return RedirectToAction("Index");
        if (ModelState.IsValid)
        {
            var content = new StringContent(JsonConvert.SerializeObject(id), Encoding.UTF8, "application/json");

            var response = Client.PutAsync("api/billPays", content).Result;

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");
        }
        else
        {
            return RedirectToAction("Index");
        }

        return RedirectToAction("Index");
    }

    // GET: Movies/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Movies/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(BillPay billPay)
    {
        if(ModelState.IsValid)
        {
            var content = new StringContent(JsonConvert.SerializeObject(billPay), Encoding.UTF8, "application/json");

            var response = Client.PostAsync("api/billPays", content).Result;

            if(response.IsSuccessStatusCode)
                return RedirectToAction("Index");
        }

        return View(billPay);
    }
        
    // GET: Movies/Edit/1
    public async Task<IActionResult> Edit(int? id)
    {
        if(id == null)
            return NotFound();

        var response = await Client.GetAsync($"api/billPays/{id}");

        if(!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var billPay = JsonConvert.DeserializeObject<BillPay>(result);

        return View(billPay);
    }

    // POST: Movies/Edit/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, BillPay billPay)
    {
        if (id != billPay.BillPayID)
            return NotFound();

        if(ModelState.IsValid)
        {
            var content = new StringContent(JsonConvert.SerializeObject(billPay), Encoding.UTF8, "application/json");

            var response = Client.PutAsync("api/billPays", content).Result;

            if(response.IsSuccessStatusCode)
                return RedirectToAction("Index");
        }

        return View(billPay);
    }

    // GET: Movies/Delete/1
    public async Task<IActionResult> Delete(int? id)
    {
        if(id == null)
            return NotFound();

        var response = await Client.GetAsync($"api/billPays/{id}");

        if(!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var billPay = JsonConvert.DeserializeObject<BillPay>(result);

        return View(billPay);
    }

    // POST: Movies/Delete/1
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var response = Client.DeleteAsync($"api/billPays/{id}").Result;

        if(response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        return NotFound();
    }
}
