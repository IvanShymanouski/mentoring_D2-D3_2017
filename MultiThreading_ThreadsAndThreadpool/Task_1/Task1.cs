using System;
using System.Collections.Generic;
using System.Threading;

namespace Task_1
{
    class Task1
    {
        static void Main(string[] args)
        {
            List<Thread> threads = new List<Thread>();

            //100 000 (threads) - 278   + 16 (Mb)
            //200 000 (threads) - 278*2 + 16 (Mb)
            //300 000 (threads) - 278*3 + 16 (Mb)
            //Время создания - около секунды
            for (var i=0; i<300000; i++)
            {
                threads.Add(new Thread(funcForThread));
            }

            threads.ForEach(thread => thread.Start());
            
            Thread.CurrentThread.Join();
        }

        public static void funcForThread()
        {
            for (Int64 i = 0; i < Int64.MaxValue; i++)
            {
                Console.WriteLine(i);
            }
        }
    }
}