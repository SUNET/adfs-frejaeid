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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Claim = System.Security.Claims.Claim;


namespace ADFSFrejaMFA
{
    public class FrejaRefedsAdapter : IAuthenticationAdapter
    {
        private HttpClient _httpClient;
        public FrejaService _frejaService;
        public IPersonService _personService;
        
        private static FrejaMFASettings _frejaMFASettings { get; set; }
        protected IAdapterPresentationForm CreateAdapterPresentation(string username)
        {
            return new UsernamePasswordPresentation(username);
        }
        
        protected IAdapterPresentationForm CreateAdapterPresentationOnError(string username, ExternalAuthenticationException ex)
        {
            return new UsernamePasswordPresentation(username, ex);
        }

        protected IAdapterPresentation CreateFrejaPresentation(string civicNumber)
        {
            return new FrejaPresentation(civicNumber);
        }

        protected IAdapterPresentationForm CreateFrejaPresentationOnError(ExternalAuthenticationException ex)
        {
            return new FrejaPresentation(ex);
        }

        #region IAuthenticationAdapter Members

        public IAuthenticationAdapterMetadata Metadata => new FrejaRefedsMetadata();

        public IAdapterPresentation BeginAuthentication(Claim identityClaim, HttpListenerRequest request, IAuthenticationContext authContext)
        {
            if (null == identityClaim) throw new ArgumentNullException(nameof(identityClaim));

            if (null == authContext) throw new ArgumentNullException(nameof(authContext));

            if (String.IsNullOrEmpty(identityClaim.Value))
            {
                throw new InvalidDataException(ResourceHandler.GetResource(FrejaConstants.ResourceNames.ErrorNoUserIdentity, authContext.Lcid));
            }

            authContext.Data.Add(FrejaConstants.AuthContextKeys.Identity, identityClaim.Value);
            return CreateAdapterPresentation(identityClaim.Value);
        }

        public bool IsAvailableForUser(Claim identityClaim, IAuthenticationContext context)
        {
            return true;
        }

        public IAdapterPresentation OnError(HttpListenerRequest request, ExternalAuthenticationException ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException(nameof(ex));
            }
            Log.WriteEntry("Freja OnError: " + ex.Message + " " + ex.StackTrace, EventLogEntryType.Error, 335);
            return CreateAdapterPresentationOnError(String.Empty,ex);
        }

