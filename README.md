# Library Introduction

Sumerge Sam2 Authentication Core is a ready to use library for adding support for Saml2 protocol. Websites built using the ASP .NET can simply plug in the library and become a service provider, which can provide Single Sign on using the SAML2 protocol.

Brief Introduction for Saml2 Sign In

![alt text](https://github.com/Sumerge/Saml-Authentication/blob/main/flow.png?raw=true)


1. The User attempt to access protected resource on service provider page.
2. The Service provider redirect the user to the identity provider portal.
3. The user login on the identity provider portal.
4. The identity provider returns with a Saml2 response containing user information.
5. The response payload is validated and parsed allowing to access the secured resource.

# Library&#39;s role

The Saml authentication core library will mange the redirections, and necessary handshaking, as well as response parsing and validating and fethcing the user data recivied from idp.

# How to use the library.

Using the library is very simple and only contains three steps.

First, you need to import the library

Second you need to add your SP and idp s configurations.

Third you need to copy and paste the following controller ( Add your business logic in the commented section which mentions add here)

```C#
public class SSOAuthController : ApiController
    {

        private SamlConfig SumergeConfig = new SamlConfig();
        [HttpGet]
        public void Login()
        {
            string RedirectUrl = SumergeConfig.GenerateSamlAuthRedirectionUrl();
            HttpContext.Current.Response.Redirect(RedirectUrl);
        }
        [HttpPost]
        [Route("acs")]
        public bool ParseACSResponse(FormDataCollection form)
        {
                string response = form.Get("SAMLResponse");
                PostLoginResponseParser LoginParser = SumergeConfig.ParseLoginResponse(response);
                if (LoginParser.ValidateSignature())
                {
                    //Add your Portal Auhtneication Logic
					// Claims can be obtained using 
					//LoginParser.SamlResponse.Assertion.AttributeStatement;
					
                }
                else
                {
                    throw new Exception("InValidCertificate");
                }
        }





        [HttpPost]
        [AllowAnonymous]
        [Route("logout")]
        public void ParseLogoutResponse(FormDataCollection form)
        {
            string response = null;
            PostLogoutResponseParser LogoutParse = null;
            try
            {
                log.Info("Entering ParseLogoutResponse with FORM Params" + form);
                response = form.Get("SAMLResponse");
                LogoutParse = SumergeConfig.ParseLogoutResponse(response);
                if (LogoutParse.ValidateSignature())
                {
					/// DELETE ADDTIONAL COOKIES HERE
					
                    HttpContext.Current.Response.SumergeConfig.PostLogoutURL;
                }
                else
                {
                    throw new Exception("InValidCertificate");
                }
            }
            catch (Exception e)
            {
                log.Error("Error @ ParseACSResponse " + e);
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            
        }

           
    }

```

## Setting the Configuration

In the Web.Config Section the following Keys need to be added.

| **Property** | **Description** |
| --- | --- |
| **SigningCertificate** | The serial number for Certificate used to Sign the SAML Request (Should be installed on the machine) |
| **PublicCertificate** | The serial number Certificate used to verify the authenticity of the SAML Response (Should be installed on the machine) |
| **PostLoginURL** | A URI reference for the Service provider, which should get a redirection once successfully logged in. |
| **IDPDestinationURL** | A URI reference for the identity provider, which the request will be sent to. |
| **Issuer** | The Service Provider name issuing the request. (Preferably the host name) |
| **PostLogoutURL** | A URI reference for the Service provider, which the with ger redirection post logout response. |
| **EntityId** | The Entity Id to be sent in Saml Request |

In the Project route config, we need to add the following route configuration if it doesn&#39;t exists

```C#
config.Routes.MapHttpRoute(

name: &quot;Saml2&quot;,

routeTemplate: &quot;{controller}/{action}&quot;

);
```
## Initiating the Request

- In initiating the login request is simple as calling a get function.
- One just needs to redirect to the following url &quot;/SSOAuth/Login&quot;
  - Then will initiate and redirect to the identity provider.
  - On successful login and request will be redirected to to the Post Login URL and can obtain the user info from the stored claims.
- In case for logouts, the request will hit the Saml Authentication core and on successful call will redirect a call to the configured post logout url , where the SP can terminate its local session and cookies.