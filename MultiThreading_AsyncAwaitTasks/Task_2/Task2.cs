using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task_2
{
    class Task2
    {
        //Синтетические пример от доброго Rikhter-а
        static void Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            // Передаем операции CancellationToken и число
            ThreadPool.QueueUserWorkItem(o => Count(cts.Token, 1000));
            Console.WriteLine("Press <Enter> to cancel the operation.");
            Console.ReadLine();
            cts.Cancel(); // Если метод Count уже вернул управления,
                          // Cancel не оказывает никакого эффекта
                          // Cancel немедленно возвращает управление, метод продолжает работу...
            Console.ReadLine();
        }

        private static void Count(CancellationToken token, Int32 countTo)
        {
            for (Int32 count = 0; count < countTo; count++)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Count is cancelled");
                    break; // Выход их цикла для остановки операции
                }
                Console.WriteLine(count);
                Thread.Sleep(200); // Для демонстрационных целей просто ждем
            }
            Console.WriteLine("Count is done");
        }
    }
}