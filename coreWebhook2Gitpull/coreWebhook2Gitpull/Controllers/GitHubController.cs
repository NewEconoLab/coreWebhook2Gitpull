using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
//using Microsoft.AspNetCore.WebHooks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

            //执行shell命令测试
            var output = bashCMD.Bash();
            
            J.Add("bashCMD", bashCMD);
            J.Add("bashCMDout", output);
            string resStr = JsonConvert.SerializeObject(J);
            System.IO.File.AppendAllTextAsync(System.IO.Directory.GetCurrentDirectory() + "/log/" + System.DateTime.Now.ToString ("yyyyMMddHH") + ".log",resStr);

            return Json(new JObject() { {"repoName",repoName },{ "cmdRes", output} });
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