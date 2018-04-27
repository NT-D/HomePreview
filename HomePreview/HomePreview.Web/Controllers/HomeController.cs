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
                ImageUrl = "https://roadtovrlive-5ea0.kxcdn.com/wp-content/uploads/2014/09/Venice.Still001.jpeg",
            };
            ViewBag.BackgroundImageUrl = viewModel.ImageUrl;
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
                ImageUrl = "",
            };
            var response = await RequestRenderAsync(viewModel);
            viewModel.Id = response.id;
            viewModel.ImageUrl = response.imageUrl;

            ViewBag.BackgroundImageUrl = response.imageUrl ?? "https://roadtovrlive-5ea0.kxcdn.com/wp-content/uploads/2014/09/Venice.Still001.jpeg";

            return View("Index", viewModel);
        }
        private static async Task<ResponseModel> RequestRenderAsync(Models.HomeViewModel parameters)
        {
            var renderRequestApiUrl = ConfigurationManager.AppSettings["FunctionsEndPoint"];
            ResponseModel resModel = null;

            // 現在、部屋の広さと窓の大きさしかサーバ側が対応していないので、取り急ぎリクエストのパラメータの項目を減らす
            var requestModel = new RequestModel() { roomsize = parameters.Roomsize, windowsize = parameters.Windowsize };
            var requestObj = JsonConvert.SerializeObject(requestModel);

            using (var client = new HttpClient())
            {
                var content = new StringContent(requestObj, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(renderRequestApiUrl, content);
                var responseJson = await response.Content.ReadAsStringAsync();
                resModel = JsonConvert.DeserializeObject<ResponseModel>(responseJson);
            }
            return resModel;
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
    public class RequestModel
    {
        public int windowsize { get; set; }
        public int roomsize { get; set; }
    }
    public class ResponseModel
    {
        public string id { get; set; }
        public int roomsize { get; set; }
        public int windowsize { get; set; }
        public string imageUrl { get; set; }
    }
}