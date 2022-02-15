using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace ADFSFreja.Application.Utils
{
    public static class CertificateUtils
    {
        /// <summary>
        /// Gets certificate from store using thumbprint as reference. 
        /// Certificate can be installed in azure aswell as locally.
        /// Azure: App Service => TLS/SSL settings => Private Key Certificates
        /// Locally: MMC => Local Computer => Personal
        /// </summary>
        /// <param name="thumbprint"></param>
        /// <returns></returns>
        public static X509Certificate2 LoadCertificateFromStore(string thumbprint, StoreName storeName, StoreLocation location)
        {
            using (X509Store userCaStore = new X509Store(storeName, location))
            {
                try
                {
                    userCaStore.Open(OpenFlags.ReadOnly);

                    X509Certificate2Collection certificatesInStore = userCaStore.Certificates;

                    var certificate = certificatesInStore
                        .Find(X509FindType.FindByThumbprint, thumbprint, false)
                        .OfType<X509Certificate2>().FirstOrDefault();

                    return certificate;
                }
                catch (Exception e)
                {
                    throw new Exception($"An issue occurred while loading certificate with thumbprint {thumbprint}.", e);
                }
                finally
                {
                    userCaStore.Close();
                }
            }
        }
    }
}
