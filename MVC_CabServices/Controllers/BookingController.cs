using Domain;
using Microsoft.AspNetCore.Mvc;
using MVC_CabServices.Models;

namespace MVC_CabServices.Controllers
{
    public class BookingController : Controller
    {
        public readonly HttpClient client;
        private CrudStatus response;

        public BookingController()
        {

            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7234/api/Booking/");
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

        public ActionResult Index1()
        {
            IEnumerable<TbBooking> bookings = null!;
            var responseTask = client.GetAsync("GetBookingDetails");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readJob = result.Content.ReadFromJsonAsync<IList<TbBooking>>();
                readJob.Wait();
                bookings = readJob.Result!;
            }
            else
            {
                bookings = Enumerable.Empty<TbBooking>();
                ModelState.AddModelError(string.Empty, "server error");
            }
            return View(bookings);
        }

        public ActionResult BookCab()
        {
            return View();
        }

        [HttpPost]
        [ActionName("BookCab")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookCab(TbBooking bookingcab)
        {
            try
            {
                TbBooking bookingcabs = new TbBooking();
                var postJob = client.PostAsJsonAsync<TbBooking>("CabBooking", bookingcab);
                postJob.Wait();
                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index2", "Booking");
                }
                ModelState.AddModelError(string.Empty, "server Error");
                return View(bookingcab);
            }

            catch
            {
                return View();
            }
        }

        public ActionResult ConfirmBooking()
        {
            return View();
        }

        [HttpPost]
        [ActionName("ConfirmBooking")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmBooking(TbBooking tbBooking)
        {
            try
            {
                TbBooking bookingcabs = new TbBooking();
                bookingcabs.BookingId = tbBooking.BookingId;
                bookingcabs.Status = tbBooking.Status; 
                var putJob = client.PutAsJsonAsync<TbBooking>("ConfirmBooking", tbBooking);
                putJob.Wait();
                var postResult = putJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index1", "Booking");
                }
                ModelState.AddModelError(string.Empty, "server Error");
                return View(tbBooking);
            }

            catch
            {
                return View();
            }
        }

        public ActionResult Index2()
        {
            IEnumerable<TbBooking> bookings = null!;
            var responseTask = client.GetAsync("BookingPending");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readJob = result.Content.ReadFromJsonAsync<IList<TbBooking>>();
                readJob.Wait();
                bookings = readJob.Result!;
            }
            else
            {
                bookings = Enumerable.Empty<TbBooking>();
                ModelState.AddModelError(string.Empty, "server error");
            }
            return View(bookings);
        }

        public ActionResult RideCompleted()
        {
            return View();
        }


        [HttpPost]
        [ActionName("RideCompleted")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RideCompleted(TbBooking tbBooking)
        {
            try
            {
                TbBooking bookingcabs = new TbBooking();
                bookingcabs.BookingId = tbBooking.BookingId;
                bookingcabs.Status = tbBooking.Status;
                var putJob = client.PutAsJsonAsync<TbBooking>("RideCompleted", tbBooking);
                putJob.Wait();
                var postResult = putJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index1", "Booking");
                }
                ModelState.AddModelError(string.Empty, "server Error");
                return View(tbBooking);
            }

            catch
            {
                return View();
            }
        }
    }
}
