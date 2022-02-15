using ADFSFreja.Application;
using ADFSFreja.Application.Interfaces;
using ADFSFreja.Application.Services;
using ADFSFreja.Application.Settings;
using ADFSFreja.Application.Utils;
using Freja.Model;
using Freja.Service;
using Microsoft.IdentityServer.Web.Authentication.External;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Claim = System.Security.Claims.Claim;

namespace ADFSFrejaSecondFactor
{
    public class FrejaAdapter : IAuthenticationAdapter
    {
        private HttpClient _httpClient;
        public FrejaService _frejaService;
        public IPersonService _personService;
        private static FrejaMFASettings _frejaMFASettings { get; set; }
        protected IAdapterPresentationForm CreateAdapterPresentation(string civicnumber)
        {
            return new FrejaPresentation(civicnumber);
        }
        protected IAdapterPresentationForm CreateAdapterPresentationOnError(string civicnumber, ExternalAuthenticationException ex)
        {
            return new FrejaPresentation(civicnumber, ex);
        }
        
        public IAuthenticationAdapterMetadata Metadata => new FrejaAdapterMetadata();

        public IAdapterPresentation BeginAuthentication(Claim identityClaim, HttpListenerRequest request, IAuthenticationContext authContext)
        {
            Log.WriteEntry("Enter begin Authentication in FrejaAdapter", EventLogEntryType.Information, 335);
            if (null == identityClaim) throw new ArgumentNullException(nameof(identityClaim));

            if (null == authContext) throw new ArgumentNullException(nameof(authContext));

            if (String.IsNullOrEmpty(identityClaim.Value))
            {
                throw new InvalidDataException(ResourceHandler.GetResource(FrejaConstants.ResourceNames.ErrorNoUserIdentity, authContext.Lcid));
            }


            Log.WriteEntry("Proceeding with " + identityClaim.Value, EventLogEntryType.Information, 335);
            authContext.Data.Add(FrejaConstants.AuthContextKeys.Identity, identityClaim.Value);
            var civicNumber = _personService.GetCivicNumber(identityClaim.Value).Trim();
            authContext.Data.Add(FrejaConstants.AuthContextKeys.CivicNumber, civicNumber);
            Log.WriteEntry("Proceeding with civicno: " + civicNumber, EventLogEntryType.Information, 335);
            if (civicNumber != null)
            {
                return CreateAdapterPresentation(civicNumber);
            }
            else
            {
                return CreateAdapterPresentationOnError("", new ExternalAuthenticationException("ErrorMissingCivicNo", authContext));
            }

        }

        public bool IsAvailableForUser(Claim identityClaim, IAuthenticationContext context)
        {
            return true;
        }

