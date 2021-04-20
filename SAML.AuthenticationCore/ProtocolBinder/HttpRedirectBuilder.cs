using SAML.AuthenticationCore.Constants;
using SAML.AuthenticationCore.Utilities;
using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace SAML.AuthenticationCore.ProtocolBinder
{
    public class HttpRedirectBuilder
    {
        private AsymmetricAlgorithm Signingkey;
        public string RelayState { get; set; }
        public XmlDocument Request { get; set; }
        
        public HttpRedirectBuilder()
        {

        }
        public HttpRedirectBuilder(X509Certificate2 cert, XmlDocument Request, string RelayState = null)
        {
            this.Signingkey = cert.PrivateKey;
            this.Request = Request;
            this.RelayState = RelayState;
        }
        public String GenerateRedirectUrl(string destination)
        {
            return destination + "?" + this.ToQuery();
        }

        private String GenerateSamlRequest(RequestFormat format)
        {
            using (StringWriter sw = new StringWriter())
            {
                XmlWriterSettings xws = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = true
                };
                string xmlString = "";
                using (StringWriter stringWriter = new StringWriter())
                {
                    using (XmlWriter xmlTextWriter = XmlWriter.Create(stringWriter, xws))
                    {
                        Request.WriteTo(xmlTextWriter);
                        xmlTextWriter.Flush();
                        xmlString = stringWriter.GetStringBuilder().ToString();

                        if (format == RequestFormat.Base64)
                        {
                            string result;
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                using (StreamWriter writer = new StreamWriter(new DeflateStream(memoryStream, CompressionMode.Compress, true), new UTF8Encoding(false)))
                                {
                                    writer.Write(xmlString);
                                    writer.Close();
                                }
                                result = Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length, Base64FormattingOptions.None);
                            }
                            return result;
                        }
                        if (format == RequestFormat.Text)
                        {
                            return xmlString;
                        }
                    }
                }
                return null;
            }
        }

        public string ToQuery()
        {
            StringBuilder result = new StringBuilder();
            
            AddMessageParameter(result);
            AddRelayState(result);
            AddSignature(result);

            return result.ToString();
        }

        private static string UpperCaseUrlEncode(string value)
        {
            StringBuilder result = new StringBuilder(value);
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] == '%')
                {
                    result[++i] = char.ToUpper(result[i]);
                    result[++i] = char.ToUpper(result[i]);
                }
            }

            return result.ToString();
        }

        private void AddRelayState(StringBuilder result)
        {
            if (RelayState == null)
            {
                return;
            }

            result.Append(string.Format("&{0}=", SamlConstants.RelayState));

            result.Append(Request != null ? Uri.EscapeDataString(Compression.DeflateEncode(RelayState)) : RelayState);
        }

        private void AddSignature(StringBuilder result)
        {
            if (Signingkey == null)
            {
                return;
            }

            result.Append(string.Format("&{0}=", SamlConstants.SignatureAlg));

            if (Signingkey is RSA)
            {
                result.Append(UpperCaseUrlEncode(Uri.EscapeDataString(SignedXml.XmlDsigRSASHA1Url)));
            }
            else
            {
                result.Append(UpperCaseUrlEncode(Uri.EscapeDataString(SignedXml.XmlDsigDSAUrl)));
            }

            // Calculate the signature of the URL as described in [SAMLBind] section 3.4.4.1.
            byte[] signature = SignData(Encoding.UTF8.GetBytes(result.ToString()));

            result.AppendFormat("&{0}=", SamlConstants.Signature);
            result.Append(Uri.EscapeDataString(Convert.ToBase64String(signature)));
        }

        private byte[] SignData(byte[] data)
        {
            if (Signingkey is RSACryptoServiceProvider)
            {
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)Signingkey;
                return rsa.SignData(data, new SHA1CryptoServiceProvider());
            }
            else
            {
                DSACryptoServiceProvider dsa = (DSACryptoServiceProvider)Signingkey;
                return dsa.SignData(data);
            }
        }
        private void AddMessageParameter(StringBuilder result)
        {
            if (Request == null)
            {
                throw new Exception("Request property MUST be set.");
            }
            string value;
            result.AppendFormat("{0}=", SamlConstants.SamlRequest);
            value = this.GenerateSamlRequest(RequestFormat.Text);
            string encoded = Compression.DeflateEncode(value);
            result.Append(UpperCaseUrlEncode(Uri.EscapeDataString(encoded)));

        }
    }
}
