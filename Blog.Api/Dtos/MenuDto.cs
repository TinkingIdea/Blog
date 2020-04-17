using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Blog.Api.Dtos
{
    public class MenuDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
        [JsonProperty("level")]
        public string Level { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("order")]
        public string Order { get; set; }
    }
}
