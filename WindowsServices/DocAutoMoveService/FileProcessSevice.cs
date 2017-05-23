using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace DocAutoMoveService
{
    public class FileProcessSevice
    {
        private string _inDirectory;
        private string _outDirectory;
        private string _imagesDirectory;

        private List<Thread> _workingThreads;
        private List<FileSystemWatcher> _watchers;
        private ManualResetEvent _workStopped;

        public FileProcessSevice(string inDirectory, string outDirectory)
        {
            _inDirectory = inDirectory;
            _outDirectory = outDirectory;
            _imagesDirectory = Path.Combine(_inDirectory, "images");

            _workStopped = new ManualResetEvent(false);

            _workingThreads = new List<Thread>();
            _watchers = new List<FileSystemWatcher>();

            var FP = new FileProcessor(_inDirectory, _outDirectory, _workStopped);
            _workingThreads.Add(FP.GetThread());
            _watchers.Add(FP._watcher);

            var IP = new ImagesProcessor(_imagesDirectory, _outDirectory, _workStopped);
            _workingThreads.Add(IP.GetThread());
            _watchers.Add(IP._watcher);
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

            _workingThreads.ForEach(thr => thr.Join());
        }
        
    }
}
