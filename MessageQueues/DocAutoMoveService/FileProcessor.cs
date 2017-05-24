using Contracts;
using System.IO;
using System.Threading;

namespace DocAutoMoveService
{
    class FileProcessor
    {
        public FileSystemWatcher _watcher { get; private set; }

        protected string _inDirectory;
        protected string _outDirectory;
        protected ManualResetEvent _workStopped;
        protected AutoResetEvent _newFileAdded;
        protected object _lockObj;

        protected FileProcessor(string inDirectory, ManualResetEvent workStopped, object lockObj)
        {
            _lockObj = lockObj;

            _inDirectory = inDirectory;

            Directory.CreateDirectory(_inDirectory);

            _workStopped = workStopped;
            _newFileAdded = new AutoResetEvent(false);

            _watcher = new FileSystemWatcher(_inDirectory);
            _watcher.Created += On_Created;
        }

        public FileProcessor(string inDirectory, string outDirectory, ManualResetEvent workStopped, object lockObj) : 
              this(inDirectory, workStopped, lockObj)
        {
            _outDirectory = outDirectory;
            Directory.CreateDirectory(_outDirectory);
        }

        public Thread GetThread()
        {
            return new Thread(WorkProcedure);
        }

        protected virtual void WorkProcedure()
        {
            do
            {
                SetStatus();
                //Thread.Sleep(1000); // for debugging reason
                if (_workStopped.WaitOne(0)) break;
                foreach (var filePath in Directory.EnumerateFiles(_inDirectory))
                {
                    if (_workStopped.WaitOne(0)) break;
                    if (TryToOpen(filePath, 3))
                    {
                        var fileName = Path.GetFileName(filePath);
                        MoveFile(Path.Combine(_inDirectory, fileName), Path.Combine(_outDirectory, fileName));
                    }
                }
                ResetStatus();
            }
            while (WaitHandle.WaitAny(new WaitHandle[] { _workStopped, _newFileAdded }) != 0);
            ResetStatus();
        }

        protected virtual void SetStatus()
        {
            if (!Monitor.TryEnter(_lockObj, 5000))
            {
                if (_workStopped.WaitOne(0)) return;
                throw new System.Exception("Unknown exception");
            };
            FileProcessSevice.ProcessStatus = (FileProcessSevice.ProcessStatus == Status.ProcessingImage) ? Status.ProcessingFileAndImage :  Status.ProcessingFile;
            Monitor.Exit(_lockObj);
        }

        protected virtual void ResetStatus()
        {
            if (!Monitor.TryEnter(_lockObj, 5000))
            {
                if (_workStopped.WaitOne(0)) return;
                throw new System.Exception("Unknown exception");
            };
            FileProcessSevice.ProcessStatus = (FileProcessSevice.ProcessStatus == Status.ProcessingFile || FileProcessSevice.ProcessStatus == Status.Waiting) ? Status.Waiting : Status.ProcessingImage;
            Monitor.Exit(_lockObj);
        }

        protected bool TryToOpen(string filePath, int tryCount)
        {
            for (int i = 0; i < tryCount; i++)
            {
                try
                {
                    var file = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
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

        private static void MoveFile(string sourceFilePath, string newFilePath)
        {
            if (File.Exists(newFilePath))
            {
                File.Delete(newFilePath);
            }

            File.Move(sourceFilePath, newFilePath);
        }

        private void On_Created(object sender, FileSystemEventArgs e)
        {
            _newFileAdded.Set();
        }
    }
}
