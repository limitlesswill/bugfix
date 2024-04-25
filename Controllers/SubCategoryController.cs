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
    public class SubCategoryController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public SubCategoryController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration.GetValue<string>("ApiBaseUrl");
        }
        public async Task<IActionResult> getSubCategory()
        {
            var cats = new List<SubCategory>();

            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/SubCategory");
                response.EnsureSuccessStatusCode(); // Throw an exception if not successful.

                var responseString = await response.Content.ReadAsStringAsync();
                cats = JsonConvert.DeserializeObject<List<SubCategory>>(responseString);
            }
            catch (HttpRequestException e)
            {
                // Log and handle the exception
                // You might want to return a view with an error message
                ModelState.AddModelError(string.Empty, "An error occurred while fetching the users.");
            }
            // Pass the API base URL to the view
            ViewBag.ApiBaseUrl = $"{_apiBaseUrl}/api/SubCategory";
            return View(cats);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Fetch categories
            var responseCat = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Category");
            responseCat.EnsureSuccessStatusCode(); // Throw an exception if not successful.

            var responseString = await responseCat.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<List<Category>>(responseString);

            addOrUpdateSubCategoryViewModel cat = null;
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/SubCategory/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                cat = JsonConvert.DeserializeObject<addOrUpdateSubCategoryViewModel>(content);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while Edit the SubCategory.");
            }
            // Pass the categories to the view
            ViewData["Categories"] = categories;

            return View(cat);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(addOrUpdateSubCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiBaseUrl}/api/SubCategory/{model.id}", content);

            if (response.IsSuccessStatusCode)
            {
                // Redirect to a confirmation page or back to the list
                return RedirectToAction("getSubCategory"); // Redirect to the user list page, adjust as needed
            }
            else
            {
                // Handle HTTP error response
                ModelState.AddModelError("", "An error occurred while updating the SubCategory.");
                return View(model);
            }
        }


        public async Task<IActionResult> addSubCategory()
        {
            // Fetch categories
            var responseCat = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Category");
            responseCat.EnsureSuccessStatusCode(); // Throw an exception if not successful.

            var responseString = await responseCat.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<List<Category>>(responseString);
            
            // Pass the categories to the view
            ViewData["Categories"] = categories;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> addSubCategory(addOrUpdateSubCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/SubCategory", content);

                if (response.IsSuccessStatusCode)
                {

                    return RedirectToAction("getSubCategory");
                }
                else
                {

                    // Handle API errors (e.g., display them to the user)
                    ModelState.AddModelError(string.Empty, "An error occurred while adding.");
                }
            }

            // If we got this far, something failed; redisplay form
            return View(model);
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
