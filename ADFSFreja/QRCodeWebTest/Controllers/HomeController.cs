using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using System.Web.Mvc;
using Freja.Service;
using Freja.Settings;
using System.Threading.Tasks;
using Freja.Model;
using System.Diagnostics;
using System.Threading;
using ADFSFreja.Application.Utils;

namespace QRCodeWebTest.Controllers
{
    public class ApplyViewModel
    {
        public string Nin { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Mobile { get; set; }
    }

    public class HomeController : Controller
    {
        private const bool V = true;
        private const bool CancelOrError = false;
        private const bool V1 = true;
        public FrejaSettings _frejaSettings = new FrejaSettings();
        public FrejaService _frejaLogic;
        public HomeController()
        {
            _frejaSettings.RPCertificateThumbprint = ConfigurationManager.AppSettings.Get("");
            _frejaSettings.AuthURL = ConfigurationManager.AppSettings.Get("AuthURL");
            _frejaSettings.AuthOneResultURL = ConfigurationManager.AppSettings.Get("AuthOneResultURL");
            _frejaSettings.CancelRequestURL = ConfigurationManager.AppSettings.Get("CancelRequestURL");
            _frejaSettings.QRCodeURL = ConfigurationManager.AppSettings.Get("QRCodeURL");
            var cert = CertificateUtils.LoadCertificateFromStore(_frejaSettings.RPCertificateThumbprint, StoreName.My, StoreLocation.LocalMachine) ??
                            CertificateUtils.LoadCertificateFromStore(_frejaSettings.RPCertificateThumbprint, StoreName.My, StoreLocation.CurrentUser);
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.SslProtocols = SslProtocols.Tls12;
            handler.ClientCertificates.Add(cert);
            var _httpClient = new HttpClient(handler);
            _frejaLogic = new FrejaService(_httpClient, _frejaSettings);
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public async Task<string> QR()
        {
            var authTicket = _frejaLogic.RequestQRAuthTicket();
            var qrpicture = _frejaLogic.GetQRHref(authTicket.authRef, null);
          //  var qrpicture = _frejaLogic.GetQRHref(authTicket.authRef, "administrationsverktyg.test.umu.se");

            return qrpicture;
        }

        //public bool WaitForResponse(AuthResultRequest authResultRequest)
        //{
        //  //  authResultRequest.I
        //    AuthenticationResponse response = null;
        //    var stopwatch = new Stopwatch();
        //    stopwatch.Start();

        //    while (stopwatch.Elapsed.TotalMilliseconds < TimeSpan.FromMilliseconds(60000).TotalMilliseconds)
        //    {
        //        Thread.Sleep(3000);
        //        response = _frejaLogic.GetAuthenticationResponse(authResultRequest);
        //        if (_frejaLogic.ValidAuthResponse(response.status))
        //        {
        //            var guest = new ApplyViewModel
        //            {
        //                FirstName = response.requestedAttributes.basicUserInfo.name,
        //                LastName = response.requestedAttributes.basicUserInfo.surname,
        //                Email = response.requestedAttributes.emailAddress,
        //                Nin = response.requestedAttributes.ssn.ssn
        //            };
        //            return V1;

        //        }
        //        if (_frejaLogic.CancelledAuthResponse(response.status))
        //        {
        //            return CancelOrError;
        //        }
        //    }
        //    return CancelOrError;
        //}
      //  [HttpGet]
        public ActionResult GettingThere()
        {
            return View();
        }
    }
}