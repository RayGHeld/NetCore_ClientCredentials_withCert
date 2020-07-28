using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MSAL_ClientCred_Cert
{

    /*
     * Reference: https://blogs.aaddevsup.xyz/2019/01/console_keyvault_clientcredentials/ -- for instructions on how to create a certificate
     */
    class Program
    {
        static String accessToken = string.Empty; // just to store the access token

        static String client_id = ""; // client id of the app registration you're using -- also used for the cert name and password in this sample
        static String tenant_id = ""; // tenant id of the tenant -- also used for the cert name in this sample
        static String cert_password = client_id;
        static String authority = $"https://login.microsoftonline.com/{tenant_id}";
        static List<String> scopes = new List<String>() { $"https://graph.microsoft.com/.default" };
        static String thumbprint = "";

        static void Main( string[] args )
        {

            Console.WriteLine( "Authenticating..." );
            GetToken();
            Console.ReadKey();
        } 

        static async void GetToken()
        {
            accessToken = string.Empty;

            IConfidentialClientApplication ica = ConfidentialClientApplicationBuilder.Create( client_id )
                .WithCertificate(GetCert( @"C:\Users\raheld\OneDrive - Microsoft\Documents\Certs\"))
                //.WithCertificate(GetCertByThumbprint( StoreLocation.CurrentUser, thumbprint ) )
                //.WithCertificate( GetCertByName( StoreLocation.CurrentUser, $"{tenant_id}.{client_id}" ) )
                .WithTenantId(tenant_id)
                .WithAuthority(authority)
                .Build();

            AuthenticationResult authResult = null;

            try
            {
                authResult = await ica.AcquireTokenForClient( scopes ).ExecuteAsync();
                accessToken = authResult.AccessToken;

                Console.WriteLine( $"Access Token: {accessToken}" );
            } catch ( Exception ex )
            {
                Console.WriteLine( $"MSAL Error: {ex.Message}" );
            }

            
        }

        /// <summary>
        /// Gets a certificate by the .pfx name from the file system
        /// </summary>
        /// <param name="basePath">The folder where the cert is stored -- filename is derived from the settings in this application</param>
        /// <returns></returns>
        static X509Certificate2 GetCert(String basePath)
        {
            String certPath = $"{basePath}{tenant_id}.{client_id}.pfx";
            X509Certificate2 cer = new X509Certificate2( certPath, cert_password, X509KeyStorageFlags.EphemeralKeySet );

            return cer;
        }

        /// <summary>
        /// Gets a certificate based on the name -- please note, there is an issue using this method
        /// if there are multiple certs with the same name.
        /// </summary>
        /// <param name="storeLoc"></param>
        /// <param name="subjectName"></param>
        /// <returns></returns>
        static X509Certificate2 GetCertByName(StoreLocation storeLoc, String subjectName )
        {

            // read certificates already installed from the certificate store
            X509Store store = new X509Store( storeLoc );
            X509Certificate2 cer = null;

            store.Open( OpenFlags.ReadOnly );

            // look for the specific cert by subject name -- returns the first one found, if any.  Also, for self signed, set the valid parameter to 'false'
            X509Certificate2Collection cers = store.Certificates.Find( X509FindType.FindBySubjectName, subjectName, false );
            if ( cers.Count > 0 )
            {
                cer = cers[ 0 ];
            };
            store.Close();

            return cer;

        }

        /// <summary>
        /// Gets a certificate by Thumbprint -- this is the preferred method
        /// </summary>
        /// <param name="storeLoc"></param>
        /// <param name="thumbprint"></param>
        /// <returns></returns>
        static X509Certificate2 GetCertByThumbprint(StoreLocation storeLoc, String thumbprint )
        {
            X509Store store = new X509Store( storeLoc );
            X509Certificate2 cer = null;

            store.Open( OpenFlags.ReadOnly );

            // look for the specific cert by thumbprint -- Also, for self signed, set the valid parameter to 'false'
            X509Certificate2Collection cers = store.Certificates.Find( X509FindType.FindByThumbprint, thumbprint,false );
            if ( cers.Count > 0 )
            {
                cer = cers[ 0 ];
            };
            store.Close();

            return cer;
        }
    }
}
