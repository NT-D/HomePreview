using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
        public async Task<ActionResult> OnSubmitButtonClicked(int Roomsize, string Windowsize)
        {
            Debug.WriteLine("l");
            var viewModel = new Models.HomeViewModel()
            {
                Id = "test",
                Roomsize = Roomsize,
                Windowsize = ConvertWindowSizeToInt(Windowsize),
                ImageUrl = "https://video.360cities.net/littleplanet-360-imagery/360Level43Lounge-8K-stable-noaudio-1024x512.jpg",
            };
            var imageUrl = await RequestRenderAsync();
            viewModel.ImageUrl = imageUrl;
            return View("Index", viewModel);
        }
        private static async Task<string> RequestRenderAsync()
        {
            var renderRequestApiUrl = ConfigurationManager.AppSettings["FunctionsEndPoint"];
            string imageUrl = "";

            using (var client = new HttpClient())
            {
                var json = @"
{
    'roomsize':22,
    'windowsize':11
}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(renderRequestApiUrl, content);
                imageUrl = await response.Content.ReadAsStringAsync();
            }
            return imageUrl;
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