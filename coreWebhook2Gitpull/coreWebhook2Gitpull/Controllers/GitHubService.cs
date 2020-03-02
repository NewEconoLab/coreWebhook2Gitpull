using coreWebhook2Gitpull.lib;
using Newtonsoft.Json.Linq;
using System.IO;

namespace coreWebhook2Gitpull.Controllers
{
    public class GitHubService
    {
        private static GitHubService instance = new GitHubService();
        public static GitHubService getService() { return instance; }

        public string processBash(string repoName)
        {
            // DapiDoc
            string outStr;
            if (processDapiDocHook(repoName, out outStr))
            {
                return outStr;
            }
            // MultiBranch
            if (processMultiBranch(repoName, out outStr))
            {
                return outStr;
            }
            // General
            var resJo = new JObject();
            var bashCMD = ConstHelper.getInstance().bashCMD.formatBashCmd(repoName);
            var output = bashCMD.Bash();
            resJo.Add("bashCMD", bashCMD);
            resJo.Add("bashCMDout", output);
            outStr = resJo.ToString();
            return outStr;
        }

        private bool processDapiDocHook(string repoName, out string outStr)
        {
            if (repoName == "DapiDoc")
            {
                var res = new JObject();
                var cmdStr = ConstHelper.getInstance().deployCMD_CN.formatBashCmd(repoName);
                var outRes = cmdStr.Bash();
                res.Add("cnCmd", cmdStr);
                res.Add("cnRes", outRes);

                cmdStr = ConstHelper.getInstance().deployCMD_EN.formatBashCmd(repoName);
                outRes = cmdStr.Bash();
                res.Add("enCmd", cmdStr);
                res.Add("enRes", outRes);
                outStr = res.ToString();
                return true;
            }
            outStr = "";
            return false;
        }

        private bool processMultiBranch(string repoName, out string outStr)
        {
            outStr = "";
            var dir = ConstHelper.getInstance().baseDir;
            if (!Directory.Exists(dir)) return false;

            var flag = false;
            var index = 0;
            var ds = Directory.GetDirectories(dir);
            foreach (var fs in ds)
            {
                var oldfs = fs.ToLower();
                var newfs = repoName.ToLower();
                if (oldfs.StartsWith(newfs))
                {
                    var resJo = new JObject();
                    var bashCMD = ConstHelper.getInstance().bashCMD.formatBashCmd(fs);
                    var output = bashCMD.Bash();
                    resJo["bashCMD" + index] = bashCMD;
                    resJo["bashCMDout" + index] = output;
                    outStr = resJo.ToString();
                    ++index;
                    flag = true;
                }
            }
            return flag;
        }
    }
}
