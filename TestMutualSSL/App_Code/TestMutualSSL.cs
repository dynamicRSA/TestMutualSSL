using System.Web.Services;
using System.Net;
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


/// <summary>
/// This Web Service is to test Mutual Authentication between the client and a server
/// </summary>
[WebService(Namespace = "TestMutualSSL/v1")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class TestMutualSSL : System.Web.Services.WebService
{
    public static WebProxy Proxy;
    [WebMethod]
    public string TestSsl(string certSerial, string requestString, string url)
    {
        var response = "";
        try
        {
            //check input
            if (string.IsNullOrEmpty(certSerial)) return response = "Cert Serial is null or empty";
            if (string.IsNullOrEmpty(requestString)) return response = "Request String is null or empty";
            if (string.IsNullOrEmpty(url)) return response = "URL is null or empty";

            //get SSL certificate from certSerial
            if (ClientSSLSetup.CreateMutualSslClient(certSerial) != null)
            {
                X509Certificate clientCert = ClientSSLSetup.CreateMutualSslClient(certSerial);
                var req = (HttpWebRequest) WebRequest.Create(url);
                req.ContentType = "text/xml";
                req.Method = "GET";
                req.UserAgent = "Identify Agent";
                req.ClientCertificates.Add(clientCert);
                req.AuthenticationLevel = AuthenticationLevel.MutualAuthRequired;
                req.GetResponse();
                response = "Successfully connected with Mutual Authentication";
            }
            else
            {
                response = "Error Occurred";
            }
        }
        catch (Exception ex)
        {
            response = "Error Occurred " + ex.Message;
        }
        return response;
        }
}


