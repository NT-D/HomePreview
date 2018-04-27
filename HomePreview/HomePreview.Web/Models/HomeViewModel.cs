using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomePreview.Web.Models
{
    public class HomeViewModel
    {
        public string Id { get; set; }
        public int Roomsize { get; set; }
        public int Windowsize { get; set; }
        public string ImageUrl { get; set; }
    }
}