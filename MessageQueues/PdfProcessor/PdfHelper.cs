using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PdfProcessor
{
    public class PdfHelper
    {
        private Document _currentDocument;
        private Section _currentSection;

        public List<string> Images { get; private set; }

        public void AddImage(string filePath)
        {
            var img = _currentSection.AddImage(filePath);
            img.RelativeHorizontal = RelativeHorizontal.Page;
            img.RelativeVertical = RelativeVertical.Page;

            img.Top = 0;
            img.Left = 0;

            img.Height = _currentDocument.DefaultPageSetup.PageHeight;
            img.Width = _currentDocument.DefaultPageSetup.PageWidth;

            _currentSection.AddPageBreak();

            Images.Add(filePath);
        }

        public void SaveDocument(string outDirectory, List<Chunk> chunks)
        {
            var filePath = Path.Combine(outDirectory, $"images_{DateTime.Now:MM-dd-yy_H-mm-ss}.pdf");

            using (Stream destination = File.Create(Path.Combine(outDirectory, filePath)))
            {
                foreach (var chunk in chunks)
                {
                    destination.Write(chunk.Buffer.ToArray(), 0, chunk.BufferSize);
                }
            }
        }

        public void CreateNewDocument()
        {
            _currentDocument = new Document();
            _currentSection = _currentDocument.AddSection();
            Images = new List<string>();
        }

        public List<Chunk> GetChunks(int chunkSize)
        {
            var render = new PdfDocumentRenderer();
            render.Document = _currentDocument;
            render.RenderDocument();

            var pageCount = render.PdfDocument.PageCount - 1;
            render.PdfDocument.Pages.RemoveAt(pageCount);
            var buffer = new byte[1024];
            int bytesRead;
            var chunks = new List<Chunk>();

            using (var ms = new MemoryStream())
            {
                render.PdfDocument.Save(ms, false);
                ms.Position = 0;
                var position = 1;
                var size = (int)Math.Ceiling((double)(ms.Length) / chunkSize);

                while ((bytesRead = ms.Read(buffer, 0, buffer.Length)) > 0)
                {
                    var chunk = new Chunk
                    {
                        Position = position,
                        Size = size,
                        Buffer = buffer.ToList(),
                        BufferSize = bytesRead
                    };

                    chunks.Add(chunk);
                    position++;
                }
            }

            return chunks;
        }
    }
}
