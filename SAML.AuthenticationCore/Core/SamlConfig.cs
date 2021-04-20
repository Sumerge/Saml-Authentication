using SAML.AuthenticationCore.Constants;
using SAML.AuthenticationCore.Entities;
using SAML.AuthenticationCore.ProtocolBinder;
using SAML.AuthenticationCore.Utilities;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Serialization;
using System.Configuration;
namespace SAML.AuthenticationCore.Core
{
    public class SamlConfig
    {
        private X509Certificate2 SigningCertificate;
        private X509Certificate2 PublicCertificate;
        public String ACSURL { get; private set; }
        public String LogOutURL { get; private set; }
        public String Destination { get; private set; }
        public String EntityId { get; private set; }
        public String Issuer { get; private set; }
        public String PostLogoutURL { get; private set; }

        public SamlConfig(string ACSURL, string Issuer, String Destination, string EntityId, string SigningCertifcatePath, string SigningCertPass, string PublicCertificatePath)
        {
            this.ACSURL = ACSURL;
            this.Issuer = Issuer;
            this.Destination = Destination;
            this.EntityId = EntityId;
            this.SigningCertificate = new X509Certificate2(SigningCertifcatePath, SigningCertPass);
            this.PublicCertificate = new X509Certificate2(PublicCertificatePath);

        }

        public SamlConfig()
        {
            this.Issuer = ConfigurationManager.AppSettings["Issuer"];
            this.Destination = ConfigurationManager.AppSettings["IDPDestinationURL"];
            this.EntityId = ConfigurationManager.AppSettings["EntityId"];
            this.PostLogoutURL = ConfigurationManager.AppSettings["PostLogoutURL"];
            this.ACSURL = this.Issuer + "SSOAuth/acs";

            X509Store store = new X509Store(StoreLocation.LocalMachine);
            
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection cers = store.Certificates.Find(X509FindType.FindBySerialNumber, ConfigurationManager.AppSettings["SigningCertificate"],false);
            if (cers.Count > 0)
            {
                this.SigningCertificate = cers[0];
            }
            else
            {
                throw new Exception("Signing Certifcate Not Found");
            }
            X509Certificate2Collection cers2 = store.Certificates.Find(X509FindType.FindBySerialNumber, ConfigurationManager.AppSettings["PublicCertificate"], false);
            if (cers2.Count > 0)
            {
                this.PublicCertificate = cers2[0];
            }
            else
            {
                throw new Exception("Public Certifcate Not Found");
            }
            store.Close();

        }

        public String GenerateSamlAuthRedirectionUrl()
        {
            //XmlDocument authRequest = this.GetAuthRequestXml();
            XmlSerializerNamespaces XmlNamespaces = new XmlSerializerNamespaces();
            XmlNamespaces.Add(SamlConstants.SamlPrefix, SamlConstants.SamlProtocolNameSpace);
            XmlNamespaces.Add(SamlConstants.Saml, SamlConstants.AssertionNameSpace);
            AuthnRequest authReq = new AuthnRequest(this.Issuer,this.ACSURL,this.Destination, SamlConstants.ProtocolBindingValue);
            XmlDocument authRequestXML = XMLUtilites.SerialzetoXML(authReq, XmlNamespaces);
            HttpRedirectBuilder redirectBuilder = new HttpRedirectBuilder(this.SigningCertificate, authRequestXML);
            String redirectionUrl = redirectBuilder.GenerateRedirectUrl(this.Destination);
            return redirectionUrl;
        }
        public String GenerateLogoutRedirectionUrl(string nationalId,string sessionIndex)
        {
            //XmlDocument authRequest = this.GetAuthRequestXml();
            XmlSerializerNamespaces XmlNamespaces = new XmlSerializerNamespaces();
            XmlNamespaces.Add(SamlConstants.SamlPrefix, SamlConstants.SamlProtocolNameSpace);
            XmlNamespaces.Add(SamlConstants.Saml, SamlConstants.AssertionNameSpace);
            LogoutRequest logoutReq = new LogoutRequest(this.Issuer, nationalId, sessionIndex, this.Destination, SamlConstants.SLO);
            XmlDocument logoutReqXML = XMLUtilites.SerialzetoXML(logoutReq, XmlNamespaces);
            HttpRedirectBuilder redirectBuilder = new HttpRedirectBuilder(this.SigningCertificate, logoutReqXML);
            String redirectionUrl = redirectBuilder.GenerateRedirectUrl(this.Destination);
            return redirectionUrl;
        }
        public PostLoginResponseParser ParseLoginResponse(String response)
        {
            PostLoginResponseParser loginParser = new PostLoginResponseParser(response, PublicCertificate);
            return loginParser;
        }
        public PostLogoutResponseParser ParseLogoutResponse(String response)
        {
            PostLogoutResponseParser logoutParser = new PostLogoutResponseParser(response, PublicCertificate);
            return logoutParser;
        }
        private XmlDocument GetAuthRequestXml()
        {
            XmlDocument SamlXmlDoc = new XmlDocument();
            using (XmlWriter xw = SamlXmlDoc.CreateNavigator().AppendChild())
            {
                string IssueInstant = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                xw.WriteStartElement(SamlConstants.SamlPrefix, SamlConstants.AuthRequest, SamlConstants.SamlProtocolNameSpace);

                xw.WriteAttributeString(SamlConstants.ID, "_" + Guid.NewGuid().ToString());
                xw.WriteAttributeString(SamlConstants.Version, "2.0");
                xw.WriteAttributeString(SamlConstants.IssueInstant, IssueInstant);
                xw.WriteAttributeString(SamlConstants.ProtocolBinding, SamlConstants.ProtocolBindingValue);
                xw.WriteAttributeString(SamlConstants.AssertionService, this.ACSURL);
                xw.WriteAttributeString(SamlConstants.Destination, this.Destination);

                xw.WriteStartElement(SamlConstants.Saml, SamlConstants.Issuer, SamlConstants.AssertionNameSpace);
                xw.WriteString(this.Issuer);
                xw.WriteEndElement();

                xw.WriteEndElement();


            }
            return SamlXmlDoc;

        }
        private XmlDocument GetLogoutRequestXml()
        {
            XmlDocument SamlXmlDoc = new XmlDocument();
            using (XmlWriter xw = SamlXmlDoc.CreateNavigator().AppendChild())
            {
                string IssueInstant = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                xw.WriteStartElement(SamlConstants.SamlPrefix, SamlConstants.LogoutRequest, SamlConstants.SamlProtocolNameSpace);

                xw.WriteAttributeString(SamlConstants.ID, "_" + Guid.NewGuid().ToString());
                xw.WriteAttributeString(SamlConstants.Version, "2.0");
                xw.WriteAttributeString(SamlConstants.IssueInstant, IssueInstant);
                xw.WriteAttributeString(SamlConstants.Reason, SamlConstants.SLO);
                xw.WriteAttributeString(SamlConstants.Destination, this.Destination);

                xw.WriteStartElement(SamlConstants.Saml, SamlConstants.Issuer, SamlConstants.AssertionNameSpace);
                xw.WriteString(this.Issuer);
                xw.WriteEndElement();

                xw.WriteEndElement();


            }
            return SamlXmlDoc;

        }
    }
}
