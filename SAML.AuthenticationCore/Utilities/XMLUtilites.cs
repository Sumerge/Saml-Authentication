using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SAML.AuthenticationCore.Utilities
{
    public static class XMLUtilites
    {
        public static XmlDocument LoadXMLFromBase64(string base64String, Encoding encoding)
        {
            byte[] data = Convert.FromBase64String(base64String);
            string xmlString = encoding.GetString(data);
            XmlDocument XmlDoc = new XmlDocument
            {
                PreserveWhitespace = true,
                XmlResolver = null
            };
            XmlDoc.LoadXml(xmlString);
            return XmlDoc;
        }

        public static T ParseXMLtoObject<T>(XmlDocument XmlDoc) where T : class
        {
            T result;
            using (MemoryStream stm = new MemoryStream())
            {
                using (StreamWriter stw = new StreamWriter(stm))
                {
                    stw.Write(XmlDoc.OuterXml);
                    stw.Flush();

                    stm.Position = 0;
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    result = serializer.Deserialize(stm) as T;
                    
                }

            }
            return result;
        }

        public static XmlDocument SerialzetoXML<T>(T Request, XmlSerializerNamespaces XmlNamespaces)
        {
            XmlDocument document;
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, Request, XmlNamespaces);
                stream.Flush();
                document = new XmlDocument();
                stream.Seek(0, SeekOrigin.Begin);
                document.Load(stream);
                stream.Close();
            }

            return document;
        }


    }
}
