using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Blog.Api.Dtos.User
{
    public class QueryInfoDto
    {
        [JsonProperty("query")]
        public string Query { get; set; }
        [JsonProperty("pagesize")]
        public int PageSize { get; set; }
        [JsonProperty("pagenum")]
        public int PageNum { get; set; }

        public int Skip { get; set; }

        public QueryInfoDto()
        {
            Skip = (PageNum - 1) * PageSize;
        }
    }
}
