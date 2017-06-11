using Castle.DynamicProxy;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AOPLogger
{
    public class LogInterceptor : IInterceptor
    {

       private static Logger _logger;

        static LogInterceptor()
        {
            var logConfig = new LoggingConfiguration();
            var target = new FileTarget()
            {
                FileName = Path.Combine(@"d:\work\mentoring_D1-D2_2017\AOP\DinamycProxy\AOPLog.txt"),
                Layout = "${date} - '${message}' ${onexception:inner=${exception:format=toString}}"
            };

            logConfig.AddTarget("file", target);
            logConfig.AddRuleForAllLevels(target);

            LogManager.Configuration = logConfig;

            _logger = LogManager.GetLogger("LogInterceptor");
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();

            _logger.Trace($"{invocation.Method.Name}(Value = {invocation.Arguments[0]}, Power = {invocation.Arguments[1]}) = {invocation.ReturnValue}");
        }
    }
}
