using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Mvc5_reCAPTCHA.Models;
using Mvc5_reCAPTCHA.ViewModels;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;

namespace Mvc5_reCAPTCHA.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public LoginController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }
         
        [HttpPost]
        public async Task<IActionResult> LoginForm(LoginForm form)
        {
            var response = Request.Form["g-recaptcha-response"];
            string secretKey = _configuration.GetSection("reCAPTCHA").GetSection("secret-key").Value;
            using WebClient cliente = new();
            string url = "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}";
            var resultado = cliente.DownloadString(string.Format(url, secretKey, response));
            var obj = JObject.Parse(resultado);
            var status = (bool)obj.SelectToken("success");
            ViewBag.Message = status ? "Foi" : "Deu ruim";
            if (status)
            {
                return Redirect("/Home/Index");
            }
            return View("Index");
        }
    }
}