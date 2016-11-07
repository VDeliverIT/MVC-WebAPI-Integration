using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using MvcApiIntegration.Models;

namespace MvcApiIntegration.Controllers
{
    public class HomeController : Controller
    {
        HttpClient Client;
        string BaseUrl;
        public HomeController()
        {
            Client = new HttpClient();
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            BaseUrl = "http://localhost:1946/api/v1/todo";
        }
        public async Task<ActionResult> Index()
        {
            Client.BaseAddress = new Uri(BaseUrl);
            HttpResponseMessage responseMessage = await Client.GetAsync(BaseUrl);
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var todoItems = JsonConvert.DeserializeObject<List<TodoItem>>(responseData);
                var viewModel = new TodoGenreViewModel { items = todoItems };
                return View(viewModel);
            }
            return View("Error");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,IsComplete")] TodoItem item)
        {
            if (ModelState.IsValid)
            {
                Client.BaseAddress = new Uri(BaseUrl);
                HttpResponseMessage responseMessage = await Client.PostAsJsonAsync(BaseUrl, item);
                if (responseMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return View("Error");
        }
        public async Task<IActionResult> Edit(string key)
        {
            string url = BaseUrl + "/" + key;
            Client.BaseAddress = new Uri(url);
            HttpResponseMessage responseMessage = await Client.GetAsync(url);
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var todoItem = JsonConvert.DeserializeObject<TodoItem>(responseData);
                return View(todoItem);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string key, [Bind("Name,IsComplete,Key")] TodoItem aTodoItem)
        {
            if (ModelState.IsValid)
            {
                string url = BaseUrl +"/" + key;
                Client.BaseAddress = new Uri(url);
                HttpResponseMessage responseMessage = await Client.PutAsJsonAsync(url, aTodoItem);
                if (responseMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return View("Error");
        }

        public async Task<ActionResult> Delete(string key)
        {
            string url = BaseUrl + "/" + key;
            Client.BaseAddress = new Uri(url);
            HttpResponseMessage responseMessage = await Client.GetAsync(url);
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<TodoItem>(responseData);
                return View(item);
            }
            return View("Error");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string key, [Bind("Name,IsComplete,Key")] TodoItem aTodoItem)
        {
            string url = BaseUrl + "/" + key;
            Client.BaseAddress = new Uri(url);
            HttpResponseMessage responseMessage = await Client.DeleteAsync(url);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View("Error");
        }
    }
}
