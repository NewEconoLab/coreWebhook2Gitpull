using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coreWebhook2Gitpull.Controllers
{
    class ConstHelper
    {
        public string bashCMD { get; }
        public string deployCMD { get; }
        public string deployCMD_CN { get; }
        public string deployCMD_EN { get; }
        public string baseDir { get; }
        private ConstHelper()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection()    //将配置文件的数据加载到内存中
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())   //指定配置文件所在的目录
                .AddJsonFile("settings.json", optional: true, reloadOnChange: false)  //指定加载的配置文件
                .Build();    //编译成对象  
            bashCMD = config["bashCMD"];
            deployCMD = config["deployCMD"];
            deployCMD_CN = config["deployCMD_CN"];
            deployCMD_EN = config["deployCMD_EN"];
            baseDir = config["baseDir"];
        }

        private static ConstHelper instance = new ConstHelper();
        public static ConstHelper getInstance()
        {
            return instance;
        }
    }
}
