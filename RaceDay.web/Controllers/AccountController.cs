
using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using System.Net.Http.Json;

namespace RaceDay.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("RaceDayAPI");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/auth/login", request);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Invalid email or password. Please try again.";
                return View(request);
            }

            var loginResponse =
                await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (loginResponse == null)
            {
                ViewBag.Error = "Unable to read the login response.";
                return View(request);
            }

            HttpContext.Session.SetString("Token", loginResponse.Token);
            HttpContext.Session.SetString("Role", loginResponse.Role);
            HttpContext.Session.SetInt32("UserId", loginResponse.UserId);

            return RedirectToAction("Index", "Home");
        }
    }
}
