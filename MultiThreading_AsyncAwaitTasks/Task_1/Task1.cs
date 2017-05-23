using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task_1
{
    class Task1
    {
        [ThreadStatic]
        static int sum;
        
        static void Main(string[] args)
        {

            var prog = new Task1();

           // PLINQCheck(prog);
            TaskCeck(prog);

            Thread.CurrentThread.Join();
        }

        static void PLINQCheck(Task1 prog)
        {
            CallContext.LogicalSetData("Name", "Loory");
             
            //Выводимые имена из переменной контектса будут равны Loory
            Enumerable.Range(1, 20).AsParallel().ForAll((number) => prog.customAction(number));

            // Запрещаем копирование контекста исполнения потока метода Main
            ExecutionContext.SuppressFlow();

            //Выводимые имена из переменной контектса будут пустые
            Enumerable.Range(21, 40).AsParallel().ForAll((number) => prog.customAction(number));

            ExecutionContext.RestoreFlow();

            //Выводимые имена из переменной контектса будут равны Loory
            Enumerable.Range(41, 60).AsParallel().ForAll((number) => prog.customAction(number));
        }

        static void TaskCeck(Task1 prog)
        {
            List<Task<int>> tasks = new List<Task<int>>();

            CallContext.LogicalSetData("Name", "Jeffrey");

            //все созданные задания унаследуют контекст
            for (var i = 0; i < 20; i++)
            {
                tasks.Add(prog.Summator(i));
            }
            
            Console.WriteLine("SuppressFlow");
            // Запрещаем копирование контекста исполнения потока метода Main
            ExecutionContext.SuppressFlow();
            Console.WriteLine("-SuppressFlow");

            //наследование контекста не произойдёт
            for (var i = 20; i < 40; i++)
            {
                tasks.Add(prog.Summator(i));
            }

            Console.WriteLine("RestoreFlow");
            // Запрещаем копирование контекста исполнения потока метода Main
            ExecutionContext.RestoreFlow();
            Console.WriteLine("-RestoreFlow");

            //все созданные задания унаследуют контекст
            for (var i = 40; i < 60; i++)
            {
                tasks.Add(prog.Summator(i));
            }
            
            Task.WaitAll(tasks.ToArray());
        }

        ThreadLocal<string> ThreadName = new ThreadLocal<string>(() =>
        {
            return "Thread" + Thread.CurrentThread.ManagedThreadId;
        });

        //Новая задача может выполняться в переиспользованном потоке => ThreadLocal/ThreadStatic - не работают изолированно для задачи
        public Task<int> Summator(int maxValue)
        {
            return Task.Run(() => customAction(maxValue));
        }

        public int customAction(int maxValue)
        {
            Console.WriteLine($"{maxValue} I'm {ThreadName.Value}. CallContext(\"Name\") is {CallContext.LogicalGetData("Name")}");

            bool repeat = ThreadName.IsValueCreated;
            if (sum == 0)
            {
                Console.WriteLine($"{maxValue} {ThreadName.Value}'s the FIRST run. " + (repeat ? "The FIRST ThreadLocal use" : "------Not the first ThreadLocal use"));
            }
            else
            {
                Console.WriteLine($"{maxValue} {ThreadName.Value}'s the NOT first run!!!!! " + (repeat ? "The FIRST ThreadLocal use" : "------Not the first ThreadLocal use"));
            }

            for (var i = 0; i < maxValue; i++)
            {
                sum += maxValue;
            }

            return ++sum;
        }
    }
}