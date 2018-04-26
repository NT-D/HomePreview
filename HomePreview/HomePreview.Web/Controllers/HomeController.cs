using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomePreview.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var viewModel = new Models.HomeViewModel()
            {
                Id = "test",
                Roomsize = 11,
                Windowsize = 1,
                ImageUrl = "https://video.360cities.net/littleplanet-360-imagery/360Level43Lounge-8K-stable-noaudio-1024x512.jpg",
            };
            return View(viewModel);
        }
    }
}