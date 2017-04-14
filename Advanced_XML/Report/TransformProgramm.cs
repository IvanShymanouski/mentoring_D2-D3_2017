using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;


namespace Report
{
    [TestClass]
    public class TransformProgramm
    {
        [TestMethod]
        public void Transform()
        {
            new XMLtoHTMLBookTransformer().Transform("Books.xml", "BookToHTML.xslt", "books.html");
        }
    }
}