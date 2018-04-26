using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        [HttpPost]
        public ActionResult OnSubmitButtonClicked(int Roomsize, string Windowsize)
        {
            Debug.WriteLine("l");
            var viewModel = new Models.HomeViewModel()
            {
                Id = "test",
                Roomsize = Roomsize,
                Windowsize = ConvertWindowSizeToInt(Windowsize),
                ImageUrl = "https://video.360cities.net/littleplanet-360-imagery/360Level43Lounge-8K-stable-noaudio-1024x512.jpg",
            };
            return View("Index", viewModel);
        }
        private int ConvertWindowSizeToInt(string windowSize)
        {
            switch (windowSize)
            {
                case "big":
                    return 3;
                case "medium":
                    return 2;
                case "small":
                    return 1;
            }
            return 0;
        }
    }
}