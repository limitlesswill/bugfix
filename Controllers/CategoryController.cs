using EcommerceWebSite.Dashboard.Models;
using EcommerceWebSite.Dashboard.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebSite.Dashboard.Controllers
{
    
    public class CategoryController : Controller
    {

        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public CategoryController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration.GetValue<string>("ApiBaseUrl");
        }
        //[Authorize]
        public IActionResult addCategory()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> addCategory(addOrUpdateCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var token = HttpContext.Session.GetString("AccessToken");
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/Category", content);

                    if (response.IsSuccessStatusCode)
                    {

                        return RedirectToAction("getCategory");
                    }
                    else
                    {

                        // Handle API errors (e.g., display them to the user)
                        ModelState.AddModelError(string.Empty, "An error occurred while adding.");
                    }
                }
                    
            }

            // If we got this far, something failed; redisplay form
            return View(model);
        }
        //[Authorize(Roles = "Seller")]
        public async Task<IActionResult> getCategory()
        {
            var cats = new List<Category>();

            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Category");
                response.EnsureSuccessStatusCode(); // Throw an exception if not successful.

                var responseString = await response.Content.ReadAsStringAsync();
                cats = JsonConvert.DeserializeObject<List<Category>>(responseString);
            }
            catch (HttpRequestException e)
            {
                // Log and handle the exception
                // You might want to return a view with an error message
                ModelState.AddModelError(string.Empty, "An error occurred while fetching the users.");
            }
            // Pass the API base URL to the view
            ViewBag.ApiBaseUrl = $"{_apiBaseUrl}/api/Category";
            return View(cats);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            addOrUpdateCategoryViewModel cat = null;
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Category/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                cat = JsonConvert.DeserializeObject<addOrUpdateCategoryViewModel>(content);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while Edit the category.");
            }

            return View(cat);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(addOrUpdateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiBaseUrl}/api/Category/{model.id}", content);

            if (response.IsSuccessStatusCode)
            {
                // Redirect to a confirmation page or back to the list
                return RedirectToAction("getCategory"); // Redirect to the user list page, adjust as needed
            }
            else
            {
                // Handle HTTP error response
                ModelState.AddModelError("", "An error occurred while updating the category.");
                return View(model);
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
