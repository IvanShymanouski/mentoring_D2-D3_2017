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

        public FileProcessor(string inDirectory, string outDirectory, ManualResetEvent workStopped)
        {
            _inDirectory = inDirectory;
            _outDirectory = outDirectory;

            CreateIfNotExist(_inDirectory);
            CreateIfNotExist(_outDirectory);

            _workStopped = workStopped;
            _newFileAdded = new AutoResetEvent(false);

            _watcher = new FileSystemWatcher(_inDirectory);
            _watcher.Created += On_Created;
        }

        public Thread GetThread()
        {
            return new Thread(WorkProcedure);
        }

        protected virtual void WorkProcedure()
        {
            do
            {
                if (_workStopped.WaitOne(0)) return;
                foreach (var filePath in Directory.EnumerateFiles(_inDirectory))
                {
                    if (_workStopped.WaitOne(0)) return;
                    if (TryToOpen(filePath, 3))
                    {
                        var fileName = Path.GetFileName(filePath);
                        MoveFile(Path.Combine(_inDirectory, fileName), Path.Combine(_outDirectory, fileName));
                    }
                }
            }
            while (WaitHandle.WaitAny(new WaitHandle[] { _workStopped, _newFileAdded }) != 0);
        }

        protected bool TryToOpen(string filePath, int tryCount)
        {
            for (int i = 0; i < tryCount; i++)
            {
                try
                {
                    var file = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
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

        private void CreateIfNotExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
