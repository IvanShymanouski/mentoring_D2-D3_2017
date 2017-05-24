using Contracts;
using PdfProcessor;
using System.IO;
using System.Messaging;
using System.Text.RegularExpressions;
using System.Threading;

namespace DocAutoMoveService
{
    class ImagesProcessor : FileProcessor
    {
        private PdfHelper _pdfHelper;

        public ImagesProcessor(string inDirectory, ManualResetEvent workStopped, object lockObj)
            :base(inDirectory, workStopped, lockObj)
        {
            _pdfHelper = new PdfHelper();
        }

        protected override void WorkProcedure()
        {
            var wasEndImage = false;

            do
            {
                SetStatus();
                //Thread.Sleep(1000); // for debugging reason
                wasEndImage = false;
                _pdfHelper.CreateNewDocument();
                if (_workStopped.WaitOne(0))  break;
                foreach (var filePath in Directory.EnumerateFiles(_inDirectory))
                {
                    if (_workStopped.WaitOne(0)) break;
                    if (IsImage(filePath) && TryToOpen(filePath, 3))
                    {
                        wasEndImage = wasEndImage | IsEndImage(filePath);
                        _pdfHelper.AddImage(filePath);
                    }
                }
                if (wasEndImage) TrySendDocument(3);
                ResetStatus();
            }
            while (WaitHandle.WaitAny(new WaitHandle[] { _workStopped, _newFileAdded }) != 0);
            if (wasEndImage) TrySendDocument(3);
            ResetStatus();
        }

        protected override void SetStatus()
        {
            if (!Monitor.TryEnter(_lockObj, 5000))
            {
                if (_workStopped.WaitOne(0)) return;
                throw new System.Exception("Unknown exception");
            };
            FileProcessSevice.ProcessStatus = (FileProcessSevice.ProcessStatus == Status.ProcessingFile) ? Status.ProcessingFileAndImage : Status.ProcessingImage;
            Monitor.Exit(_lockObj);
        }

        protected override void ResetStatus()
        {
            if (!Monitor.TryEnter(_lockObj, 5000))
            {
                if (_workStopped.WaitOne(0)) return;
                throw new System.Exception("Unknown exception");
            };
            FileProcessSevice.ProcessStatus = (FileProcessSevice.ProcessStatus == Status.ProcessingImage || FileProcessSevice.ProcessStatus == Status.Waiting) ? Status.Waiting : Status.ProcessingFile;
            Monitor.Exit(_lockObj);
        }

        private bool IsImage(string fileName)
        {
            var pattern = @"[\s\S]*[.](?:png|jpeg|jpg)";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            return regex.IsMatch(fileName);
        }

        private bool IsEndImage(string fileName)
        {
            var pattern = @"[\s\S]*End[.](?:png|jpeg|jpg)";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            return regex.IsMatch(fileName);
        }

        public void TrySendDocument(int tryCount)
        {
            for (int i = 0; i < tryCount; i++)
            {
                try
                {
                    var chunks = _pdfHelper.GetChunks(Settings.ChunkSize);
                    using (var serverQueue = new MessageQueue(Settings.ServerQueueName, QueueAccessMode.Send))
                    {
                        foreach (var chunk in chunks)
                        {
                            var message = new Message(chunk);
                            serverQueue.Send(message);
                        }
                    }
                    return;
                }
                catch (IOException)
                {
                    Thread.Sleep(5000);
                }
            }
        }
    }
}
