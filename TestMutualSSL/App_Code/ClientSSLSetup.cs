using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;

/// <summary>
/// Summary description for ClientSSLSetup
/// </summary>
public static class ClientSSLSetup
{
    public static X509Certificate2 CreateMutualSslClient(string serial)
    {
        X509Certificate2 clientCertificate;
        if (TryFindClientCertificate(serial, out clientCertificate))
        {
            return clientCertificate;
        }
        return null;
    }

    public static bool TryFindClientCertificate(string clientCertSerial, out X509Certificate2 certificate)
    {
        certificate = null;
        const StoreName personal = StoreName.My;
        const StoreLocation currentUser = StoreLocation.CurrentUser;
        const StoreLocation localMachine = StoreLocation.LocalMachine;
        var store = new X509Store(personal, currentUser);
        var store2 = new X509Store(personal, localMachine);
        var clientCert = searchCertificateSerial(clientCertSerial, ref store);
        if (clientCert != null)
        {
            certificate = clientCert;
            return true;
        }

        clientCert = searchCertificateSerial(clientCertSerial, ref store2);

        return clientCert != null;
    }

    public static X509Certificate2 searchCertificateSerial(string certificateSerial, ref X509Store store)
    {
        var certFound = false;
        X509Certificate2 clientCertificate = null;
        try
        {
            store.Open(OpenFlags.ReadOnly);
            var certificates = store.Certificates;
            foreach (var certificate in certificates)
            {
                if (certificate.SerialNumber != null &&
                    certificate.SerialNumber.Equals(certificateSerial.ToUpper()))
                {
                    certFound = true;
                    clientCertificate = certificate;
                    store.Close();
                    break;
                }
            }

            if (!certFound)
            {

            }

            // Log.Debug("Client Certificate not found in store.");
        }
        catch (Exception ex)
        {
            // Log.Debug("Failed to find client certificate. General exception occurred", ex);
        }

        return clientCertificate;
    }
}