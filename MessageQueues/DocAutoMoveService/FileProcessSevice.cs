using Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Messaging;
using System.Threading;

namespace DocAutoMoveService
{
    public class FileProcessSevice
    {
        public static Status ProcessStatus;

        private string _inDirectory;
        private string _outDirectory;
        private string _imagesDirectory;

        private List<Thread> _workingThreads;
        private List<FileSystemWatcher> _watchers;
        private ManualResetEvent _workStopped;

        private int _timeout;

        public FileProcessSevice(string inDirectory, string outDirectory)
        {
            var lockObj = new object();
            _inDirectory = inDirectory;
            _outDirectory = outDirectory;
            _imagesDirectory = Path.Combine(_inDirectory, "images");

            CheckMessageQueue(Settings.ServerQueueName);
            CheckMessageQueue(Settings.MonitorQueueName);
            CheckMessageQueue(Settings.ClientQueueName);

            _timeout = Settings.DefaultTimeOut;
            ProcessStatus = Status.Waiting;

            _workStopped = new ManualResetEvent(false);

            _workingThreads = new List<Thread>();
            _watchers = new List<FileSystemWatcher>();

            var FP = new FileProcessor(_inDirectory, _outDirectory, _workStopped, lockObj);
            _workingThreads.Add(FP.GetThread());
            _watchers.Add(FP._watcher);

            var IP = new ImagesProcessor(_imagesDirectory, _workStopped, lockObj);
            _workingThreads.Add(IP.GetThread());
            _watchers.Add(IP._watcher);

            _workingThreads.Add(new Thread(SendSettings));
            _workingThreads.Add(new Thread(SettingsMonitoring));
        }

        public void Start()
        {
            _workingThreads.ForEach(thr => thr.Start());
            _watchers.ForEach(w => w.EnableRaisingEvents = true);
        }

        public void Stop()
        {
            _watchers.ForEach(w => w.EnableRaisingEvents = false);
            _workStopped.Set();

            _workingThreads.ForEach(thr =>
            {
                //вот это странновато, но иногда ThreadPool выдаёт поток из тех, что я создал вручную...
                if (thr.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
                    thr.Join();
            });
        }

        private void CheckMessageQueue(string name)
        {
            if (!MessageQueue.Exists(name))
            {
                MessageQueue.Create(name);
            }
        }

        public void SendSettings()
        {
            while (!_workStopped.WaitOne(_timeout))
            {
                var settings = new ProcessStatus
                {
                    Status = ProcessStatus,
                    Timeout = _timeout
                };

                using (var serverQueue = new MessageQueue(Settings.MonitorQueueName))
                {
                    var message = new Message(settings);
                    serverQueue.Send(message);
                }
            }
        }

        public void SettingsMonitoring()
        {
            using (var clientQueue = new MessageQueue(Settings.ClientQueueName))
            {
                clientQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(int) });

                while (!_workStopped.WaitOne(_timeout))
                {
                    var asyncReceive = clientQueue.BeginPeek();

                    if (WaitHandle.WaitAny(new WaitHandle[] { _workStopped, asyncReceive.AsyncWaitHandle }) == 0)
                    {
                        break;
                    }

                    var message = clientQueue.EndPeek(asyncReceive);
                    clientQueue.Receive();
                    _timeout = (int)message.Body;
                }
            }
        }

    }
}
