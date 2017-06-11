using System;
using PostSharp.Aspects;
using NLog;
using NLog.Targets;
using NLog.Config;
using System.IO;

namespace AOPLogger
{
    [Serializable]
    public class LoggerPostSharp : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            AOPNlogLogger.Logger.Trace($"started {args.Method.Name}(Value = {args.Arguments[0]}, Power = {args.Arguments[1]})");
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {

            AOPNlogLogger.Logger.Trace($"Result of {args.Method.Name}(Value = {args.Arguments[0]}, Power = {args.Arguments[1]}) execution is {args.ReturnValue}");
        }
    }
}
