using EcommerceWebSite.Dashboard.Models;
using EcommerceWebSite.Dashboard.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace EcommerceWebSite.Dashboard.Controllers
{
    [LoggedinOnly]
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
                List<string> allowedExtensions = new List<string>() { "jpg", "jpeg", "gif", "png", "webp", "bmp", "bpg", "ico", "img", "svg", "pns", "jps", "jpe", "pnz" };

                if (!allowedExtensions.Contains(Path.GetExtension(model.imgURL.FileName)?.TrimStart('.').ToLower()) || !allowedExtensions.Contains(Path.GetExtension(model.imgURLAr.FileName)?.TrimStart('.').ToLower()) || !model.imgURL.FileName.Contains('.') || !model.imgURLAr.FileName.Contains('.'))
                {
                    ModelState.AddModelError(string.Empty, $"ERROR: please upload Images only: {string.Join(',', allowedExtensions)}");
                    return View(model);
                }

                try
                {
                    var enImagePath = Path.Combine("wwwroot", "images", model.imgURL.FileName);
                    var arImagePath = Path.Combine("wwwroot", "images", model.imgURLAr.FileName);

                    if (System.IO.File.Exists(enImagePath))
                        System.IO.File.Delete(enImagePath);

                    using (var enStream = new FileStream(enImagePath, FileMode.CreateNew))
                        await model.imgURL.CopyToAsync(enStream);

                    if (System.IO.File.Exists(arImagePath))
                        System.IO.File.Delete(arImagePath);

                    using (var arStream = new FileStream(arImagePath, FileMode.CreateNew))
                        await model.imgURLAr.CopyToAsync(arStream);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"ERROR: Host FileStorage Error , details: {ex.Message}");
                    return View(model);
                }
                addCategoryViewModel addjson = new addCategoryViewModel { enName = model.enName, arName = model.arName, imgURL = model.imgURL.FileName, imgURLAr = model.imgURLAr.FileName };

                var content = new StringContent(JsonConvert.SerializeObject(addjson), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/Category", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("getCategory");
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
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error. {ex.Message}");
            }
            // Pass the API base URL to the view
            ViewBag.ApiBaseUrl = $"{_apiBaseUrl}/api/Category";
            return View(cats);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (TempData["hack"] is not null && (TempData["hack"] as string) == "backwards")
            {
                TempData.Remove("hack");
                List<string> allowedExtensions = new List<string>() { "jpg", "jpeg", "gif", "png", "webp", "bmp", "bpg", "ico", "img", "svg", "pns", "jps", "jpe", "pnz" };
                ModelState.AddModelError(string.Empty, $"ERROR: please upload Images only: {string.Join(',', allowedExtensions)}");
            }

            addCategoryViewModel cat = null;
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Category/{id}");
            var content = await response.Content.ReadAsStringAsync();
            if (content == string.Empty || !response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, $"ERROR: {content}. ");
                return View();
            }
            cat = JsonConvert.DeserializeObject<addCategoryViewModel>(content);

            addOrUpdateCategoryViewModel tmpCat = new addOrUpdateCategoryViewModel
            {
                id = id,
                enName = cat.enName,
                arName = cat.arName,
                imgURL = string.IsNullOrEmpty(cat.imgURL) ? null : new FormFile(new MemoryStream(), 0, 0, "imgURL", cat.imgURL),
                imgURLAr = string.IsNullOrEmpty(cat.imgURLAr) ? null : new FormFile(new MemoryStream(), 0, 0, "imgURLAr", cat.imgURLAr)
            };

            TempData["img"] = tmpCat.imgURL.FileName;
            TempData["imgar"] = tmpCat.imgURLAr.FileName;
            return View(tmpCat);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(addOrUpdateCategoryViewModel model)
        {
            var img = model?.imgURL?.FileName ?? TempData["img"] as string;
            var imgar = model?.imgURLAr?.FileName ?? TempData["imgar"] as string;

            if (model.imgURL is null)
                ModelState.Remove("imgURL");

            if (model.imgURLAr is null)
                ModelState.Remove("imgURLAr");

            List<string> allowedExtensions = new List<string>() { "jpg", "jpeg", "gif", "png", "webp", "bmp", "bpg", "ico", "img", "svg", "pns", "jps", "jpe", "pnz" };

            if (!allowedExtensions.Contains(Path.GetExtension(img)?.TrimStart('.').ToLower()) || !allowedExtensions.Contains(Path.GetExtension(imgar)?.TrimStart('.').ToLower()) || !img.Contains('.') || !imgar.Contains('.'))
            {
                TempData["HACK"] = "backwards";
                return RedirectToAction("Edit", model.id);
            }


            if (!ModelState.IsValid)
                return View(model);

            try
            {
                if (model.imgURL is not null)
                {
                    var enImagePath = Path.Combine("wwwroot", "images", model.imgURL.FileName);

                    if (System.IO.File.Exists(enImagePath))
                        System.IO.File.Delete(enImagePath);

                    using (var enStream = new FileStream(enImagePath, FileMode.CreateNew))
                        await model.imgURL.CopyToAsync(enStream);
                }
                if (model.imgURLAr is not null)
                {
                    var arImagePath = Path.Combine("wwwroot", "images", model.imgURLAr.FileName);

                    if (System.IO.File.Exists(arImagePath))
                        System.IO.File.Delete(arImagePath);

                    using (var arStream = new FileStream(arImagePath, FileMode.CreateNew))
                        await model.imgURLAr.CopyToAsync(arStream);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"ERROR: Host FileStorage Error , details: {ex.Message}");
                return View(model);
            }
            addCategoryViewModel addjson = new addCategoryViewModel { enName = model.enName, arName = model.arName, imgURL = img, imgURLAr = imgar };

            var content = new StringContent(JsonConvert.SerializeObject(addjson), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiBaseUrl}/api/Category/{model.id}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("getCategory");
            }
            else
            {
                // Handle HTTP error response
                ModelState.AddModelError(string.Empty, $"An {response.Content.ReadAsStringAsync()} ");
                return View(model);
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
