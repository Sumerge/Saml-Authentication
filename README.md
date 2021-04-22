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

The Saml authentication core library will manage the redirections, and necessary handshaking, as well as response parsing and validating and fetching the user data received from idp.

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

name: Saml2,

routeTemplate: {controller}/{action}

);
```
The SamlConfig can also be configured using a parameterized construtor as well 

```C#
 public SamlConfig(string ACSURL, string Issuer, String Destination, string EntityId, string SigningCertifcatePath, string SigningCertPass, string PublicCertificatePath)
        
```

## Initiating the Request

- In initiating the login request is simple as calling a get function.
- One just needs to redirect to the following url &quot;/SSOAuth/Login&quot;
  - Then will initiate and redirect to the identity provider.
  - On successful login and request will be redirected to to the Post Login URL and can obtain the user info from the stored claims.
- In case for logouts, the request will hit the Saml Authentication core and on successful call will redirect a call to the configured post logout url , where the SP can terminate its local session and cookies.



# Certificate Installation Step

This section explains the step need to install the PFX certificate on the machines.

- Starting by righting click on certificate in click install PFX, with admin rights
- Chose the Store Location to be **LOCAL MACHINE**
- Enter the password if Any
- Select the Certificate Store to be custom
  - Press the Browser Option and choose **Personal**

- Finalize the remaining and press finish

Once successfully installed, we need to give IIS the certificate rights

## IIS Certificate Control access

- Open **Manage Computer Certificates**
- Find the installed certificate in **personal**

- Right click on the certificate and choose
  - All Tasks
    - **Mange Private Keys**

- A pop up will open, saying to SELECT USER just press the **advanced** button
- A new window will open click on **Select a Principal**
- From the location tab change the **location** to be the master machine ie the **Node Name**
- Press the **Find now** button and Search results will appear
- Find the **User IIS\_USER** and choose

- Give the IIS\_USER full perssion **FULL CONTROL** and **READ**

## Load User Profile Step

- Once that is completed go to the IIS server and open the Portal Application pool.
- Go to Advanced Pool Settings
- Go to Process Model and set **Load User Profile** to **TRUE**
