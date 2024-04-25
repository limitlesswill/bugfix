using EcommerceWebSite.Dashboard.Models;
using EcommerceWebSite.Dashboard.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebSite.Dashboard.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public ProductController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration.GetValue<string>("ApiBaseUrl");
        }

        public async Task<IActionResult> getProducts()
        {
            var cats = new List<Product>();

            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Products");
                response.EnsureSuccessStatusCode(); // Throw an exception if not successful.

                var responseString = await response.Content.ReadAsStringAsync();
                cats = JsonConvert.DeserializeObject<List<Product>>(responseString);
            }
            catch (HttpRequestException e)
            {
                // Log and handle the exception
                // You might want to return a view with an error message
                ModelState.AddModelError(string.Empty, "An error occurred while fetching the users.");
            }
            // Pass the API base URL to the view
            ViewBag.ApiBaseUrl = $"{_apiBaseUrl}/api/Product";
            return View(cats);
        }

        public async Task<IActionResult> addProduct()
        {
            // Fetch categories
            var responseCat = await _httpClient.GetAsync($"{_apiBaseUrl}/api/SubCategory");
            responseCat.EnsureSuccessStatusCode(); // Throw an exception if not successful.

            var responseString = await responseCat.Content.ReadAsStringAsync();
            var SubCategories = JsonConvert.DeserializeObject<List<SubCategory>>(responseString);

            // Pass the categories to the view
            ViewData["SubCategories"] = SubCategories;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> addProduct(addOrUpdateProductViewModel model, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (image != null && image.Length > 0)
                    {
                        // Save the image file
                        var imagePath = Path.Combine("wwwroot", "images", image.FileName); // Adjust the path as needed
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        // Set the image URL in the model
                        model.imgURL = $"/images/{image.FileName}"; // Adjust the URL format as needed
                        var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                        var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/Product/Create", content);

                        if (response.IsSuccessStatusCode)
                        {

                            return RedirectToAction("getProducts");
                        }
                        else
                        {

                            ModelState.AddModelError(string.Empty, "An error occurred while adding.");
                        }
                    }
                }

                catch (Exception ex)
                {
                    // Log the exception
                    ModelState.AddModelError(string.Empty, "An error occurred while processing the request.");
                }
            }

            // If we got this far, something failed; redisplay form
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Fetch Subcategories
            var responseCat = await _httpClient.GetAsync($"{_apiBaseUrl}/api/SubCategory");
            responseCat.EnsureSuccessStatusCode(); // Throw an exception if not successful.

            var responseString = await responseCat.Content.ReadAsStringAsync();
            var subcategories = JsonConvert.DeserializeObject<List<SubCategory>>(responseString);

            addOrUpdateProductViewModel cat = null;
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Product/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                cat = JsonConvert.DeserializeObject<addOrUpdateProductViewModel>(content);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while Edit the SubCategory.");
            }
            // Pass the categories to the view
            ViewData["SubCategories"] = subcategories;

            return View(cat);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(addOrUpdateProductViewModel model, IFormFile image)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                if (image != null && image.Length > 0)
                {
                    // Save the image file
                    var imagePath = Path.Combine("wwwroot", "images", image.FileName); // Adjust the path as needed
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    // Set the image URL in the model
                    model.imgURL = $"/images/{image.FileName}"; // Adjust the URL format as needed
                }

                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/api/Product/{model.id}", content);

                if (response.IsSuccessStatusCode)
                {
                    // Redirect to a confirmation page or back to the list
                    return RedirectToAction("getProducts"); // Redirect to the product list page, adjust as needed
                }
                else
                {
                    // Handle HTTP error response
                    ModelState.AddModelError("", "An error occurred while updating the product.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                ModelState.AddModelError("", "An error occurred while processing the request.");
                return View(model);
            }
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
