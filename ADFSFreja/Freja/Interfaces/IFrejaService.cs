using Freja.Model;
using System.Collections.Generic;
namespace Freja.Interfaces
{
    public interface IFrejaService
    {
        bool CancelledAuthResponse(string status);
        string CreateEncodedQRRequest();
        string CreateEncodedSSNRequest(string ssn, string country, AuthLevel authLevel, List<ReturnAttribute> requestedAttributes);
        string EncodeAuthTicket(AuthTicket authTicket);
        string EncodePayload(Payload payload);
        string EncodeUserInfo(SsnUserInfo userInfo);
        AuthenticationResponse GetAuthenticationResponse(AuthResultRequest authResultRequest);
        string GetQRHref(string encodedAuth, string websiteaddress);
        AuthTicket RequestAuthTicket(string ssn, List<ReturnAttribute> requestedAttributes = null);
        AuthTicket RequestQRAuthTicket();
        AuthenticationResponse SendAuthRequest(AuthRequest authRequest);
        int SendCancelRequest(CancelAuthResultRequest cancelRequest);
        bool ValidAuthResponse(string status);
    }
}