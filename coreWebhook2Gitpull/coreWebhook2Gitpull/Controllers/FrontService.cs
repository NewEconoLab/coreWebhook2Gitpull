using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;

namespace coreWebhook2Gitpull.Controllers
{
    public class FrontService
    {
        private static FrontService instance = new FrontService();
        public static FrontService getService() { return instance; }
        private string defaultDir = ConstHelper.getInstance().baseDir;


        public JObject getServiceList()
        {
            var dir = new DirectoryInfo(defaultDir);
            var ds = dir.GetDirectories();
            var fs = ds.Select(p => p.Name).ToArray();
            var count = fs.Count();
            var res = new JObject { { "count", count }, { "list", new JArray { fs } } };
            return res;
        }
        public JObject getServicePull(string id)
        {
            var realDir = defaultDir + id;
            if (!Directory.Exists(realDir))
            {
                return new JObject { { "error", "Path not exist, path=" + realDir } };
            }
            var cmdCfg = ConstHelper.getInstance().bashCMD;
            var cmdStr = string.Format(cmdCfg, id);
            var cmdRes = cmdStr.Bash();
            var res = new JObject {
                { "id", id },
                { "cmdStr", cmdStr},
                { "cmdRes", cmdRes}
            };
            return res;
        }


    }
}
