using Domain;
using Microsoft.AspNetCore.Mvc;
using MVC_CabServices.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MVC_CabServices.Controllers
{
    public class TripController : BaseController
    {
        public readonly HttpClient client;
        private CrudStatus response;
        //private new const string Sessionkey = "token";

        public TripController()
        {

            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7234/api/Trip/");
        }

        public ActionResult Index()
        {
            IEnumerable<TbTripDetail> trips = null!;
            var responseTask = client.GetAsync("GetTripDetails");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readJob = result.Content.ReadFromJsonAsync<IList<TbTripDetail>>();
                readJob.Wait();
                trips = readJob.Result!;
            }
            else
            {
                trips = Enumerable.Empty<TbTripDetail>();
                ModelState.AddModelError(string.Empty, "server error");
            }
            return View(trips);
        }

        public ActionResult AddTrip()
        {
            return View();
        }

        [HttpPost]
        [ActionName("AddTrip")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTrip(TbTripDetail trip)
        {
            try
            {
                TbTripDetail trips = new TbTripDetail();
                var postJob = client.PostAsJsonAsync<TbTripDetail>("AddTripDetails", trip);
                postJob.Wait();
                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    TempData["success"] = "New Trip Added Successfully";
                    return RedirectToAction("Index", "Trip");
                }
                ModelState.AddModelError(string.Empty, "server Error");
                return View(trip);
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
        public ActionResult Edit(int id, TbTripDetail trip)
        {
            try
            {
                trip.TripDetailId = id;
                var putTask = client.PutAsJsonAsync<TbTripDetail>("UpdateTripDetails", trip);
                putTask.Wait();
                var result = putTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["success"] = "Trip Updated Successfully";
                    return RedirectToAction("Index");
                }
                return View(trip);
            }
            catch (Exception)
            {
                return View("Error");

            }
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CabController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, TbTripDetail trip)
        {
            try
            {
                trip.TripDetailId=id;
                var postJob = client.DeleteAsync("DeleteTrip?id=" + id);
                postJob.Wait();
                var postResult = postJob.Result;
                var resultMessage = postResult.Content.ReadAsStringAsync().Result;
                response = JsonConvert.DeserializeObject<CrudStatus>(resultMessage)!;
                if (postResult.IsSuccessStatusCode)
                {
                    if (response.Status == true)
                    {
                        ModelState.AddModelError(string.Empty, response.Message!);
                        TempData["success"] = "trip Removed Successfully";
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
    }
}
