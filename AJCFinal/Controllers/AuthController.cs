using Microsoft.AspNetCore.Mvc;
using AJCFinal.Models.Auth;
using AJCFinal.Models.Person;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

public class AuthController : Controller
{
    private readonly HttpClient httpClient;

    public AuthController(IHttpClientFactory httpClientFactory)
    {
        this.httpClient = httpClientFactory.CreateClient("MyApiClient");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var response = await httpClient.PostAsJsonAsync("api/auth/login", model);

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                // Here you would typically decode the JWT token and extract claims
                // For simplicity, we'll just use the email as the identifier
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim("AccessToken", token)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(PersonInputViewModel model)
    {
        if (ModelState.IsValid)
        {
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(model.Email), "Email");
            formData.Add(new StringContent(model.Password), "Password");
            formData.Add(new StringContent(model.FirstName), "FirstName");
            formData.Add(new StringContent(model.LastName), "LastName");
            formData.Add(new StringContent(model.DateOfBirth.ToString("yyyy-MM-dd")), "DateOfBirth");

            if (model.Image != null)
            {
                var imageContent = new StreamContent(model.Image.OpenReadStream());
                formData.Add(imageContent, "Image", model.Image.FileName);
            }

            var response = await httpClient.PostAsync("api/auth/register", formData);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Login));
            }

            ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}