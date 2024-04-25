using EcommerceWebSite.Dashboard.Models;
using EcommerceWebSite.Dashboard.ViewModel;
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
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration.GetValue<string>("ApiBaseUrl");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        // POST: Submits the registration form
        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserDto model)
        {
            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/account/addSeller", content);

                if (response.IsSuccessStatusCode)
                {
                    // Redirect or show a success message
                    return RedirectToAction("Login");
                }
                else
                {
                    // Handle API errors (e.g., display them to the user)
                    ModelState.AddModelError(string.Empty, "An error occurred while registering.");
                }
            }

            // If we got this far, something failed; redisplay form
            return View(model);
        }

        [HttpGet]
        public IActionResult addAdmin()
        {
            return View();
        }
        // POST: Submits the registration form
        [HttpPost]
        public async Task<IActionResult> addAdmin(RegisterUserDto model)
        {
            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/account/addAdmin", content);

                if (response.IsSuccessStatusCode)
                {
                    // Set success message
                    TempData["SuccessMessage"] = "Admin registered successfully.";
                    return RedirectToAction("addAdmin");

                }
                else
                {
                    // Handle API errors (e.g., display them to the user)
                    ModelState.AddModelError(string.Empty, "An error occurred while registering.");
                }
            }

            // If we got this far, something failed; redisplay form
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/account/login", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var tokenData = JsonConvert.DeserializeObject<TokenResponse>(responseData);
                        
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenData.token);
                        HttpContext.Session.SetString("AccessToken", tokenData.token);
                        // Redirect to dashboard or another protected area
                        return RedirectToAction("getCategory", "Category");
                    }
                    else
                    {
                        // Handle failed login (e.g., display error message)
                        ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., display error message)
                    ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                }
            }

            // If we got this far, something failed; redirect to login page
            return RedirectToAction("Login", "Account");
        }
    

    public IActionResult Index()
        {
            return View();
        }
    }
}
