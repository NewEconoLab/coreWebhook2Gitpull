using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace coreWebhook2Gitpull.Controllers
{
    [Route("api/[controller]")]
    public class FrontController : Controller
    {
        private FrontService service = FrontService.getService();

        [HttpGet("service/list")]
        public JObject Get()
        {
            try
            {
                return service.getServiceList();
            }
            catch (Exception ex)
            {
                return toErrRes(ex);
            }
        }
        [HttpPost("service/update")]
        public JObject Post([FromBody] ReqParam param)
        {
            try
            {
                return service.getServicePull(param.id);
            }
            catch (Exception ex)
            {
                return toErrRes(ex);
            }
        }
        public class ReqParam
        {
            public string id { get; set; }
        }
        JObject toErrRes(Exception ex)
        {
            return new JObject {
                {"error", ex.StackTrace }
            };
        }

    }
}
