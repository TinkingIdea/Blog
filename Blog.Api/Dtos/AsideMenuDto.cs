using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Blog.Api.Dtos
{
    public class AsideMenuDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("state")]
        public bool State { get; set; }
        [JsonProperty("children")]
        public IEnumerable<Child> Children { get; set; }
        public class Child
        {
            [JsonProperty("id")]
            public long Id { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("path")]
            public string Path { get; set; }
            [JsonProperty("state")]
            public bool State { get; set; }
        }
    }
}
