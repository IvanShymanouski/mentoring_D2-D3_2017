using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace Task_2
{
    class Task2
    {
        [ThreadStatic] static double previous = 0.0;


        //не понял магию с ThreadName...
        //в примере из MSDN ThreadName.IsValueCreated - показывает false при первом вхождении в поток..
        //в моих примерах всегда true...
        //хотя вообще IsValueCreated -  true if System.Threading.ThreadLocal`1.Value is initialized on the current thread otherwise false.
        //repeat ? "The FIRST ThreadLocal use" : "-------Not the first ThreadLocal use" - это непонятно что за текст у меня(если честно..), мне так казалось по началу
        static void Main(string[] args)
        {

            MSDNExample();


            //Я не могу проверить AsyncLocal потому, что у меня нет Windows 10 в наличии
            //https://msdn.microsoft.com/en-us/library/dn906268(v=vs.110).aspx
            Task2 task2 = new Task2();

            Console.WriteLine("Main thread id is " + Thread.CurrentThread.ManagedThreadId);

            ThreadPoolCheck(task2);
            ThreadsCheck(task2);

            //Кто-то точно, да, использовал текущий поток (скорее всего)
            task2.TestThreadStaticAndLocal(33);
            task2.TestThreadStaticAndLocal(33);

            Thread.CurrentThread.Join();
        }

        public static void MSDNExample()
        {
            // Thread-Local variable that yields a name for a thread
            ThreadLocal<string> ThreadName = new ThreadLocal<string>(() =>
            {
                return "Thread" + Thread.CurrentThread.ManagedThreadId;
            });

            // Action that prints out ThreadName for the current thread
            Action action = () =>
            {
                // If ThreadName.IsValueCreated is true, it means that we are not the
                // first action to run on this thread.
                bool repeat = ThreadName.IsValueCreated;

                Console.WriteLine("ThreadName = {0} {1}", ThreadName.Value, repeat ? "(repeat)" : "");
            };

            // Launch eight of them.  On 4 cores or less, you should see some repeat ThreadNames
            Parallel.Invoke(action, action, action, action, action, action, action, action);

            // Dispose when you are done
            ThreadName.Dispose();
        }

        static void ThreadPoolCheck(Task2 task2)
        {
            CallContext.LogicalSetData("Name", "Jeffrey");

            // Заставляем поток из пула работать
            //все созданные потоки унаследуют контекст
            //ThreadPool запускает поток и отдаёт оправление
            ThreadPool.QueueUserWorkItem(state => task2.TestThreadStaticAndLocal(0));
            ThreadPool.QueueUserWorkItem(state => task2.TestThreadStaticAndLocal(0));
            ThreadPool.QueueUserWorkItem(state => task2.TestThreadStaticAndLocal(0));
            //TPL ждёт выполнения всех методов, но выполняет их параллельно
            Parallel.Invoke(() => task2.TestThreadStaticAndLocal(0), () => task2.TestThreadStaticAndLocal(0), () => task2.TestThreadStaticAndLocal(0), () => task2.TestThreadStaticAndLocal(0), () => task2.TestThreadStaticAndLocal(0), () => task2.TestThreadStaticAndLocal(0), () => task2.TestThreadStaticAndLocal(0), () => task2.TestThreadStaticAndLocal(0), () => task2.TestThreadStaticAndLocal(0), () => task2.TestThreadStaticAndLocal(0), () => task2.TestThreadStaticAndLocal(0), () => task2.TestThreadStaticAndLocal(0), () => task2.TestThreadStaticAndLocal(0), () => task2.TestThreadStaticAndLocal(0), () => task2.TestThreadStaticAndLocal(0), () => task2.TestThreadStaticAndLocal(0));

            Console.WriteLine("SuppressFlow");
            // Запрещаем копирование контекста исполнения потока метода Main
            ExecutionContext.SuppressFlow();
            Console.WriteLine("-SuppressFlow");

            //контекст не наследуется
            ThreadPool.QueueUserWorkItem(state => task2.TestThreadStaticAndLocal(1));
            ThreadPool.QueueUserWorkItem(state => task2.TestThreadStaticAndLocal(1));
            ThreadPool.QueueUserWorkItem(state => task2.TestThreadStaticAndLocal(1));
            Parallel.Invoke(() => task2.TestThreadStaticAndLocal(1), () => task2.TestThreadStaticAndLocal(1), () => task2.TestThreadStaticAndLocal(1), () => task2.TestThreadStaticAndLocal(1), () => task2.TestThreadStaticAndLocal(1), () => task2.TestThreadStaticAndLocal(1), () => task2.TestThreadStaticAndLocal(1), () => task2.TestThreadStaticAndLocal(1), () => task2.TestThreadStaticAndLocal(1), () => task2.TestThreadStaticAndLocal(1), () => task2.TestThreadStaticAndLocal(1), () => task2.TestThreadStaticAndLocal(1), () => task2.TestThreadStaticAndLocal(1), () => task2.TestThreadStaticAndLocal(1), () => task2.TestThreadStaticAndLocal(1), () => task2.TestThreadStaticAndLocal(1));

            Console.WriteLine("RestoreFlow");
            // Запрещаем копирование контекста исполнения потока метода Main
            ExecutionContext.RestoreFlow();
            Console.WriteLine("-RestoreFlow");

            ThreadPool.QueueUserWorkItem(state => task2.TestThreadStaticAndLocal(2));
            ThreadPool.QueueUserWorkItem(state => task2.TestThreadStaticAndLocal(2));
            ThreadPool.QueueUserWorkItem(state => task2.TestThreadStaticAndLocal(2));
            Parallel.Invoke(() => task2.TestThreadStaticAndLocal(2), () => task2.TestThreadStaticAndLocal(2), () => task2.TestThreadStaticAndLocal(2), () => task2.TestThreadStaticAndLocal(2), () => task2.TestThreadStaticAndLocal(2), () => task2.TestThreadStaticAndLocal(2), () => task2.TestThreadStaticAndLocal(2), () => task2.TestThreadStaticAndLocal(2), () => task2.TestThreadStaticAndLocal(2), () => task2.TestThreadStaticAndLocal(2), () => task2.TestThreadStaticAndLocal(2), () => task2.TestThreadStaticAndLocal(2), () => task2.TestThreadStaticAndLocal(2), () => task2.TestThreadStaticAndLocal(2), () => task2.TestThreadStaticAndLocal(2), () => task2.TestThreadStaticAndLocal(2));
        }

        //Очевидно, что каждый поток создаётся вновь и не может зацепить ThreadStatic предыдущих потоков, в отличии от функций посылаемых threadpool-у
        //Контекст подхватывается при запуске потоков, а не при создании.
        static void ThreadsCheck(Task2 task2)
        {

            List<Thread> beforeSuppressCreatedRunAfter = new List<Thread>();
            List<Thread> afterSuppressCreatedRunAfterResume = new List<Thread>();

            CallContext.LogicalSetData("Name", "Jeffrey");

            for (int threads = 0; threads < 10; threads++)
            {
                beforeSuppressCreatedRunAfter.Add(new Thread(task2.TestThreadStaticAndLocal));
            }

            Console.WriteLine("SuppressFlow");
            // Запрещаем копирование контекста исполнения потока метода Main
            ExecutionContext.SuppressFlow();
            Console.WriteLine("-SuppressFlow");

            beforeSuppressCreatedRunAfter.ForEach(thread => thread.Start(10));

            for (int threads = 0; threads < 10; threads++)
            {
                afterSuppressCreatedRunAfterResume.Add(new Thread(task2.TestThreadStaticAndLocal));
            }

            Console.WriteLine("RestoreFlow");
            // Запрещаем копирование контекста исполнения потока метода Main
            ExecutionContext.RestoreFlow();
            Console.WriteLine("-RestoreFlow");

            afterSuppressCreatedRunAfterResume.ForEach(thread => thread.Start(12));

        }

        static ThreadLocal<string> ThreadName = new ThreadLocal<string>(() =>
        {
            return "Thread" + Thread.CurrentThread.ManagedThreadId;
        });

        private void TestThreadStaticAndLocal(object o)
        {
            Console.WriteLine($"{o} I'm {ThreadName.Value}. CallContext(\"Name\") is {CallContext.LogicalGetData("Name")}");
             
            bool repeat = ThreadName.IsValueCreated;
            if (previous != 666)
            {
                previous = 666;
                Console.WriteLine($"{o} {ThreadName.Value}'s the FIRST run. " + (repeat ? "The FIRST ThreadLocal use" : "-------Not the first ThreadLocal use"));
            }
            else
            {
                Console.WriteLine($"{o} {ThreadName.Value}'s the NOT first run!!!!!! " + (repeat ? "The FIRST ThreadLocal use" : "-------Not the first ThreadLocal use"));
            }
            repeat = ThreadName.IsValueCreated;
            Console.WriteLine($"{(repeat ? " " : "------Not the first ThreadLocal use")}");

            /*for(var i=0; i<int.MaxValue; i++)
            {
                var t = i * 0.1;
            }*/
        }
    }
}
