using Microsoft.AspNetCore.Mvc;
using MVC_CabServices.Models;
using Newtonsoft.Json;
using NuGet.Common;
using NuGet.Protocol.Plugins;
using System.Net.Http.Json;

namespace MVC_CabServices.Controllers
{
    public class UserController : Controller
    {
        public readonly ILogger<UserController> _logger;
        private  CrudStatus token;
      
        public readonly HttpClient client;
        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7234/api/UserDetails/");
        }
        public ActionResult Index()
        {
            IEnumerable<TbUserClass> users = null!;
            var responseTask = client.GetAsync("GetUserDetails");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readJob = result.Content.ReadFromJsonAsync<IList<TbUserClass>>();
                readJob.Wait();
                users = readJob.Result!;
            }
            else
            {
                users = Enumerable.Empty<TbUserClass>();
                ModelState.AddModelError(string.Empty, "server error");
            }
            return View(users);
        }

        public ActionResult UserRegister()
        {
            return View();
        }

        [HttpPost]
        [ActionName("UserRegister")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserRegister(UserRegister userRegister)
        {
            try
            {
                UserRegister register = new UserRegister();
                var postJob = client.PostAsJsonAsync<UserRegister>("Register", userRegister);
                postJob.Wait();
                var postResult = postJob.Result;
                
                if (postResult.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "User");
                }
                ModelState.AddModelError(string.Empty, "server Error");
                return View(userRegister);
            }

            catch
            {
                return View();
            }
        }

        public ActionResult UserLogIn()
        {
            return View();
        }
       
        [HttpPost]
        [ActionName("UserLogIn")]
        [ValidateAntiForgeryToken]
        public ActionResult UserLogIn(UserLogin user)
        {
            try
            {
                //HttpClient client = new HttpClient();
               // client.BaseAddress = new Uri(BaseUrl);
                UserLogin log = new UserLogin();
                var postJob = client.PostAsJsonAsync<UserLogin>("Login", user);
                postJob.Wait();
                var postResult = postJob.Result;
                var resultMessage = postResult.Content.ReadAsStringAsync().Result;
                token = JsonConvert.DeserializeObject<CrudStatus>(resultMessage)!;
                if (postResult.IsSuccessStatusCode)
                {
                    if (token.Status == true)
                    {
                        return RedirectToAction("Index", "User");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, token.Message!);
                        return View(user);
                    }
                }
                ModelState.AddModelError(string.Empty, "server Error");
                return View(user);
            }
            catch
            {
                return View();
            }
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ActionName("ForgotPassword")]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPassword forgotpassword)
        {
            try
            {
               
                ForgotPassword log = new ForgotPassword();
                //client.BaseAddress = new Uri(BaseURl + "UserDetails/ForgotPassword");
                //HTTP POST
                var putTask = client.PostAsJsonAsync<ForgotPassword>("ForgotPassword", forgotpassword);
                putTask.Wait();
                var result = putTask.Result;
                //var postResult = postJob.Result;
                var resultMessage = result.Content.ReadAsStringAsync().Result;
                token = JsonConvert.DeserializeObject<CrudStatus>(resultMessage)!;
                if (result.IsSuccessStatusCode)
                {
                    if (token.Status == true)
                    {
                        return RedirectToAction("Index", "User");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, token.Message!);
                        return View(forgotpassword);
                    }
                   
                }
                ModelState.AddModelError(string.Empty, "server Error");
                return View(forgotpassword);
            }
            catch
            {
                return View();
            }
          return View(forgotpassword);
        }
    }
}
