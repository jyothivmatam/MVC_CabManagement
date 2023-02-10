using Microsoft.AspNetCore.Mvc;
using MVC_CabServices.Models;
using Newtonsoft.Json;
using NuGet.Common;
using NuGet.Protocol.Plugins;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MVC_CabServices.Controllers
{
    public class UserController : BaseController
    {
        public readonly ILogger<UserController> _logger;
        private  CrudStatus token;
        private new const string Sessionkey = "token";
        public new const string SessionId = "id";


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
            string? token = HttpContext.Session.GetString(Sessionkey);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer",
                parameter: token);
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
                    TempData["success"] = "user registered successfully";
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
                
                UserLogin log = new UserLogin();
                var postJob = client.PostAsJsonAsync<UserLogin>("Login", user);
                postJob.Wait();
                var postResult = postJob.Result;
                var resultMessage = postResult.Content.ReadAsStringAsync().Result;
                token = JsonConvert.DeserializeObject<CrudStatus>(resultMessage)!;
                HttpContext.Session.SetInt32(SessionId, Convert.ToInt32(token.id));
                HttpContext.Session.SetString(Sessionkey, token.Message!);
                HttpContext.Session.SetInt32(SessionAdminOrUser, Convert.ToInt32(token.checkAdminOrUser));
                if (postResult.IsSuccessStatusCode)
                {
                    if (token.Status == true)
                    {
                        if (token.checkAdminOrUser == 2)
                        {
                            TempData["success"] = "Login successfully";
                            return RedirectToAction("Profile", "User");
                        }
                        return RedirectToAction("BookCab", "Booking");
                    }
                    else
                    {
                        TempData["message"] = "Incorrect UserId/Password";
                    }
                }
               
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
                        TempData["success"] = "user changed password successfully";
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

        public ActionResult Profile()
        {
            int? admin = HttpContext.Session.GetInt32(SessionAdminOrUser);
            int? id = HttpContext.Session.GetInt32(SessionId);
            string? token = HttpContext.Session.GetString(Sessionkey);
            TbUserClass users = null!;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer",
                parameter: token);
            var responseTask = client.GetAsync("V5?id=" +id);
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readJob = result.Content.ReadFromJsonAsync<TbUserClass>();
                readJob.Wait();
                users = readJob.Result!;
                if (admin == 2)
                {
                    TempData["message"] = "Admin";
                }
                else
                {
                    TempData["message"] = "Customer";
                }
            }
            
            else
            {
                ModelState.AddModelError(string.Empty, "server error");
            }
            return View(users);
        }
    }
}
