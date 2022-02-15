
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Freja.Service;
using Freja.Settings;
using ADFSFreja.Application.Utils;

namespace FrejaMFATest
{
    public class BaseTest
    {
        protected FrejaService _frejaLogic { get; set; }
        protected FrejaSettings _frejaSettings { get; set; }
        public string Uid { get; set; }
        public string CivicNumber { get; set; }
        public string Password { get; set; }
        public BaseTest()
        {
            Configure();
            Uid = ConfigurationManager.AppSettings["uid"];
            CivicNumber = ConfigurationManager.AppSettings["civicNumber"];
            Password = ConfigurationManager.AppSettings["password"];
        }
        private void Configure()
        {
            var _frejaSettings = new FrejaSettings()
            {
                RPCertificateThumbprint = ConfigurationManager.AppSettings.Get("RPCertificateThumbprint"),
                AuthURL = ConfigurationManager.AppSettings.Get("AuthURL"),
                AuthOneResultURL = ConfigurationManager.AppSettings.Get("AuthOneResultURL"),
                CancelRequestURL = ConfigurationManager.AppSettings.Get("CancelRequestURL")
            };
            var cert = CertificateUtils.LoadCertificateFromStore(_frejaSettings.RPCertificateThumbprint, StoreName.My, StoreLocation.LocalMachine) ??
                            CertificateUtils.LoadCertificateFromStore(_frejaSettings.RPCertificateThumbprint, StoreName.My, StoreLocation.CurrentUser);
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.SslProtocols = SslProtocols.Tls12;
            handler.ClientCertificates.Add(cert);
            var _httpClient = new HttpClient(handler);
            this._frejaLogic = new FrejaService(_httpClient, _frejaSettings);

        }
    }
}
