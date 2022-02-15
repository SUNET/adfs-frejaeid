using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Freja.Model
{
    public class AuthenticationResponse
    {
        public string authRef { get; set; }
        public string status { get; set; }
        public RequestedAttributes requestedAttributes { get; set; }
        public string details { get; set; }
    }
    public class RequestedAttributes
    {
        public UserInfo basicUserInfo { get; set; }
        public string emailAddress { get; set; }
        public string dateOfBirth { get; set; }
        public string customIdentifier { get; set; }
        public Ssn ssn { get; set; }
        public string relyingPartyUserId { get; set; }
        public string integratorSpecificUserId { get; set; }

    }

    public class UserInfo
    {
        public string name { get; set; }
        public string surname { get; set; }
    }

    public class Ssn
    {
        public string ssn { get; set; }
        public string country { get; set; }
    }
}
