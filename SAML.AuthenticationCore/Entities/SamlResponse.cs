using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace SAML.AuthenticationCore.Entities
{
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [XmlRoot(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class Response
    {

        /// <remarks/>
        [XmlElement(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public Issuer Issuer { get; set; }

        /// 
        [XmlElement(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }

        public ResponseStatus Status { get; set; }


        [XmlElement(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public Assertion Assertion { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        public string Destination { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        public string ID { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        public string InResponseTo { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        public DateTime IssueInstant { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        public decimal Version { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [XmlRoot(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class Issuer
    {

        /// <remarks/>
        [XmlAttribute()]
        public string Format { get; set; }

        /// <remarks/>
        [XmlText()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [XmlRoot(Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class Signature
    {

        public SignatureSignedInfo SignedInfo { get; set; }

        public string SignatureValue { get; set; }

        public SignatureKeyInfo KeyInfo { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureSignedInfo
    {

        /// <remarks/>
        public SignatureSignedInfoCanonicalizationMethod CanonicalizationMethod { get; set; }

        /// <remarks/>
        public SignatureSignedInfoSignatureMethod SignatureMethod { get; set; }

        /// <remarks/>
        public SignatureSignedInfoReference Reference { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureSignedInfoCanonicalizationMethod
    {

        /// <remarks/>
        [XmlAttribute()]
        public string Algorithm { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureSignedInfoSignatureMethod
    {

        /// <remarks/>
        [XmlAttribute()]
        public string Algorithm { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureSignedInfoReference
    {

        /// <remarks/>
        [XmlArrayItem("Transform", IsNullable = false)]
        public SignatureSignedInfoReferenceTransform[] Transforms { get; set; }

        /// <remarks/>
        public SignatureSignedInfoReferenceDigestMethod DigestMethod { get; set; }

        /// <remarks/>
        public string DigestValue { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string URI { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [DesignerCategory("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureSignedInfoReferenceTransform
    {

        /// <remarks/>
        [XmlElementAttribute(Namespace = "http://www.w3.org/2001/10/xml-exc-c14n#")]
        public InclusiveNamespaces InclusiveNamespaces { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string Algorithm { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2001/10/xml-exc-c14n#")]
    [XmlRootAttribute(Namespace = "http://www.w3.org/2001/10/xml-exc-c14n#", IsNullable = false)]
    public partial class InclusiveNamespaces
    {

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string PrefixList { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureSignedInfoReferenceDigestMethod
    {

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string Algorithm { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureKeyInfo
    {

        /// <remarks/>
        public SignatureKeyInfoX509Data X509Data { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureKeyInfoX509Data
    {

        /// <remarks/>
        public string X509Certificate { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    public partial class ResponseStatus
    {

        /// <remarks/>
        public ResponseStatusStatusCode StatusCode { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    public partial class ResponseStatusStatusCode
    {

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [XmlRootAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class Assertion
    {

        /// <remarks/>
        public AssertionIssuer Issuer { get; set; }

        /// <remarks/>
        [XmlElementAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }

        /// <remarks/>
        public AssertionSubject Subject { get; set; }

        /// <remarks/>
        public AssertionConditions Conditions { get; set; }

        /// <remarks/>
        public AssertionAuthnStatement AuthnStatement { get; set; }

        /// <remarks/>
        [XmlArrayItemAttribute("Attribute", IsNullable = false)]
        public AssertionAttribute[] AttributeStatement { get; set; }

        [XmlAttributeAttribute()]
        public string ID { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute()]
        public System.DateTime IssueInstant { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute()]
        public decimal Version { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public partial class AssertionIssuer
    {

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string Format { get; set; }

        /// <remarks/>
        [XmlTextAttribute()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public partial class AssertionSubject
    {

        /// <remarks/>
        public AssertionSubjectNameID NameID { get; set; }

        /// <remarks/>
        public AssertionSubjectSubjectConfirmation SubjectConfirmation { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public partial class AssertionSubjectNameID
    {

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string Format { get; set; }

        /// <remarks/>
        [XmlTextAttribute()]
        public uint Value { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public partial class AssertionSubjectSubjectConfirmation
    {

        /// <remarks/>
        public AssertionSubjectSubjectConfirmationSubjectConfirmationData SubjectConfirmationData { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string Method { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public partial class AssertionSubjectSubjectConfirmationSubjectConfirmationData
    {

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string InResponseTo { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute()]
        public System.DateTime NotOnOrAfter { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string Recipient { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public partial class AssertionConditions
    {

        /// <remarks/>
        public AssertionConditionsAudienceRestriction AudienceRestriction { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute()]
        public System.DateTime NotBefore { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute()]
        public System.DateTime NotOnOrAfter { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public partial class AssertionConditionsAudienceRestriction
    {

        /// <remarks/>
        public string Audience { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public partial class AssertionAuthnStatement
    {

        /// <remarks/>
        public AssertionAuthnStatementAuthnContext AuthnContext { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute()]
        public System.DateTime AuthnInstant { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        public string SessionIndex { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public partial class AssertionAuthnStatementAuthnContext
    {

        /// <remarks/>
        public string AuthnContextClassRef { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public partial class AssertionAttribute
    {

        /// <remarks/>
        public string AttributeValue { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        public string Name { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        public string NameFormat { get; set; }
    }
}
