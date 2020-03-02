using coreWebhook2Gitpull.lib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
//using Microsoft.AspNetCore.WebHooks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace coreWebhook2Gitpull.Controllers
{
    [Route("api/[controller]")]
    public class GitHubController : Controller
    {
        private GitHubService service = GitHubService.getService();

        [HttpGet]
        public JsonResult Get(){
            JObject J = new JObject();
            //执行shell命令测试
            string bashCMD = "hostname";
            var output = bashCMD.Bash();
            //var output2 = "git pull origin".Bash();

            //JObject Json = new JObject();
            J.Add("bashCMD", bashCMD);
            J.Add("bashCMDout", output);
            //J.Add("put1", output2);
            return Json(J);
        }

        [HttpPost]
        public JsonResult Post([FromBody] JObject J)
        {
            Console.WriteLine("req:"+J.ToString());
            string repoName = J["repository"]["name"].ToString();
            
            try
            {
                var outStr = service.processBash(repoName);
                return Json(toRes(repoName, outStr));
            } catch (Exception ex)
            {
                log(ex);
                return Json(toErrRes(repoName, ex));
            }
            
        }

        private void log(Exception ex)
        {
            System.Text.StringBuilder msg = new System.Text.StringBuilder();
            msg.Append("*************************************** \n");
            msg.AppendFormat(" 异常发生时间： {0} \n", DateTime.Now);
            msg.AppendFormat(" 异常类型： {0} \n", ex.HResult);
            msg.AppendFormat(" 导致当前异常的 Exception 实例： {0} \n", ex.InnerException);
            msg.AppendFormat(" 导致异常的应用程序或对象的名称： {0} \n", ex.Source);
            msg.AppendFormat(" 引发异常的方法： {0} \n", ex.TargetSite);
            msg.AppendFormat(" 异常堆栈信息： {0} \n", ex.StackTrace);
            msg.AppendFormat(" 异常消息： {0} \n", ex.Message);

            string path = Directory.GetCurrentDirectory() + "/log/";
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
            System.IO.File.AppendAllTextAsync(path + DateTime.Now.ToString("yyyyMMddHH") + ".log", msg.ToString());
        }

        private JObject toRes(string repoName, string outStr)
        {
            return new JObject() { { "repoName", repoName }, { "cmdRes", outStr }, { "error", "" } };
        }
        private JObject toErrRes(string repoName, Exception ex)
        {
            return new JObject() { { "repoName", repoName }, { "cmdRes", "" }, { "error", ex.Message } };
        }
        

    }

}