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
    public class RoleController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public RoleController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration.GetValue<string>("ApiBaseUrl");
        }
        public IActionResult addRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> addRole(addRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/Role/CreateRole", content);

                if (response.IsSuccessStatusCode)
                {

                    return RedirectToAction("Dashboard", "Dashboard");
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


        public IActionResult Index()
        {
            return View();
        }
    }
}
