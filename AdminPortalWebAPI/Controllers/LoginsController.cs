using System.Text;
using Microsoft.AspNetCore.Mvc;
using McbaExample.Models;
using Newtonsoft.Json;
using McbaExampleWithLogin.Filters;

namespace MvcMovie.Controllers;

[AuthorizeAdmin]
public class LoginsController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient();

    public LoginsController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    // GET: Movies/Index
    public async Task<IActionResult> Index()
    {
        var response = await Client.GetAsync("api/logins");

        if(!response.IsSuccessStatusCode)
            throw new Exception();

        // Storing the response details received from web api.
        var result = await response.Content.ReadAsStringAsync();

        // Deserializing the response received from web api and storing into a list.
        var logins = JsonConvert.DeserializeObject<List<Login>>(result);

        return View(logins);
    }

    // GET: Movies/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Movies/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Login login)
    {
        if(ModelState.IsValid)
        {
            var content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");

            var response = Client.PostAsync("api/logins", content).Result;

            if(response.IsSuccessStatusCode)
                return RedirectToAction("Index");
        }

        return View(login);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult LockOrUnlock(string id)
    {
        //return RedirectToAction("Index");
        if (ModelState.IsValid)
        {
            var content = new StringContent(JsonConvert.SerializeObject(id), Encoding.UTF8, "application/json");

            var response = Client.PutAsync("api/logins", content).Result;

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");
        } else
        {
            return RedirectToAction("Index");
        }

        return RedirectToAction("Index");
    }

    // GET: Movies/Edit/1
    public async Task<IActionResult> Edit(string id)
    {
        if(id == null)
            return NotFound();

        var response = await Client.GetAsync($"api/logins/{id}");

        if(!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var login = JsonConvert.DeserializeObject<Login>(result);

        return View(login);
    }

    // POST: Movies/Edit/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(string id, Login login)
    {
        if(id != login.LoginID)
            return NotFound();

        if(ModelState.IsValid)
        {
            var content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");

            var response = Client.PutAsync("api/logins", content).Result;

            if(response.IsSuccessStatusCode)
                return RedirectToAction("Index");
        }

        return View(login);
    }

    // GET: Movies/Delete/1
    public async Task<IActionResult> Delete(string id)
    {
        if(id == null)
            return NotFound();

        var response = await Client.GetAsync($"api/logins/{id}");

        if(!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var login = JsonConvert.DeserializeObject<Login>(result);

        return View(login);
    }

    // POST: Movies/Delete/1
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(string id)
    {
        var response = Client.DeleteAsync($"api/logins/{id}").Result;

        if(response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        return NotFound();
    }
}
