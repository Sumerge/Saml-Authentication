using SAML.AuthenticationCore.Constants;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;


namespace SAML.AuthenticationCore.Utilities
{
    /// <summary>
    /// This class contains methods that creates and validates signatures on XmlDocuments.
    /// </summary>
    public class XmlSignatureUtils
    {

        public static bool CheckSignature(XmlDocument doc)
        {
            CheckDocument(doc);
            SignedXml signedXml = RetrieveSignature(doc);

            if (signedXml.SignatureMethod.Contains("rsa-sha256"))
            {
                // SHA256 keys must be obtained from message manually
                List<X509Certificate2> trustedCertificates = GetCertificates(doc);
                foreach (X509Certificate2 cert in trustedCertificates)
                {
                    if (signedXml.CheckSignature(cert.PublicKey.Key))
                    {
                        return true;
                    }
                }
                return false;
            }

            return signedXml.CheckSignature();
        }

        public static bool CheckSignature(XmlDocument doc, AsymmetricAlgorithm alg)
        {
            CheckDocument(doc);
            SignedXml signedXml = RetrieveSignature(doc);

            return signedXml.CheckSignature(alg);
        }

        public static void SignDocument(XmlDocument doc, string id, X509Certificate2 cert)
        {
            SignedXml signedXml = new SignedXml(doc);
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            signedXml.SigningKey = cert.PrivateKey;

            // Retrieve the value of the "ID" attribute on the root assertion element.
            Reference reference = new Reference("#" + id);

            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigExcC14NTransform());

            signedXml.AddReference(reference);

            // Include the public key of the certificate in the assertion.
            signedXml.KeyInfo = new KeyInfo();
            signedXml.KeyInfo.AddClause(new KeyInfoX509Data(cert, X509IncludeOption.WholeChain));

            signedXml.ComputeSignature();

            // Append the computed signature. The signature must be placed as the sibling of the Issuer element.
            if (doc.DocumentElement != null)
            {
                XmlNodeList nodes = doc.DocumentElement.GetElementsByTagName("Issuer", SamlConstants.AssertionNameSpace);

                // doc.DocumentElement.InsertAfter(doc.ImportNode(signedXml.GetXml(), true), nodes[0]);
                XmlNode parentNode = nodes[0].ParentNode;
                if (parentNode != null)
                {
                    parentNode.InsertAfter(doc.ImportNode(signedXml.GetXml(), true), nodes[0]);
                }
            }
        }

        #region Private methods

        /// <summary>
        /// Do checks on the document given. Every public method accepting a XmlDocument instance as parameter should
        /// call this method before continuing.
        /// </summary>
        /// <param name="doc">The doc.</param>
        private static void CheckDocument(XmlDocument doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException("doc");
            }

