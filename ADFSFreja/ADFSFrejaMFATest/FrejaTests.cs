using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Freja.Model;
using Freja.Settings;
using System.Configuration;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.Threading;
using System.Diagnostics;
using Microsoft.IdentityServer.Web.Authentication.External;
using System.Collections.Generic;
using System.Security.Claims;
using System.IO;
using Newtonsoft.Json;
using Freja.Service;
using ADFSFreja.Application.Settings;
using ADFSFrejaSecondFactor;
using ADFSFreja.Application.Services;
using ADFSFreja.Application;
using ADFSFrejaMFA;

namespace FrejaMFATest
{
    [TestClass]
    public class FrejaTests : BaseTest
    {
        public FrejaTests() : base()
        {

        }
        [TestMethod]
        public void FrejaNinTest()
        {
            
            AuthenticationResponse response = null;
            var authTicket = _frejaLogic.RequestAuthTicket(CivicNumber);
            var authResultRequest = new AuthResultRequest() { getOneAuthResultRequest = _frejaLogic.EncodeAuthTicket(authTicket) };
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.Elapsed.TotalMilliseconds < TimeSpan.FromMilliseconds(60000).TotalMilliseconds)
            {
                Thread.Sleep(3000);
                response = _frejaLogic.GetAuthenticationResponse(authResultRequest);
                if (_frejaLogic.ValidAuthResponse(response.status))
                {
                    Assert.AreEqual(AuthStatusCodes.APPROVED.ToString(), response.status);
                    Console.Out.WriteLine("Authentication completed with status APPROVED");
                    break;
                }
                if (_frejaLogic.CancelledAuthResponse(response.status))
                {
                    Console.Out.WriteLine("Authentication failed, reason=" + response.status);
                    break;
                }
            }
        }

        [TestMethod]
        [DeploymentItem("FrejaSettings.json")]
        public void GetCivicnumberTest()
        {
            var settings = new FrejaMFASettings();
            using (StreamReader file = File.OpenText(@"FrejaSettings.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                settings = (FrejaMFASettings)serializer.Deserialize(file, typeof(FrejaMFASettings));
            }
            var personService = new PersonServiceSql(settings.SqlConfig);
           
            var nin = personService.GetCivicNumber(this.Uid);
            Assert.IsNotNull(nin);
            Assert.AreNotEqual("", nin);
            
        }

        [TestMethod]
        [DeploymentItem("FrejaSettings.json")]
        public void TryEndAuthenticationTest()
        {
            
            var d = new Dictionary<string, object>();
            d.Add("id",this.Uid);
            d.Add("CivicNumber", this.CivicNumber);
            IAuthenticationContext authContext = new AuthenticationContext()
            {
                ActivityId = "minAktivitet",
                ContextId = "minContext",
                Lcid = 1033,
                Data = d
            };
            IAuthenticationMethodConfigData configData = new ConfigData()
            {
                Data = File.Open("FrejaSettings.json", FileMode.Open)
            };


            IProofData proofData = null;// new ProofData();
            var outgoingClaims = new Claim[] { };
            var adapter = new FrejaRefedsAdapter();
            adapter.OnAuthenticationPipelineLoad(configData);

            //first factor/step
            proofData = new ProofData() { Properties = GetFirstFactorProperties() };
            try
            {
                adapter.TryEndAuthentication(authContext, proofData, null, out outgoingClaims);
            }
            catch (TypeInitializationException typeEx) 
            {
                // ok, can´t handle that were in a different domain. Simulate first factor success
                authContext.Data["FirstFactor"] = true;
                // ok, cant handle the presentationform creation
            }

            //second factor
            proofData = new ProofData() { Properties = GetSecondFactorProperties() };
            IAdapterPresentation presentation = adapter.TryEndAuthentication(authContext, proofData, null, out outgoingClaims);
            if (presentation != null)
            {
                Assert.Fail("Misslyckades med inloggning");
            }
            

        }

        [TestMethod]
        public void GetPresentationFormTest()
        {
            //var content = ResourceHandler.GetPresentationResource("FrejaResponse_CANCELED", 1053);
            var text = ResourceHandler.GetResource("PageFrejaInstruction", 29); 
            var textSv = ResourceHandler.GetResource("FrejaResponse_CANCELED", 29);
            var textEn = ResourceHandler.GetResource("FrejaResponse_CANCELED", 9);
            var foo = "";
        }
        [TestMethod]
        public void TestSettings()
        {
            
            var fs = new FrejaSettings() { RPCertificateThumbprint="a", AuthOneResultURL = "c", AuthURL = "d", CancelRequestURL = "e" };
            var ls = new LdapSettings() { SearchRoot = "a", Filter = "b", AttributeToRetrieve = "c" };
            var s = new FrejaMFASettings() { FrejaConfig = fs, LdapConfig = ls };
            string output = JsonConvert.SerializeObject(s);
            var foo="";
        }


        private Dictionary<string,object> GetFirstFactorProperties()
        {
            var d = new Dictionary<string, object>();
            d.Add("View", "Password");
            d.Add("PasswordInput", Password);
            return d;
        }
        private Dictionary<string, object> GetSecondFactorProperties()
        {
            var d = new Dictionary<string, object>();
            d.Add("View", "Freja");
            d.Add("CivicNumberInput", CivicNumber);
            return d;
        }

        
    }

    
}
