using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Task_4
{
    class Task4
    {
        [ThreadStatic]
        static string site;

        //Не точно понял, что надо было сделать, может и сделал что требуется, но, в любом случае, поразвлекался в целом...
        //можно(хотелось бы) обсудить что надо было сделать
        static void Main(string[] args)
        {
            Console.WriteLine($"Main thread is {Thread.CurrentThread.ManagedThreadId}");

            //поскольку метод асинхронный, 
            //то при спотыкании на await происходит освобождение потока и переход к следующей функции
            var t = ExecuterAwaitTask();
            //Интересно было что статус сразу waitingForActivation - создан не явно
            Console.WriteLine($"first async func status - {t.Status}");
            ExecuterTask();
            //Всё ещё watingForActication, хотя уже результаты в массиве есть.. 
            //Видимо при объявлении фунции Task, статусы возвращаются по текущему запущенному Task-у внутри функции
            Console.WriteLine($"first async func status - {t.Status}");
            ExecuterTaskAndAwait();
            //RanToCompletion
            Console.WriteLine($"first async func status - {t.Status}");

            Thread.CurrentThread.Join();
        }


        static List<string> sitesAwaitTasks = new List<string>();
        public async static Task ExecuterAwaitTask()
        {
            for (var i = 0; i < 10; i++)
            {
                //ждём каждый раз и запускаем новое задание
                sitesAwaitTasks.Add(await TaskWithAwait("AT"));
            }
        }


        static List<Task<string>> sitesTasks = new List<Task<string>>();
        public async static void ExecuterTask()
        {
            for (var i = 0; i < 10; i++)
            {
                //Запускаются все задания
                sitesTasks.Add(TaskWithoutAwait());
            }

            //Только сейчас происходит выполнение задания
            sitesTasks.ForEach((x) => x.Wait());
        }

        static List<Task<string>> sitesTaskAndAwait = new List<Task<string>>();
        public async static void ExecuterTaskAndAwait()
        {
            for (var i = 0; i < 10; i++)
            {
                sitesTaskAndAwait.Add(TaskWithAwait("TaW"));
            }

            //В это время выполняются задания
            Thread.Sleep(5000);

            //Выводим результат.. В идеале надо было бы написать сайт, захостить, обращаться туда и смотреть в какой момент произойдёт обращение, 
            //чтобы полностью убедится когда происходит обращение.
            //Создал, проверил:
            //Зупущенное таким образом задание выполняется в момент вызова функции <TaskWithAwait("TaW")>
            //по всей видимости создаётся новый поток, выполняется задача, потом возвращается управление функции, функция завершается...
            //и дальше можно забирать результат:
            //Вызов таким образом очень похож на параллельность, которая позволяет вернуть результат после выполнения функции.
            sitesTaskAndAwait.ForEach((x) => x.Wait());
        }

        public static async Task<string> TaskWithAwait(string caller)
        {
            Console.WriteLine($"{caller} {Thread.CurrentThread.ManagedThreadId}'s started");

            Console.WriteLine($"{caller} {Thread.CurrentThread.ManagedThreadId}'s got control");

            using (var client = new HttpClient())
            {
                site = await client.GetStringAsync("https://msdn.microsoft.com/en-us/library/ee792409(v=vs.110).aspx");
                //Развеел свой миф, что configureAwait(true) продолжает работу в том же потоке... оно восстанавливает контекст.. хотя вот тут только запутался... 
                //хотелось бы поговорить, узнать(понять) что происходит.
                //site = await client.GetStringAsync("http://localhost:12312/").ConfigureAwait(true);
            }

            Console.WriteLine($"{caller} {Thread.CurrentThread.ManagedThreadId}'s released control");

            return site;
        }

        public static Task<string> TaskWithoutAwait()
        {
            Task<string> site;
            Console.WriteLine($"w {Thread.CurrentThread.ManagedThreadId}'s started");

            Console.WriteLine($"w {Thread.CurrentThread.ManagedThreadId}'s got control");

            //Плохая идея так делать, на самом деле...
            /*using (*/var client = new HttpClient();//)
            {
                site = client.GetStringAsync("https://msdn.microsoft.com/en-us/library/ee792409(v=vs.110).aspx");
                //site = client.GetStringAsync("http://localhost:12312/");
            }

            Console.WriteLine($"w {Thread.CurrentThread.ManagedThreadId}'s released control");

            return site;
        }

    }
}
