using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace DocAutoMoveService
{
    class ImagesProcessor : FileProcessor
    {
        private PdfHelper _pdfHelper;

        public ImagesProcessor(string inDirectory, string outDirectory, ManualResetEvent workStopped)
            :base(inDirectory, outDirectory, workStopped)
        {
            _pdfHelper = new PdfHelper();
        }

        protected override void WorkProcedure()
        {
            do
            {
                var wasEndImage = false;
                _pdfHelper.CreateNewDocument();
                if (_workStopped.WaitOne(0))
                {
                    TrySaveDocument(3);
                    return;
                }
                foreach (var filePath in Directory.EnumerateFiles(_inDirectory))
                {
                    if (_workStopped.WaitOne(0))
                    {
                        if (wasEndImage) TrySaveDocument(3);
                        return;
                    }
                    if (IsImage(filePath) && TryToOpen(filePath, 3))
                    {
                        wasEndImage = wasEndImage | IsEndImage(filePath);
                        _pdfHelper.AddImage(filePath);
                    }
                }
                if (wasEndImage) TrySaveDocument(3);
            }
            while (WaitHandle.WaitAny(new WaitHandle[] { _workStopped, _newFileAdded }) != 0);
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

        public void TrySaveDocument(int tryCount)
        {
            for (int i = 0; i < tryCount; i++)
            {
                try
                {
                    _pdfHelper.SaveDocument(_outDirectory);
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
