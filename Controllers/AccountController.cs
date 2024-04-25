using EcommerceWebSite.Dashboard.Models;
using EcommerceWebSite.Dashboard.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
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
            var islogged = HttpContext.Session.GetString("email");
            if (!string.IsNullOrEmpty(islogged))
            {
                return RedirectToAction("index", "home");
            }
            return View();
        }
        // POST: Submits the registration form
        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserDto model)
        {
            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/account/addAdmin", content);

                if (response.IsSuccessStatusCode)
                {
                    // WAEL FIX
                    return RedirectToAction("getCategory", "Category");
                }
                else
                {
                    var err = await response.Content.ReadAsStringAsync();
                    // Handle API errors (e.g., display them to the user)
                    ModelState.AddModelError(string.Empty, $"ERROR: {err}. ");
                }
            }

            // If we got this far, something failed; redisplay form
            return View(model);
        }

        [LoggedinOnly]
        [HttpGet]
        public IActionResult addAdmin()
        {
            return View();
        }
        [LoggedinOnly]
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
                    var err = await response.Content.ReadAsStringAsync();
                    // Handle API errors (e.g., display them to the user)
                    ModelState.AddModelError(string.Empty, $"ERROR: {err}. ");
                }
            }

            // If we got this far, something failed; redisplay form
            return View(model);
        }

        public IActionResult Login()
        {
            var islogged = HttpContext.Session.GetString("email");
            if (!string.IsNullOrEmpty(islogged))
            {
                return RedirectToAction("index", "home");
            }
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
                        var Roles = await _httpClient.PostAsync($"{_apiBaseUrl}/api/account/getRole", content);
                        var RoleList = await Roles.Content.ReadAsStringAsync();
                        if (!RoleList.ToLower().Contains("admin"))
                        {
                            ModelState.AddModelError(string.Empty, $"Only Admins allowed to use Dashboard");
                            return View(model);
                        }

                        var responseData = await response.Content.ReadAsStringAsync();
                        var tokenData = JsonConvert.DeserializeObject<TokenResponse>(responseData);

                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenData.token);
                        HttpContext.Session.SetString("AccessToken", tokenData.token);
                        HttpContext.Session.SetString("email", model.UserEmail);
                        // Redirect to dashboard or another protected area
                        return RedirectToAction("index", "home");
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

            // Wael FIX
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        [LoggedinOnly]
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
