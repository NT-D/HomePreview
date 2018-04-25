using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace HomePreviewCommon.Data
{
    public class RenderParam
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public int Roomsize { get; set; }
        public int Windowsize { get; set; }
        public string ImageUrl { get; set; }
    }
}
