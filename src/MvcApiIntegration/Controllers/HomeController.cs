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
        HttpClient client;
        public HomeController()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<ActionResult> Index()
        {
            string url = "http://localhost:1946/api/v1/todo";
            client.BaseAddress = new Uri(url);
            HttpResponseMessage responseMessage = await client.GetAsync(url);
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
                string url = "http://localhost:1946/api/v1/todo";
                client.BaseAddress = new Uri(url);
                HttpResponseMessage responseMessage = await client.PostAsJsonAsync(url, item);
                if (responseMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Error");
            }
            return RedirectToAction("Error");
        }
        public async Task<IActionResult> Edit(string key)
        {
            string url = "http://localhost:1946/api/v1/todo/"+ key;
            client.BaseAddress = new Uri(url);
            HttpResponseMessage responseMessage = await client.GetAsync(url);
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
                string url = "http://localhost:1946/api/v1/todo/" + key;
                client.BaseAddress = new Uri(url);
                HttpResponseMessage responseMessage = await client.PutAsJsonAsync(url, aTodoItem);
                if (responseMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Error");
            }
            return View("Error");
        }

        public async Task<ActionResult> Delete(string key)
        {
            string url = "http://localhost:1946/api/v1/todo/" + key;
            client.BaseAddress = new Uri(url);
            HttpResponseMessage responseMessage = await client.GetAsync(url);
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;

                var item = JsonConvert.DeserializeObject<TodoItem>(responseData);

                return View(item);
            }
            return View("Error");
        }

        //The DELETE method
        [HttpPost]
        public async Task<ActionResult> Delete(string key, [Bind("Name,IsComplete,Key")] TodoItem aTodoItem)
        {
            string url = "http://localhost:1946/api/v1/todo/" + key;
            client.BaseAddress = new Uri(url);
            HttpResponseMessage responseMessage = await client.DeleteAsync(url);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Error");
        }
    }
}
