using System.Collections.Generic;

namespace PdfProcessor
{
    public class Chunk
    {
        public int Position { get; set; }

        public int Size { get; set; }

        public List<byte> Buffer { get; set; }

        public int BufferSize { get; set; }
    }
}
