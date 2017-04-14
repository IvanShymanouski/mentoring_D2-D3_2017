using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace RSS
{
    public class XMLtoRSSBookTransformer
    {
        public string Transform(string xmlPath, string xsltPath)
        {
            var sb = new StringBuilder();
            var transformer = new XslCompiledTransform();
            transformer.Load(xsltPath, new XsltSettings(false, false), null);

            using (var xmlWriter = XmlWriter.Create(sb, transformer.OutputSettings))
            {
                transformer.Transform(xmlPath, xmlWriter);
            }

            return sb.ToString();
        }
    }
}
