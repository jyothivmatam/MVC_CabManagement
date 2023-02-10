using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC_CabServices.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MVC_CabServices.Controllers
{
    public class CabController : BaseController
    {

        public readonly HttpClient client;
        private CrudStatus response;
        private new const string Sessionkey = "token";
        public new const string SessionId = "id";

        public CabController()
        {

            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7234/api/Cab/");
        }

        public ActionResult Index(int id)
        {
            IEnumerable<TbCab> cabs = null!;
            string? token = HttpContext.Session.GetString(Sessionkey);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer",
                parameter: token);
            var responseTask = client.GetAsync("GetCabDetails");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readJob = result.Content.ReadFromJsonAsync<IList<TbCab>>();
                readJob.Wait();
                cabs = readJob.Result!;
            }
            else
            {
                cabs = Enumerable.Empty<TbCab>();
                ModelState.AddModelError(string.Empty, "server error");
            }
            return View(cabs);
        }

        public ActionResult AddCab()
        {
            return View();
        }

        [HttpPost]
        [ActionName("AddCab")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCab(TbCab cab)
        {
            try
            {
                TbCab cabs = new TbCab();
                string? token = HttpContext.Session.GetString(Sessionkey);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer",
                    parameter: token);
                var postJob = client.PostAsJsonAsync<TbCab>("AddCabDetails", cab);
                postJob.Wait();
                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    TempData["success"] = "New Cab Added Successfully";
                    return RedirectToAction("Index", "Cab");
                }
                ModelState.AddModelError(string.Empty, "server Error");
                return View(cab);
            }

            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(int id, TbCab cab)
        {
            try
            {
                cab.Cabid = id;
                string? token = HttpContext.Session.GetString(Sessionkey);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer",
                    parameter: token);
                //cab.RegistrationNun=regNo;
                var putTask = client.PutAsJsonAsync<TbCab>("UpdateCabDetails", cab);
                putTask.Wait();

                var result = putTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["success"] = "Cab Updated Successfully";
                    return RedirectToAction("Index");
                }
                return View(cab);
            }
            catch (Exception)
            {
                return View("Error");

            }
        }
        public ActionResult Delete(int id)
        {
            string? token = HttpContext.Session.GetString(Sessionkey);
            CabDisplay cabs = null!;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer",
                parameter: token);
            var responseTask = client.GetAsync("GetCab?id=" + id);
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readJob = result.Content.ReadFromJsonAsync<CabDisplay>();
                readJob.Wait();
                cabs = readJob.Result!;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "server error");
            }
            return View(cabs);
        }
    

        // POST: CabController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, TbCabDetail cab)
        {
            try
            {
                cab.Cabid = id;
                string? token = HttpContext.Session.GetString(Sessionkey);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer",
                    parameter: token);
                var postJob = client.DeleteAsync("DeleteCab?id=" + id);
                postJob.Wait();
                var postResult = postJob.Result;
                var resultMessage = postResult.Content.ReadAsStringAsync().Result;
                response = JsonConvert.DeserializeObject<CrudStatus>(resultMessage)!;
                if (postResult.IsSuccessStatusCode)
                {
                    if (response.Status == true)
                    {
                        ModelState.AddModelError(string.Empty, response.Message!);
                        TempData["success"] = "Cab Removed Successfully";
                        return RedirectToAction("Index"/*, "Cab"*/);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Message!);
                        return View();
                    }
                }
                ModelState.AddModelError(string.Empty, "server Error");
                return View();
            }
            catch
            {
                return View();
            }
        }

        public ActionResult GetCabById(int id, CabDisplay cab)
        {
            cab.Cabid = id;
            string? token = HttpContext.Session.GetString(Sessionkey);
            CabDisplay cabs = null!;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer",
                parameter: token);
            var responseTask = client.GetAsync("GetCab?id=" + id);
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readJob = result.Content.ReadFromJsonAsync<CabDisplay>();
                readJob.Wait();
                cabs = readJob.Result!;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "server error");
            }
            return View(cabs);
        }
    }
}
