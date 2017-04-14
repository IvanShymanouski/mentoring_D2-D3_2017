using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;


namespace RSS
{
    [TestClass]
    public class TransformProgramm
    {
        [TestMethod]
        public void Transform()
        {
            Console.WriteLine(new XMLtoRSSBookTransformer().Transform("Books.xml", "BookToRSS.xslt"));
        }
    }
}