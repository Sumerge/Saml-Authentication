using SAML.AuthenticationCore.Entities;
using SAML.AuthenticationCore.Utilities;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace SAML.AuthenticationCore.ProtocolBinder
{
    public class PostLoginResponseParser
    {
        public string ResponseBase64 { get; private set; }
        private X509Certificate2 PublicCertificate;
        public XmlDocument XMLResponse { get; private set; }
        public string SessionId { get; private set; }
        public Response SamlResponse { get; private set; }

        public PostLoginResponseParser(string Response, X509Certificate2 cert)
        {
            this.ResponseBase64 = Response;
            this.PublicCertificate = cert;
            this.XMLResponse = XMLUtilites.LoadXMLFromBase64(Response, Encoding.UTF8);
            this.SamlResponse = XMLUtilites.ParseXMLtoObject<Response>(this.XMLResponse);
            this.SessionId = SamlResponse.Assertion.AuthnStatement.SessionIndex;
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
