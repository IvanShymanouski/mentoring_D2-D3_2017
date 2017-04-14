using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace Report
{
    public class XMLtoHTMLBookTransformer
    {
        public void Transform(string xmlPath, string xsltPath, string htmlPath)
        {
            var sb = new StringBuilder();
            var transformer = new XslCompiledTransform();
            transformer.Load(xsltPath, new XsltSettings(false, true), null);
            transformer.Transform(xmlPath, htmlPath);
        }
    }
}