            if (!doc.PreserveWhitespace)
            {
                throw new InvalidOperationException("The XmlDocument must have its \"PreserveWhitespace\" property set to true when a signed document is loaded.");
            }
        }


        private static List<X509Certificate2> GetCertificates(XmlDocument doc)
        {
            List<X509Certificate2> certificates = new List<X509Certificate2>();
            XmlNodeList nodeList = doc.GetElementsByTagName("ds:X509Certificate");
            if (nodeList.Count == 0)
            {
                nodeList = doc.GetElementsByTagName("X509Certificate");
            }

            foreach (XmlNode xn in nodeList)
            {
                try
                {
                    X509Certificate2 xc = new X509Certificate2(Convert.FromBase64String(xn.InnerText));
                    certificates.Add(xc);
                }
                catch
                {
                    // Swallow the certificate parse error
                }
            }

            return certificates;
        }


        private static SignedXml RetrieveSignature(XmlDocument doc)
        {
            return RetrieveSignature(doc.DocumentElement);
        }
        private static SignedXml RetrieveSignature(XmlElement el)
        {
            if (el.OwnerDocument.DocumentElement == null)
            {
                XmlDocument doc = new XmlDocument() { PreserveWhitespace = true };
                doc.LoadXml(el.OuterXml);
                el = doc.DocumentElement;
            }

            SignedXml signedXml = new SignedXmlWithIdResolvement(el);
            XmlNodeList nodeList = el.GetElementsByTagName(SamlConstants.Signature, SamlConstants.XMLDsig);
            if (nodeList.Count == 0)
            {
                throw new InvalidOperationException("Document does not contain a signature to verify.");
            }

            signedXml.LoadXml((XmlElement)nodeList[0]);

            // To support SHA256 for XML signatures, an additional algorithm must be enabled.
            // This is not supported in .Net versions older than 4.0. In older versions,
            // an exception will be raised if an SHA256 signature method is attempted to be used.
            if (signedXml.SignatureMethod.Contains("rsa-sha256"))
            {
                var addAlgorithmMethod = typeof(CryptoConfig).GetMethod("AddAlgorithm", BindingFlags.Public | BindingFlags.Static);
                if (addAlgorithmMethod == null)
                {
                    throw new InvalidOperationException("This version of .Net does not support CryptoConfig.AddAlgorithm. Enabling sha256 not psosible.");
                }

                addAlgorithmMethod.Invoke(null, new object[] { typeof(RSAPKCS1SHA256SignatureDescription), new[] { signedXml.SignatureMethod } });
            }

            // verify that the inlined signature has a valid reference uri
            VerifyReferenceUri(signedXml, el.GetAttribute("ID"));

            return signedXml;
        }

        private static void VerifyReferenceUri(SignedXml signedXml, string id)
        {
            if (id == null)
            {
                throw new InvalidOperationException("Cannot match null id");
            }

            if (signedXml.SignedInfo.References.Count <= 0)
            {
                throw new InvalidOperationException("No references in Signature element");
            }

            Reference reference = (Reference)signedXml.SignedInfo.References[0];
            string uri = reference.Uri;

            // empty uri is okay - indicates that everything is signed
            if (!string.IsNullOrEmpty(uri))
            {
                if (!uri.StartsWith("#"))
                {
                    throw new InvalidOperationException("Signature reference URI is not a document fragment reference. Uri = '" + uri + "'");
                }

                if (uri.Length < 2 || !id.Equals(uri.Substring(1)))
                {
                    throw new InvalidOperationException("Rererence URI = '" + uri.Substring(1) + "' does not match expected id = '" + id + "'");
                }
            }
        }

        #endregion

        /// <summary>
        /// Used to validate SHA256 signatures
        /// </summary>
        public class RSAPKCS1SHA256SignatureDescription : SignatureDescription
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RSAPKCS1SHA256SignatureDescription"/> class.
            /// </summary>
            public RSAPKCS1SHA256SignatureDescription()
            {
                KeyAlgorithm = "System.Security.Cryptography.RSACryptoServiceProvider";
                DigestAlgorithm = "System.Security.Cryptography.SHA256Managed";
                FormatterAlgorithm = "System.Security.Cryptography.RSAPKCS1SignatureFormatter";
                DeformatterAlgorithm = "System.Security.Cryptography.RSAPKCS1SignatureDeformatter";
            }

        }

        /// <summary>
        /// Signed XML with Id Resolvement class.
        /// </summary>
        public class SignedXmlWithIdResolvement : SignedXml
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SignedXmlWithIdResolvement"/> class.
            /// </summary>
            /// <param name="document">The document.</param>
            public SignedXmlWithIdResolvement(XmlDocument document) : base(document) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="SignedXmlWithIdResolvement"/> class from the specified <see cref="T:System.Xml.XmlElement"/> object.
            /// </summary>
            /// <param name="elem">The <see cref="T:System.Xml.XmlElement"/> object to use to initialize the new instance of <see cref="T:System.Security.Cryptography.Xml.SignedXml"/>.</param>
            /// <exception cref="T:System.ArgumentNullException">
            /// The <paramref name="elem"/> parameter is null.
            /// </exception>
            public SignedXmlWithIdResolvement(XmlElement elem) : base(elem) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="SignedXmlWithIdResolvement"/> class.
            /// </summary>
            public SignedXmlWithIdResolvement() { }

            /// <summary>
            /// Returns the <see cref="T:System.Xml.XmlElement"/> object with the specified ID from the specified <see cref="T:System.Xml.XmlDocument"/> object.
            /// </summary>
            /// <param name="document">The <see cref="T:System.Xml.XmlDocument"/> object to retrieve the <see cref="T:System.Xml.XmlElement"/> object from.</param>
            /// <param name="idValue">The ID of the <see cref="T:System.Xml.XmlElement"/> object to retrieve from the <see cref="T:System.Xml.XmlDocument"/> object.</param>
            /// <returns>The <see cref="T:System.Xml.XmlElement"/> object with the specified ID from the specified <see cref="T:System.Xml.XmlDocument"/> object, or null if it could not be found.</returns>
            public override XmlElement GetIdElement(XmlDocument document, string idValue)
            {
                XmlElement elem = base.GetIdElement(document, idValue);
                if (elem == null)
                {
                    XmlNodeList nl = document.GetElementsByTagName("*");
                    var enumerator = nl.GetEnumerator();
                    while (enumerator != null && enumerator.MoveNext())
                    {
                        XmlNode node = (XmlNode)enumerator.Current;
                        if (node == null || node.Attributes == null)
                        {
                            continue;
                        }

                        var nodeEnum = node.Attributes.GetEnumerator();
                        while (nodeEnum != null && nodeEnum.MoveNext())
                        {
                            XmlAttribute attr = (XmlAttribute)nodeEnum.Current;
                            if (attr != null && (attr.LocalName.ToLower() == "id" && attr.Value == idValue && node is XmlElement))
                            {
                                return (XmlElement)node;
                            }
                        }
                    }
                }

                return elem;
            }
        }
    }
}
