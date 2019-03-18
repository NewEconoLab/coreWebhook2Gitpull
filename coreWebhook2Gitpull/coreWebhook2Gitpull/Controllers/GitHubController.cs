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
            Console.WriteLine("req:"+J.ToString());
            string repoName = J["repository"]["name"].ToString();
            
            try
            {
                // DapiDoc
                string outStr;
                if(processDapiDocHook(repoName, out outStr))
                {
                    return Json(toRes(repoName, outStr));
                }

                // General
                var resJo = new JObject();
                var bashCMD = formatBashCmd(repoName);
                var output = bashCMD.Bash();
                resJo.Add("bashCMD", bashCMD);
                resJo.Add("bashCMDout", output);
                outStr = resJo.ToString();
                
                return Json(new JObject() { { "repoName", repoName }, { "cmdRes", outStr }, { "error", "" } });
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


        private bool processDapiDocHook(string repoName, out string outStr)
        {
            if (repoName == "DapiDoc")
            {
                var res = new JObject();
                var cmdStr = formatDeployCmd_CN(repoName);
                var outRes = cmdStr.Bash();
                res.Add("cnCmd", cmdStr);
                res.Add("cnRes", outRes);

                cmdStr = formatDeployCmd_EN(repoName);
                outRes = cmdStr.Bash();
                res.Add("enCmd", cmdStr);
                res.Add("enRes", outRes);
                outStr = res.ToString();
                return true;
            }
            outStr = "";
            return false;
        }

        private JObject toRes(string repoName, string outStr)
        {
            return new JObject() { { "repoName", repoName }, { "cmdRes", outStr }, { "error", "" } };
        }
        private string formatBashCmd(string repositoryName)
        {
            return string.Format(ConstHelper.getInstance().bashCMD, repositoryName);
        }
        private string formatDeployCmd(string repositoryName)
        {
            return string.Format(ConstHelper.getInstance().deployCMD, repositoryName);
        }
        private string formatDeployCmd_CN(string repositoryName)
        {
            return string.Format(ConstHelper.getInstance().deployCMD_CN, repositoryName);
        }
        private string formatDeployCmd_EN(string repositoryName)
        {
            return string.Format(ConstHelper.getInstance().deployCMD_EN, repositoryName);
        }

    }

    class ConstHelper
    {
        public string bashCMD { get; }
        public string deployCMD { get; }
        public string deployCMD_CN { get; }
        public string deployCMD_EN { get; }
        private ConstHelper()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection()    //将配置文件的数据加载到内存中
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())   //指定配置文件所在的目录
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true)  //指定加载的配置文件
                .Build();    //编译成对象  
            bashCMD = config["bashCMD"];
            deployCMD = config["deployCMD"];
            deployCMD_CN = config["deployCMD_CN"];
            deployCMD_EN = config["deployCMD_EN"];
        }

        private static ConstHelper instance = new ConstHelper();
        public static ConstHelper getInstance()
        {
            return instance;
        }
        


    }
}