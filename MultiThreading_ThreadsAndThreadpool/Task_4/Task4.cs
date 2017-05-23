using System;
using System.Net;
using System.Threading;

namespace Task_4
{
    class Task4
    {
        [ThreadStatic]
        static string site;

        static void Main(string[] args)
        {  
            AutoResetEvent are = new AutoResetEvent(true);

            for (var i=0; i<100; i++)
            {
                new Thread(Thread1) { Name = i.ToString(), IsBackground = false }.Start(are);
            }
        }

        public static void Thread1(object o)
        {
            AutoResetEvent are = (AutoResetEvent)o;

            Console.WriteLine($"{Thread.CurrentThread.Name}'s started");

            are.WaitOne();

            Console.WriteLine($"{Thread.CurrentThread.Name}'s got control");
            using (var client = new WebClient())
            {
                site = client.DownloadString("https://msdn.microsoft.com/en-us/library/ee792409(v=vs.110).aspx");
            }

            are.Set();
            Console.WriteLine($"{Thread.CurrentThread.Name}'s released control");
        }

    }
}
