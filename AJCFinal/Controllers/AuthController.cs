using Microsoft.AspNetCore.Mvc;
using AJCFinal.Models.Auth;
using AJCFinal.Models.Person;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.Json;
using System.Text;

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

            var response = await httpClient.PostAsJsonAsync("api/Auth/login", model);

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<PersonBaseViewModel>();
                if (user != null)
                {
                    var token = await response.Content.ReadAsStringAsync();
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim("AccessToken", token),
                    new Claim("UserId", user.Id.ToString() )
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
            var personInput = new
            {
                Email = model.Email,
                HashedPassword = model.Password, // Note: You should hash this password
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth
            };

            var httpContent = new StringContent(JsonSerializer.Serialize(personInput), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("api/Auth/register", httpContent);

            if (response.IsSuccessStatusCode)
            {
                if (model.Image != null)
                {
                    await using var memoryStream = new MemoryStream();
                    await model.Image.CopyToAsync(memoryStream);
                    await this.httpClient.PostAsJsonAsync("api/Files", new
                    {
                        Name = model.Email,
                        Content = Convert.ToBase64String(memoryStream.ToArray()),
                        ContentType = model.Image.ContentType
                    });
                }
                return RedirectToAction(nameof(Login));
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Registration failed. Server response: {errorContent}");
            }
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