        public void OnAuthenticationPipelineLoad(IAuthenticationMethodConfigData configData)
        {
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
                        catch(Exception ex)
                        {
                            Log.WriteEntry("Unable to load Freja config data. Check that it is registered and correct." + ex.Message + ex.StackTrace, EventLogEntryType.Information, 335);
                            throw new ArgumentException();
                        }
                    }
                    try
                    {
                        if (_frejaMFASettings != null)
                        {
                            if(_frejaMFASettings.FrejaConfig!=null)
                            {

                            }
                            else { Log.WriteEntry("No Settings provided for Freja api", EventLogEntryType.Error, 335); }
                            switch (_frejaMFASettings.UserLookupMethod.ToUpper())
                            {
                                case "LDAP":
                                    if (_frejaMFASettings.LdapConfig != null)
                                    {
                                        _personService = new PersonServiceLdap(_frejaMFASettings.LdapConfig);
                                    }
                                    else
                                    {
                                        Log.WriteEntry("FrejaSecondFactor configuration, didn't get ldap settings", EventLogEntryType.Information, 335);
                                    }
                                    break;
                                case "SQL":
                                    if (_frejaMFASettings.SqlConfig != null)
                                    {
                                        _personService = new PersonServiceSql(_frejaMFASettings.SqlConfig);
                                    }
                                    else
                                    {
                                        Log.WriteEntry("FrejaSecondFactor configuration, didn't get sql settings", EventLogEntryType.Information, 335);
                                    }
                                    break;
                            }
                        }
                        else 
                        { 
                            Log.WriteEntry("No Settings provided for FrejaMFAAdapter", EventLogEntryType.Error, 335); 
                        }
                        
                        Log.WriteEntry("Start to configure Freja", EventLogEntryType.Information, 335);
                        var cert = CertificateUtils.LoadCertificateFromStore(_frejaMFASettings.FrejaConfig.RPCertificateThumbprint, StoreName.My, StoreLocation.LocalMachine) ??
                            CertificateUtils.LoadCertificateFromStore(_frejaMFASettings.FrejaConfig.RPCertificateThumbprint, StoreName.My, StoreLocation.CurrentUser);
                        Log.WriteEntry("Freja created certificate", EventLogEntryType.Information, 335);
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

        public IAdapterPresentation TryEndAuthentication(IAuthenticationContext authContext, IProofData proofData, HttpListenerRequest request, out Claim[] outgoingClaims)
        {   
            if (null == authContext)
            {
                throw new ArgumentNullException(nameof(authContext));
            }

            outgoingClaims = new Claim[0];
           
            if (proofData?.Properties == null)
            {
                throw new ExternalAuthenticationException(ResourceHandler.GetResource(FrejaConstants.ResourceNames.ErrorNoAnswerProvided, authContext.Lcid), authContext);
            }

            if (!authContext.Data.ContainsKey(FrejaConstants.AuthContextKeys.Identity))
            {
                Log.WriteEntry("TryEndAuthentication Context does not contains userID.", EventLogEntryType.Error, 335);
                throw new ArgumentOutOfRangeException(FrejaConstants.AuthContextKeys.Identity);
            }

            Log.WriteEntry("TryEndAuthentication", EventLogEntryType.Information, 335);
            string view = (string)proofData.Properties[FrejaConstants.PropertyNames.View];
            Log.WriteEntry("View: " + view, EventLogEntryType.Information, 335);
            if (view == ResourceHandler.GetResource(FrejaConstants.ResourceNames.ViewPassword, authContext.Lcid))
            {
                if (!proofData.Properties.ContainsKey(FrejaConstants.PropertyNames.Password))
                {
                    throw new ExternalAuthenticationException(ResourceHandler.GetResource(FrejaConstants.ResourceNames.ErrorNoAnswerProvided, authContext.Lcid), authContext);
                }
                string username = (string)authContext.Data[FrejaConstants.AuthContextKeys.Identity];
                string password = (string)proofData.Properties[FrejaConstants.PropertyNames.Password];
                try
                {
                    if (PasswordValidator.Validate(username, password))
                    {
                        Log.WriteEntry("First factor : OK!", EventLogEntryType.Information, 335);
                        authContext.Data["FirstFactor"]=true;
                        Log.WriteEntry("Entering Freja flow", EventLogEntryType.Information, 335);

                        var civicNumber = _personService.GetCivicNumber(username);
                        authContext.Data["CivicNumber"] = civicNumber;
                        if (string.IsNullOrEmpty(civicNumber))
                        {
                            return CreateFrejaPresentationOnError(new ExternalAuthenticationException("FrejaError_NoCivicNumber", authContext));
                        }
                        return CreateFrejaPresentation(civicNumber);
                    }
                    else
                    {
                        return CreateAdapterPresentationOnError(username, new UsernamePasswordValidationException("Authentication failed", authContext));
                    }
                }
            
                catch (Exception ex)
                {
                    throw new UsernamePasswordValidationException(string.Format("UsernamePasswordSecondFactor password validation failed due to exception {0} failed to validate password {0}", ex), ex, authContext);
                }
            }
            // ViewFreja, handle second factor
            else
            {
                if (!proofData.Properties.ContainsKey(FrejaConstants.PropertyNames.CivicNumber))
                {
                    throw new ExternalAuthenticationException(ResourceHandler.GetResource(FrejaConstants.ResourceNames.ErrorNoAnswerProvided, authContext.Lcid), authContext);
                }
                AuthResultRequest authResultRequest = null;
                Log.WriteEntry("Starting Freja login", EventLogEntryType.Information, 335);
                AuthenticationResponse response = null;
                string civicNumber = authContext.Data["CivicNumber"].ToString(); //(string)proofData.Properties[Constants.PropertyNames.CivicNumber];
                Log.WriteEntry("Starting Freja login with civicno: " + civicNumber, EventLogEntryType.Information, 335);
                try
                {
                    var authTicket = _frejaService.RequestAuthTicket(civicNumber);
                    authResultRequest = new AuthResultRequest() { getOneAuthResultRequest = _frejaService.EncodeAuthTicket(authTicket) };
                }
                catch(Exception ex)
                {
                    Log.WriteEntry("Error in Freja login:  " + ex.Message+ " " + ex.StackTrace, EventLogEntryType.Error, 335);
                }
                
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                while (stopwatch.Elapsed.TotalSeconds < 60)
                {
                    Thread.Sleep(3000);
                    response =_frejaService.GetAuthenticationResponse(authResultRequest);
                    if (_frejaService.ValidAuthResponse(response.status))
                    {
                        if (response.requestedAttributes.ssn.ssn == civicNumber)
                        {
                            Log.WriteEntry("Freja login : OK!", EventLogEntryType.Information, 335);
                            outgoingClaims = new[]
                            {
                                new Claim( "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod", FrejaConstants.UsernamePasswordRefeds)
                            };
                            break;
                        }
                        else
                        {
                            return CreateFrejaPresentationOnError(new ExternalAuthenticationException("FrejaError_NoMatch", authContext));
                        }
                    }
                    if (_frejaService.CancelledAuthResponse(response.status)){
                        return CreateFrejaPresentationOnError(new ExternalAuthenticationException("FrejaResponse_"+response.status, authContext));
                    }
                }
                if (outgoingClaims.Length > 0 && (bool)authContext.Data["FirstFactor"] == true)
                {
                    return null;
                }
                else
                {
                    return CreateFrejaPresentationOnError(new ExternalAuthenticationException("Fel vid inloggning med freja", authContext));
                }
            }
        }
        #endregion
        
        
    }
}
