using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Xml.Schema;

namespace ExchangeFileChecking
{
    [TestClass]
    public class VerifyBookXML
    {
        XmlReaderSettings settings;

        [TestInitialize]
        public void Init()
        {
            settings = new XmlReaderSettings();

            settings.Schemas.Add("http://library.by/catalog", "CheckBookSchema.xsd");
            settings.ValidationEventHandler +=
                delegate (object sender, ValidationEventArgs e)
                {
                    Console.WriteLine("[{0}:{1}] {2}", e.Exception.LineNumber, e.Exception.LinePosition, e.Message);
                };

            settings.ValidationFlags = settings.ValidationFlags | XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationType = ValidationType.Schema;
        }

        [TestMethod]
        public void CheckValidXML()
        {
            XmlReader reader = XmlReader.Create("books.xml", settings);

            while (reader.Read()) ;
        }


        [TestMethod]
        public void CheckInValidXML()
        {
            XmlReader reader = XmlReader.Create("books1.xml", settings);

            while (reader.Read()) ;
        }
    }
}
