using EcommerceWebSite.Dashboard.Models;
using EcommerceWebSite.Dashboard.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebSite.Dashboard.Controllers
{
    [LoggedinOnly]
    public class OrderController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public OrderController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration.GetValue<string>("ApiBaseUrl");
        }
        public async Task<IActionResult> getOrders()
        {
            var cats = new List<Order>();

            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Order");
                response.EnsureSuccessStatusCode(); // Throw an exception if not successful.

                var responseString = await response.Content.ReadAsStringAsync();
                cats = JsonConvert.DeserializeObject<List<Order>>(responseString);
            }
            catch (HttpRequestException e)
            {
              
                ModelState.AddModelError(string.Empty, "An error occurred while fetching the users.");
            }
            // Pass the API base URL to the view
            ViewBag.ApiBaseUrl = $"{_apiBaseUrl}/api/Order";
            return View(cats);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            OrderUserDto cat = null;
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Order/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                cat = JsonConvert.DeserializeObject<OrderUserDto>(content);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while Edit the category.");
            }

            return View(cat);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(OrderUserDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiBaseUrl}/api/Order/{model.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                // Redirect to a confirmation page or back to the list
                return RedirectToAction("getOrders"); // Redirect to the user list page, adjust as needed
            }
            else
            {
                // Handle HTTP error response
                ModelState.AddModelError("", "An error occurred while updating the order.");
                return View(model);
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
