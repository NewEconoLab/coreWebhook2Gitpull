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
        public IActionResult Index()
        {
            return View();
        }

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
            string repoName = J["repository"]["name"].ToString();
            string bashCMD = formatBashCmd(repoName);

            try
            {
                //执行shell命令测试
                var output = bashCMD.Bash();

                J.Add("bashCMD", bashCMD);
                J.Add("bashCMDout", output);
                string resStr = JsonConvert.SerializeObject(J);
                return Json(new JObject() { { "repoName", repoName }, { "cmdRes", output }, { "error", "" } });
            } catch (Exception ex)
            {
                log(ex);
                return Json(new JObject() { { "repoName", repoName }, { "cmdRes", "" }, { "error", ex.Message } });
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

        private string formatBashCmd(string repositoryName)
        {
            return string.Format(ConstHelper.getInstance().bashCMD, repositoryName);
        }
        
    }

    class ConstHelper
    {
        public string bashCMD { get; }
        private ConstHelper()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection()    //将配置文件的数据加载到内存中
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())   //指定配置文件所在的目录
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true)  //指定加载的配置文件
                .Build();    //编译成对象  
            bashCMD = config["bashCMD"];
        }

        private static ConstHelper instance = new ConstHelper();
        public static ConstHelper getInstance()
        {
            return instance;
        }
        


    }
}