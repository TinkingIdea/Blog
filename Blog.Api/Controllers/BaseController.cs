using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Blog.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        public IActionResult Success(object data, string msg = "")
        {
            return Result(200, msg, data);
        }

        public IActionResult Error(string msg, object data = null)
        {
            return Result(201, msg, data);
        }

        public IActionResult Result(int code, string msg, object data)
        {
            return new JsonResult(new ReturnJson(data, code, msg));
        }

        public class ReturnJson
        {
            [JsonProperty("data")]
            public object Data { get; set; }
            [JsonProperty("meta")]
            public Meta Meta { get; set; }

            public ReturnJson(object data = null, int code = 200, string msg = "")
            {
                Data = data;
                Meta = new Meta
                {
                    Code = code,
                    Message = msg
                };
            }
        }

        public class Meta
        {
            [JsonProperty("code")]
            public int Code { get; set; }
            [JsonProperty("msg")]
            public string Message { get; set; }
        }
    }
}