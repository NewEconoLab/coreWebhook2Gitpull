using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.WebHooks;
//using Newtonsoft.Json.Linq;

namespace coreWebhook2Gitpull.Controllers
{
    public class GitHubController : Controller
    {
        public IActionResult Index()
        {
            //执行shell命令测试
            string cmd = "echo hello";
            var output = cmd.Bash();

            return View();
        }
    }
}