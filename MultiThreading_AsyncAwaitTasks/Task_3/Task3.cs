using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task_3
{
    class Task3
    {
        [ThreadStatic]
        static string site;

        //Это задание и аналогичное ему не понял, сокрее всего...
        //в том числе, не увидел дельты (кроме по памяти из-за потоков)
        //хотелось бы обсудить
        static void Main(string[] args)
        {
            Task3.Executer();

            Thread.CurrentThread.Join();
        }

        public async static void Executer()
        {
            List<string> sites = new List<string>();

            for (var i = 0; i < 10000; i++)
            {
                sites.Add(await Thread1());
            }
        }

        public static async Task<string> Thread1()
        {

            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}'s started");

            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}'s got control");
            using (var client = new HttpClient())
            {
                site = await client.GetStringAsync("https://msdn.microsoft.com/en-us/library/ee792409(v=vs.110).aspx");
            }

            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}'s released control");

            return site;
        }

    }
}
