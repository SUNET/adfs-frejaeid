using Freja.Model;

namespace Freja.Interfaces
{
    public interface IFrejaService
    {
        bool CancelledAuthResponse(string status);
        string CreateEncodedQRRequest();
        string CreateEncodedSSNRequest(string ssn, string country, AuthLevel authLevel);
        string EncodeAuthTicket(AuthTicket authTicket);
        string EncodePayload(Payload payload);
        string EncodeUserInfo(SsnUserInfo userInfo);
        AuthenticationResponse GetAuthenticationResponse(AuthResultRequest authResultRequest);
        string GetQRHref(string encodedAuth, string websiteaddress);
        AuthTicket RequestAuthTicket(string ssn);
        AuthTicket RequestQRAuthTicket();
        AuthenticationResponse SendAuthRequest(AuthRequest authRequest);
        int SendCancelRequest(CancelAuthResultRequest cancelRequest);
        bool ValidAuthResponse(string status);
    }
}