        public void OnAuthenticationPipelineLoad(IAuthenticationMethodConfigData configData)
        {
            Log.WriteEntry("Loading FrejaAdapter", EventLogEntryType.Information, 335);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            if (configData != null)
            {
                if (configData.Data != null)
                {
                    using (StreamReader reader = new StreamReader(configData.Data, Encoding.UTF8))
                    {
                        //Config should be in a json format, and needs to be registered with the 
                        //-ConfigurationFilePath parameter when registering the MFA Adapter (Register-AdfsAuthenticationProvider cmdlet)
                        try
                        {
                            var config = reader.ReadToEnd();
                            var js = new DataContractJsonSerializer(typeof(FrejaMFASettings));
                            var ms = new MemoryStream(UTF8Encoding.UTF8.GetBytes(config));
                            var mfaConfig = (FrejaMFASettings)js.ReadObject(ms);
                            _frejaMFASettings = mfaConfig;
                            Log.WriteEntry("Freja configuration loaded with the loginUrl: " + mfaConfig.FrejaConfig.AuthURL, EventLogEntryType.Information, 335);
                        }
                        catch (Exception ex)
                        {
                            Log.WriteEntry("Unable to load Freja config data. Check that it is registered and correct." + ex.Message + ex.StackTrace, EventLogEntryType.Information, 335);
                            throw new ArgumentException();
                        }
                    }
                    try
                    {
                        if (_frejaMFASettings != null)
                        {
                            if (_frejaMFASettings.FrejaConfig != null)
                            {
                                Log.WriteEntry("Start to configure Freja", EventLogEntryType.Information, 335);
                                
                                var cert = CertificateUtils.LoadCertificateFromStore(_frejaMFASettings.FrejaConfig.RPCertificateThumbprint, StoreName.My, StoreLocation.LocalMachine) ??
                                CertificateUtils.LoadCertificateFromStore(_frejaMFASettings.FrejaConfig.RPCertificateThumbprint, StoreName.My, StoreLocation.CurrentUser);
                                Log.WriteEntry("Freja created certificate (" + cert.Subject +")", EventLogEntryType.Information, 335);
                                var handler = new HttpClientHandler();
                                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                                handler.SslProtocols = SslProtocols.Tls12;
                                handler.AllowAutoRedirect = true;
                                handler.ClientCertificates.Add(cert);
                                Log.WriteEntry("Freja created handler", EventLogEntryType.Information, 335);
                                _httpClient = new HttpClient(handler);
                                Log.WriteEntry("Freja created httpclient", EventLogEntryType.Information, 335);
                                _frejaService = new FrejaService(_httpClient, _frejaMFASettings.FrejaConfig);
                                Log.WriteEntry("Freja created logic", EventLogEntryType.Information, 335);
                            }
                            else { Log.WriteEntry("No Settings provided for Freja api", EventLogEntryType.Error, 335); }
                            
                            if (_frejaMFASettings.LdapConfig != null)
                            {
                                //Set up LdapService provided
                                if (!string.IsNullOrEmpty(_frejaMFASettings.LdapConfig.UserName))
                                {
                                    _personService = new PersonServiceLdap(_frejaMFASettings.LdapConfig);
                                }

                            }
                            if (_frejaMFASettings.SqlConfig != null)
                            {
                                Log.WriteEntry("Freja, got sqlsettings", EventLogEntryType.Information, 335);
                                _personService = new PersonServiceSql(_frejaMFASettings.SqlConfig);
                            }
                            else
                            {
                                Log.WriteEntry("Freja, didn't get sqlsettings", EventLogEntryType.Information, 335);
                            }
                        }
                        else { Log.WriteEntry("No Settings provided for FrejaMFAAdapter", EventLogEntryType.Error, 335); }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteEntry("Unable to configure Freja" + ex.Message + ex.StackTrace, EventLogEntryType.Information, 335);
                        throw new ArgumentException();
                    }
                }
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public void OnAuthenticationPipelineUnload()
        {
            
        }

        public IAdapterPresentation OnError(HttpListenerRequest request, ExternalAuthenticationException ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException(nameof(ex));
            }
            Log.WriteEntry("Freja OnError: " + ex.Message + " " + ex.StackTrace, EventLogEntryType.Error, 335);
            return CreateAdapterPresentationOnError("",ex);
        }

        public IAdapterPresentation TryEndAuthentication(IAuthenticationContext authContext, IProofData proofData, HttpListenerRequest request, out Claim[] outgoingClaims)
        {
            var civicNumber = "";
            if (null == authContext)
            {
                throw new ArgumentNullException(nameof(authContext));
            }
            
            outgoingClaims = new Claim[0];

            if (proofData?.Properties == null)
            {
                throw new ExternalAuthenticationException("Error",authContext);
                //throw new ExternalAuthenticationException(ResourceHandler.GetResource(Constants.ResourceNames.ErrorNoAnswerProvided, authContext.Lcid), authContext);
            }

            if (!authContext.Data.ContainsKey(FrejaConstants.AuthContextKeys.Identity))
            {
                Log.WriteEntry("TryEndAuthentication Context does not contains userID.",EventLogEntryType.Error,335);
                throw new ArgumentOutOfRangeException(FrejaConstants.AuthContextKeys.Identity);
            }

            
            if (!authContext.Data.ContainsKey(FrejaConstants.AuthContextKeys.CivicNumber))
            {
                Log.WriteEntry("TryEndAuthentication Context does not contains civicnumber.",EventLogEntryType.Error,335);
                throw new ExternalAuthenticationException(ResourceHandler.GetResource(FrejaConstants.ResourceNames.ErrorNoAnswerProvided, authContext.Lcid), authContext);
            }
            else
            {
                civicNumber = (string)authContext.Data[FrejaConstants.AuthContextKeys.CivicNumber];
                Log.WriteEntry("Got civicnumber from authcontext (" + civicNumber + ")", EventLogEntryType.Information, 335);
            }

            AuthResultRequest authResultRequest = null;
            Log.WriteEntry("Starting Freja login", EventLogEntryType.Information, 335);
            AuthenticationResponse response = null;
            

            Log.WriteEntry("Starting Freja login with civicno: " + civicNumber, EventLogEntryType.Information, 335);
            try
            {
                var authTicket = _frejaService.RequestAuthTicket(civicNumber);
                authResultRequest = new AuthResultRequest() { getOneAuthResultRequest = _frejaService.EncodeAuthTicket(authTicket) };
            }
            catch (Exception ex)
            {
                Log.WriteEntry("Error in Freja login:  " + ex.Message + " " + ex.StackTrace, EventLogEntryType.Error, 335);
                return CreateAdapterPresentationOnError(civicNumber, new ExternalAuthenticationException("FrejaError_NoCivicNumber", authContext));
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.Elapsed.TotalSeconds < 60)
            {
                Thread.Sleep(3000);
                response = _frejaService.GetAuthenticationResponse(authResultRequest);
                if (_frejaService.ValidAuthResponse(response.status))
                {
                    if (response.requestedAttributes.ssn.ssn == civicNumber)
                    {

                        Log.WriteEntry("Freja login : OK!", EventLogEntryType.Information, 335);
                        outgoingClaims = new[]
                        {
                            new Claim( "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod",
                            FrejaConstants.FrejaMFA)
                        };
                        break;
                    }
                    else
                    {
                        return CreateAdapterPresentationOnError(civicNumber,new ExternalAuthenticationException("FrejaError_NoMatch" + response.status, authContext));
                    }
                }
                if (_frejaService.CancelledAuthResponse(response.status))
                {
                    return CreateAdapterPresentationOnError(civicNumber,new ExternalAuthenticationException("FrejaResponse_" + response.status, authContext));
                }
            }

            if (outgoingClaims.Length > 0 )
            {
                return null;
            }
            else
            {
                return CreateAdapterPresentationOnError(civicNumber,new ExternalAuthenticationException("FrejaResponse_EXPIRED", authContext));
            }

        }
    }
}
