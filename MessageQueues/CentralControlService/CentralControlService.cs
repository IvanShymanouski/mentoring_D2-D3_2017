using Contracts;
using PdfProcessor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Messaging;
using System.Threading;
using System.Xml.Linq;

namespace CentralControlService
{
    class CentralControlService
    {
        private string _processDirectory;
        private string _settingsDirectory;

        private PdfHelper _pdfHelper;
        private FileSystemWatcher _watcher;

        private Thread _processFiles;
        private Thread _settingsMonitoring;
        private ManualResetEvent _stopWaitEvent;
        private int _currentTimeout;

        public CentralControlService(string processDir)
        {
            _processDirectory = processDir;
            _settingsDirectory = Path.GetFullPath(Path.Combine(processDir, "setting"));
            _pdfHelper = new PdfHelper();
            _currentTimeout = Settings.DefaultTimeOut;

            Directory.CreateDirectory(_processDirectory);
            Directory.CreateDirectory(_settingsDirectory);
            CheckTimeout();

            CheckMessageQueue(Settings.ServerQueueName);
            CheckMessageQueue(Settings.MonitorQueueName);
            CheckMessageQueue(Settings.ClientQueueName);

            _watcher = new FileSystemWatcher(_settingsDirectory);
            _watcher.Filter = "*.xml";
            _watcher.Changed += Watcher_Changed;

            _stopWaitEvent = new ManualResetEvent(false);
            _processFiles = new Thread(ProcessFiles);
            _settingsMonitoring = new Thread(SettingsMonitorService);
        }

        public void ProcessFiles()
        {
            var chunks = new List<Chunk>();
            var count = 1;
            using (var serverQueue = new MessageQueue(Settings.ServerQueueName))
            {
                serverQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(Chunk) });
                do
                {
                    var enumerator = serverQueue.GetMessageEnumerator2();

                    while (enumerator.MoveNext())
                    {
                        var body = enumerator.Current.Body;
                        if (body is Chunk)
                        {
                            var chunk = (Chunk)body;
                            chunks.Add(chunk);

                            if (chunk.Position == chunk.Size)
                            {
                                //не записываем, если какой-то chunk потерялся
                                if (chunk.Size == count)
                                {
                                    _pdfHelper.SaveDocument(_processDirectory, chunks);
                                }
                                count = 0;
                                chunks.Clear();
                            }

                            count++;
                        }
                    }

                    for (int i = 1; i < count; i++)
                    {
                        serverQueue.Receive();
                    }

                    Thread.Sleep(_currentTimeout);

                }
                while (!_stopWaitEvent.WaitOne(0));
            }
        }

        public void SettingsMonitorService()
        {
            while (!_stopWaitEvent.WaitOne(0))
            {
                using (var serverQueue = new MessageQueue(Settings.MonitorQueueName))
                {
                    serverQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(ProcessStatus) });
                    var asyncReceive = serverQueue.BeginPeek();

                    if (WaitHandle.WaitAny(new WaitHandle[] { _stopWaitEvent, asyncReceive.AsyncWaitHandle }) == 0)
                    {
                        break;
                    }

                    var message = serverQueue.EndPeek(asyncReceive);
                    serverQueue.Receive();
                    var settings = (ProcessStatus)message.Body;
                    _currentTimeout = settings.Timeout;

                    var settinsPath = Path.Combine(_settingsDirectory, "setting.csv");
                    if (TryToOpen(settinsPath, 3, false))
                    {
                        using (StreamWriter sw = File.AppendText(settinsPath))
                        {
                            var line = string.Format("{0},{1}", settings.Status, settings.Timeout);
                            if (settings.Timeout != _currentTimeout) CheckTimeout();
                            sw.WriteLine(line);
                        }
                    }
                }
            }
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            CheckTimeout();
        }

        private void CheckTimeout()
        {
            var filePath = Path.Combine(_settingsDirectory, "timeout.xml");
            if (TryToOpen(filePath, 3))
            {
                XDocument doc = XDocument.Load(filePath);
                var timeout = int.Parse(doc.Root.Value);
                if (_currentTimeout != timeout)
                {
                    using (var clientQueue = new MessageQueue(Settings.ClientQueueName))
                    {
                        clientQueue.Send(timeout);
                        _currentTimeout = timeout;
                    }
                }
            }
        }

        public void Start()
        {
            _processFiles.Start();
            _settingsMonitoring.Start();
            _watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;

            _stopWaitEvent.Set();

            //вот это странновато, но иногда ThreadPool выдаёт поток из тех, что я создал вручную...
            //**в этом сервисе я такого не встречал, но на всякий случай...
            if (_processFiles.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)         _processFiles.Join();
            if (_settingsMonitoring.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)   _settingsMonitoring.Join();
        }

        private bool TryToOpen(string filePath, int tryCount, bool onRead = true)
        {
            for (int i = 0; i < tryCount; i++)
            {
                try
                {
                    var file = File.Open(filePath, onRead ? FileMode.Open: FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                    file.Close();

                    return true;
                }
                catch (IOException)
                {
                    Thread.Sleep(5000);
                }
            }

            return false;
        }

        private void CheckMessageQueue(string name)
        {
            if (!MessageQueue.Exists(name))
            {
                MessageQueue.Create(name);
            }
        }
    }
}