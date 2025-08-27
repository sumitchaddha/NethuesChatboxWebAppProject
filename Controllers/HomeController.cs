using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nethues_ChatboxWebApp.Models;
using Nethues_ChatboxWebApp.Services.Interface;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Nethues_ChatboxWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserRepository _userRepository;

        public HomeController(ILogger<HomeController> logger, IJwtTokenService jwtTokenService, IUserRepository userRepository)
        {
            _logger = logger;
            _jwtTokenService = jwtTokenService;
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public JsonResult EvaluateExpression([FromBody] ExpressionRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var sanitized = Regex.Replace(request.Expression, @"[^0-9+\-*/%<>=!(). ]", "");
                var result = new DataTable().Compute(sanitized, null);
                var getresultinwords = result.ToString();
                if (result is not bool)
                {
                     getresultinwords = _jwtTokenService.NumberToWords((int)result);
                }
                
                _userRepository.LogAction(userId, sanitized, getresultinwords ?? "Invalid");
                if (string.IsNullOrEmpty(result.ToString()))
                {
                    return Json(new { result = "Invalid" });
                }
                
                return Json(new { result = result.ToString() + ", " + getresultinwords
            });
                
            }
            catch
            {
                return Json(new { result = "Invalid expression" });
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult History()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var result = _userRepository.GetUserActionsHistory(userId);
            
            return Ok(result);
        }

        public class ExpressionRequest
        {
            public string Expression { get; set; }
        }

    }
}
