using NLog;
using NLog.Config;
using NLog.Targets;
using System.IO;

namespace AOPLogger
{
    class AOPNlogLogger
    {
        public static Logger Logger;

        static AOPNlogLogger()
        {
            var logConfig = new LoggingConfiguration();
            var target = new FileTarget()
            {
                FileName = Path.Combine(@"d:\work\mentoring_D1-D2_2017\AOP\CodeRewriting\AOPLog.txt"),
                Layout = "${date} - '${message}' ${onexception:inner=${exception:format=toString}}"
            };

            logConfig.AddTarget("file", target);
            logConfig.AddRuleForAllLevels(target);

            LogManager.Configuration = logConfig;

            Logger = LogManager.GetLogger("LogInterceptor");
        }
    }
}
