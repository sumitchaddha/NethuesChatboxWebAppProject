using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Nethues_ChatboxWebApp.Models;
using Nethues_ChatboxWebApp.Services.Implementation;
using Nethues_ChatboxWebApp.Services.Interface;
using System.Data.SQLite;
using System.Security.Claims;

namespace NethuesChatboxWebApp.Controllers
{
    public class AccountController : Controller
    {
        private static string? emailAddress { get; set; }
        private readonly IUserRepository _repo;
        private readonly IJwtTokenService _jwt;
        private readonly IConfiguration _config;

        public AccountController(IUserRepository repo, IJwtTokenService jwt, IConfiguration config)
        {
            _repo = repo;
            _jwt = jwt;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register([FromForm] RegisterDto user)
        {
            var userDetails = _repo.CreateUser(user.Email, user.Password, user.FirstName, user.LastName);
            TempData["SuccessMessage"] = "Registration successful!";
            return RedirectToAction("Register");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([FromForm] LoginDto userDto)
        {
            if (!ModelState.IsValid)
                return View(userDto); // Return with validation errors

            var user = _repo.ValidateUser(userDto.Username, userDto.PasswordHash);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View(userDto);
            }

            var token = _jwt.CreateToken(user);
            var refresh = _jwt.CreateRefreshToken(user.Id);
            _repo.SaveRefreshToken(refresh);
            var tokenexpiryTime = _config.GetSection("Jwt:ExpiresMinutes").Value;
            Response.Cookies.Append("AccessToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(tokenexpiryTime))
            });

            return RedirectToAction("Dashboard", "Home");
        }


        [HttpPost]
        public IActionResult Refresh([FromBody] string refreshToken)
        {
            var storedToken = _repo.GetRefreshToken(refreshToken);
            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresUtc < DateTime.UtcNow)
                return Unauthorized();

            var user = _repo.GetUserById(storedToken.UserId);
            var newAccessToken = _jwt.CreateToken(user);
            var newRefreshToken = _jwt.CreateRefreshToken(user.Id);

            storedToken.IsRevoked = true;
            _repo.SaveRefreshToken(newRefreshToken);

            return Ok(new { token = newAccessToken, refresh = newRefreshToken.Token });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

             _repo.UpdateRefreshToken(userId!);

            return RedirectToAction("Index", "Home");
           
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public  IActionResult ChangePassword(string currentPassword, string newPassword)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var userdetails = _repo.GetUserById(userId);
           
            if(userdetails == null)
                return NotFound("User not found.");

            if (userdetails.PasswordHash == null || !BCrypt.Net.BCrypt.Verify(currentPassword, userdetails.PasswordHash))
                return Unauthorized("Invalid current password.");

            _repo.UpdatePassword(userId, newPassword);

            _repo.UpdateRefreshToken(userId);

            return RedirectToAction("Dashboard", "Home");
        }
    }
}
