using Freja.Interfaces;
using Freja.Model;
using Freja.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Freja.Service
{
    public class FrejaService : IFrejaService
    {
        private readonly HttpClient _client;
        private readonly FrejaSettings _frejaSettings;
        public FrejaService(HttpClient client, FrejaSettings settings)
        {
            _client = client;
            _frejaSettings = settings;
        }
        private static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = ShouldSerializeContractResolver.Instance,
        };

        public string CreateEncodedSSNRequest(string ssn, string country, AuthLevel authLevel)
        {
            var userInfo = new SsnUserInfo() { country = country, ssn = ssn };
            var encodedUserInfo = EncodeUserInfo(userInfo);
            var payload = new Payload()
            {
                userInfoType = "SSN",
                userInfo = encodedUserInfo,
                minRegistrationLevel = "PLUS",
                attributesToReturn = GetReturnAttributes(authLevel) // new List<ReturnAttribute>() { new ReturnAttribute() { attribute = "BASIC_USER_INFO" } 
            };
            var encodedPayload = EncodePayload(payload);
            return encodedPayload;
        }

        public string EncodePayload(Payload payload)
        {
            string json = JsonConvert.SerializeObject(payload, JsonSettings);
            string base64EncodedString = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            return base64EncodedString;
        }

        public string EncodeUserInfo(SsnUserInfo userInfo)
        {
            string json = JsonConvert.SerializeObject(userInfo, JsonSettings);
            string base64EncodedString = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            return base64EncodedString;
        }
        public string EncodeAuthTicket(AuthTicket authTicket)
        {
            string json = JsonConvert.SerializeObject(authTicket, JsonSettings);
            string base64EncodedString = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            return base64EncodedString;
        }
        public string CreateEncodedQRRequest()
        {
            var payload = new Payload()
            {
                userInfoType = "INFERRED",
                userInfo = "N/A",
                minRegistrationLevel = "PLUS",
                attributesToReturn = GetReturnAttributes(AuthLevel.PLUS)
            };
            var encodedPayload = EncodePayload(payload);
            return encodedPayload;
        }
        public AuthenticationResponse SendAuthRequest(AuthRequest authRequest)
        {

            AuthenticationResponse response = null;
            var postData = $"initAuthRequest={authRequest.initAuthRequest}";
            //EventLog.WriteEntry("Freja eID", "authrequest: " + authRequest.initAuthRequest);
            var content = new StringContent(postData);
            //EventLog.WriteEntry("Freja eID", "authrequest content: " + content,EventLogEntryType.Information,335);
            //EventLog.WriteEntry("Freja eID", "Entering SendAuthRequest with request:" + authRequest.initAuthRequest, EventLogEntryType.Information, 335);
            try
            {
                if (_client == null)
                {
                    //Console.Out.WriteLine("Client is null");
                    //EventLog.WriteEntry("Freja eID", "SendAuthRequest client is null!", EventLogEntryType.Error, 335);
                }

                //EventLog.WriteEntry("Freja eID", "Before SendAuthRequest (" + _frejaSettings.AuthURL + ")", EventLogEntryType.Information, 335);
                //EventLog.WriteEntry("Freja eID", "Auth url: " + _frejaSettings.AuthURL, EventLogEntryType.Information, 335);
                
                var result = _client.PostAsync(_frejaSettings.AuthURL, content).GetAwaiter().GetResult();
                
                //EventLog.WriteEntry("Freja eID", "After SendAuthRequest", EventLogEntryType.Information, 335);
                var resultContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                //EventLog.WriteEntry("Freja eID", "SendAuthRequest result: " + resultContent, EventLogEntryType.Information, 335);
                Console.Out.WriteLine("AuthRequestResult: " + resultContent);
                response = JsonConvert.DeserializeObject<AuthenticationResponse>(resultContent);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("Error in SendAuthRequest:  " + ex.Message + " " + ex.StackTrace);
                //EventLog.WriteEntry("Freja eID", "Error in SendAuthRequest:  " + ex.Message + " " + ex.StackTrace, EventLogEntryType.Error, 335);
            }
            return response;
        }
        public AuthenticationResponse GetAuthenticationResponse(AuthResultRequest authResultRequest)
        {

            var postData = $"getOneAuthResultRequest={authResultRequest.getOneAuthResultRequest}";
            AuthenticationResponse response = null;
            HttpResponseMessage result = null;

            var content = new StringContent(postData);
            result = _client.PostAsync(_frejaSettings.AuthOneResultURL, content).GetAwaiter().GetResult();
            var resultContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            response = JsonConvert.DeserializeObject<AuthenticationResponse>(resultContent);
            if (ValidAuthResponse(response.status))
            {
                //the user has been successfully authenticated by frejaeID
                //possibly do stuff before notifying client
            }
            return response;
        }


        public AuthTicket RequestAuthTicket(string ssn)
        {
            var authRequest = new AuthRequest();
            authRequest.initAuthRequest = CreateEncodedSSNRequest(ssn, "SE", AuthLevel.PLUS);
            var authResponse = SendAuthRequest(authRequest);
            var authTicket = new AuthTicket() { authRef = authResponse.authRef };
            return authTicket;
        }

        public AuthTicket RequestQRAuthTicket()
        {
            var authRequest = new AuthRequest();
            authRequest.initAuthRequest = CreateEncodedQRRequest();
            var authResponse = SendAuthRequest(authRequest);
            var authTicket = new AuthTicket() { authRef = authResponse.authRef };
            return authTicket;
        }

        public string GetQRHref(string encodedAuth, string websiteaddress)
        {
            var postData = $"frejaeid://bindUserToTransaction?transactionReference={encodedAuth}";

            if (!string.IsNullOrEmpty(websiteaddress))
            {
                postData += "&originAppScheme=" + websiteaddress;
            }
            postData = WebUtility.UrlEncode(postData);

            var getCall = _frejaSettings.QRCodeURL + postData;
            return getCall;
        }

        public int SendCancelRequest(CancelAuthResultRequest cancelRequest)
        {
            var result = _client.PostAsync(_frejaSettings.CancelRequestURL, new StringContent(JsonConvert.SerializeObject(cancelRequest, JsonSettings))).GetAwaiter().GetResult();
            return (int)result.StatusCode;
        }

        private string StreamToString(Stream s)
        {
            s.Position = 0;
            using (StreamReader reader = new StreamReader(s, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
        //public string CreateEncodedSSNRequest(string ssn, string country, AuthLevel authLevel)
        //{
        //    var userInfo = new SsnUserInfo() { country = country, ssn = ssn };
        //    var encodedUserInfo = EncodeUserInfo(userInfo);
        //    Console.Out.WriteLine("EncodedUserInfo: " + encodedUserInfo);
        //    var payload = new Payload()
        //    {
        //        userInfoType = "SSN",
        //        userInfo = encodedUserInfo,
        //        minRegistrationLevel = "PLUS",
        //        attributesToReturn = GetReturnAttributes(authLevel) // new List<ReturnAttribute>() { new ReturnAttribute() { attribute = "BASIC_USER_INFO" } 
        //    };
        //    var encodedPayload = EncodePayload(payload);
        //    Console.Out.WriteLine("EncodedPayload: " + encodedPayload);
        //    return encodedPayload;
        //}
        //public string EncodePayload(Payload payload)
        //{
        //    string json = JsonConvert.SerializeObject(payload, JsonSettings);
        //    string base64EncodedString = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        //    return base64EncodedString;
        //}

        //public string EncodeUserInfo(SsnUserInfo userInfo)
        //{
        //    string json = JsonConvert.SerializeObject(userInfo, JsonSettings);
        //    string base64EncodedString = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        //    return base64EncodedString;
        //}
        //public string EncodeAuthTicket(AuthTicket authTicket)
        //{
        //    string json = JsonConvert.SerializeObject(authTicket, JsonSettings);
        //    string base64EncodedString = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        //    return base64EncodedString;
        //}
        private List<ReturnAttribute> GetReturnAttributes(AuthLevel authLevel)
        {
            var attributes = new List<ReturnAttribute>();
            switch (authLevel)
            {
                case AuthLevel.BASIC:
                    attributes.Add(new ReturnAttribute { attribute = nameof(ReturnAttributes.BASIC_USER_INFO) });
                    break;
                case AuthLevel.EXTENDED:
                case AuthLevel.PLUS:
                    attributes.Add(new ReturnAttribute { attribute = nameof(ReturnAttributes.BASIC_USER_INFO) });
                    attributes.Add(new ReturnAttribute { attribute = nameof(ReturnAttributes.DATE_OF_BIRTH) });
                    attributes.Add(new ReturnAttribute { attribute = nameof(ReturnAttributes.SSN) });
                    attributes.Add(new ReturnAttribute { attribute = nameof(ReturnAttributes.EMAIL_ADDRESS) });
                    break;
                    //default:
                    //    throw new ...
            }
            return attributes;
        }

        public bool ValidAuthResponse(string status)
        {
            ValidAuthStatusCodes statusCode;
            if (Enum.TryParse<ValidAuthStatusCodes>(status, out statusCode))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CancelledAuthResponse(string status)
        {
            InvalidValidAuthStatusCodes statusCode;

            if (Enum.TryParse<InvalidValidAuthStatusCodes>(status, out statusCode))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
