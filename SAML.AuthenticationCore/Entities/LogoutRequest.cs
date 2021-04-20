using SAML.AuthenticationCore.Constants;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace SAML.AuthenticationCore.Entities
{
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [XmlRoot(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class LogoutRequest
    {
        public LogoutRequest()
        {

        }

        public LogoutRequest(string issuer, string nameID, string sessionIndex, string destination, string reason, string version = "2.0")
        {
            Issuer = issuer;
            NameID = new NameID()
            {
                Value = nameID,
                Format = SamlConstants.AssertionNameSpace
            };
            SessionIndex = sessionIndex;
            Destination = destination;
            Reason = reason;
            Version = version;
            this.ID = "_" + System.Guid.NewGuid().ToString();
            this.IssueInstant = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        [XmlElement(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public string Issuer { get; set; }

        [XmlElement(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public NameID NameID { get; set; }

        public string SessionIndex { get; set; }

        [XmlAttributeAttribute()]
        public string Destination { get; set; }

        [XmlAttribute()]
        public string ID { get; set; }

        [XmlAttribute()]
        public string IssueInstant { get; set; }

        [XmlAttribute()]
        public string Reason { get; set; }

        [XmlAttribute()]
        public string Version { get; set; }
    }

    [Serializable()]
    [DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [XmlRoot(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class NameID
    {

        [XmlAttribute()]
        public string Format { get; set; }
        [XmlText()]
        public string Value { get; set; }
    }
}
