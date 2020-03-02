using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coreWebhook2Gitpull.lib
{
    public static class ShellExtension
    {
        public static string formatBashCmd(this string shell, string repositoryName)
        {
            return string.Format(shell, repositoryName);
        }
    }
}
