using System;
using System.Threading;
using System.Threading.Tasks;

namespace Task_7
{
    class Task7
    {
        //не увидел разницы между .NET 4.0 и .NET 4.5.2 в логике ловли ошибок
        static void Main(string[] args)
        {
            TaskExec();
            //не ловится
            threadPoolExec();
            //не ловится
            threadExec();

            Thread.CurrentThread.Join();
        }

        public static void TaskExec()
        {
            var t = Task.Run(() => { Thread1("Task"); });
            //для .NET 4.0
            //var t = Task.Factory.StartNew(() => { Thread1("Task"); });

            try
            {
                t.Wait();
            }
            catch (AggregateException)
            {
                Console.WriteLine("Got it, Task");
            }
        }

        public static void threadPoolExec()
        {
            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(Thread1), "Thread pool");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Got it, threadpool");
            }
        }

        public static void threadExec()
        {
            try
            {
                new Thread(Thread1).Start("Thread");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Got it, thread");
            }
        }

        static void Thread1(object o)
        {
            Console.WriteLine(o);
            throw new ArgumentException();
        }
    }
}
