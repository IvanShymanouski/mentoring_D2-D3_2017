using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Task_3
{
    public class Task3
    {
        public static void Main()
        {
            CancellationToken globalToken = new CancellationToken();

            new Thread((object o) =>
            {
                var token = (CancellationToken)o;
                var range = Enumerable.Range(1, 100000).ToList();
                foreach (var number in range)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    else
                    {
                        Console.WriteLine(number);
                    }
                }

            }).Start(globalToken);


            Thread.Sleep(2000);

            globalToken.Register((o) => { Console.WriteLine("Work finished"); });

            globalToken.Cancel();

            Thread.CurrentThread.Join();
        }
    }
}