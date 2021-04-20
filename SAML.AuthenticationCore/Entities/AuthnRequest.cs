using System;
using System.Xml.Serialization;

namespace SAML.AuthenticationCore.Entities
{
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [XmlRoot(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class AuthnRequest
    {
        public AuthnRequest(string issuer, string assertionConsumerServiceURL, string destination, string protocolBinding, string Version = "2.0")
        {
            Issuer = issuer;
            ProtocolBinding = protocolBinding;
            AssertionConsumerServiceURL = assertionConsumerServiceURL;
            Destination = destination;
            this.ID = "_" + System.Guid.NewGuid().ToString();
            this.IssueInstant = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            this.Version = Version;
        }
        public AuthnRequest()
        {

        }

        [XmlElement(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public string Issuer { get; set; }
        [XmlAttribute()]
        public string ID { get; set; }
        [XmlAttribute()]
        public string Version { get; set; }
        [XmlAttribute()]
        public string IssueInstant { get; set; }
        [XmlAttribute()]
        public string ProtocolBinding { get; set; }
        [XmlAttribute()]
        public string AssertionConsumerServiceURL { get; set; }
        [XmlAttribute()]
        public string Destination { get; set; }
    }
}
