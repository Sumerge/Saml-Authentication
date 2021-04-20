using SAML.AuthenticationCore.Utilities;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace SAML.AuthenticationCore.ProtocolBinder
{
    public class PostLogoutResponseParser
    {
        public string ResponseBase64 { get; private set; }
        private X509Certificate2 PublicCertificate;
        public XmlDocument XMLResponse { get; private set; }

        public PostLogoutResponseParser(string Response, X509Certificate2 cert)
        {
            this.ResponseBase64 = Response;
            this.PublicCertificate = cert;
            this.XMLResponse = XMLUtilites.LoadXMLFromBase64(Response, Encoding.UTF8);

        }
        public bool ValidateSignature()
        {
            if (XMLResponse != null && PublicCertificate != null)
            {
                return XmlSignatureUtils.CheckSignature(XMLResponse, PublicCertificate.PublicKey.Key);
            }
            return false;
        }
    }
}
