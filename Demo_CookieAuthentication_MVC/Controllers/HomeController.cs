using Demo_CookieAuthentication_MVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Demo_CookieAuthentication_MVC.Controllers
{
    //[AllowAnonymous]
    [Route("[Controller]")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("privacy")]
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }


        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public IActionResult ValidateLogin(LoginRequest request)
        {
            if (request.Username != "vnaj1610")
            {
                return Unauthorized();
            }
            if (request.Password == "123")
            {
                var vnajClaim = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, request.Username),
                    new Claim(ClaimTypes.Role, "User"),
                    new Claim("MyCustomClaim", "MyCustomValue"),
                    new Claim("Location", "PE")
                };

                var vnajIdentity = new ClaimsIdentity(vnajClaim, "VNAJ Identity");
                var user = new ClaimsPrincipal(new[] { vnajIdentity });

                HttpContext.SignInAsync(user);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                var vnajClaim = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, request.Username),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim(ClaimTypes.Role, "User"),
                    new Claim("MyCustomClaim", "MyCustomValue"),
                    new Claim("Location", "CO")
                };

                var vnajIdentity = new ClaimsIdentity(vnajClaim, "VNAJ Identity");
                var user = new ClaimsPrincipal(new[] { vnajIdentity });

                HttpContext.SignInAsync(user);

                return RedirectToAction("Index", "Home");
            }
        }


        [HttpGet("logout")]
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync();

            return RedirectToAction("Login", "Home");
        }

        [HttpGet("myinfo")]
        [Authorize(Roles = "User")]
        public IActionResult MyInfo()
        {
            var user = User.Identities.FirstOrDefault().Claims;
            var list = User.Claims.ToList();
            var response = "";
            foreach (var claim in list)
            {
                response += $"Type: {claim.Type} - Value: {claim.Value} \n";
            }

            return Ok(response);
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult MyInfoOnlyAdmin()
        {

            return Ok("Only for Admins");
        }

        [HttpGet("peru")]
        [Authorize(Policy = "Only4PE")]
        public IActionResult OnlyForPE()
        {

            return Ok("Only for PE Users");
        }

        [HttpGet("colombia")]
        [Authorize(Policy = "Only4CO")]
        public IActionResult OnlyForCO()
        {

            return Ok("Only for CO Users");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
