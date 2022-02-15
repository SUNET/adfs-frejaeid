using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ADFSFreja.Application
{
    public static class FrejaConstants
    {

        public const string FrejaMFA = "http://freja.com/mfa";
        public const string UsernamePasswordMfa = "http://schemas.microsoft.com/ws/2012/12/authmethod/usernamepasswordMFA";
        public const string UsernamePasswordRefeds = "https://refeds.org/profile/mfa";
        public const string AuthenticationMethodClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod";
        public const string WindowsAccountNameClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsaccountname";
        public const string UpnClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn";


        public static class AuthContextKeys
        {
            public const string SessionId = "sessionid";
            public const string Identity = "id";
            public const string CivicNumber = "nin";
        }

        public static class DynamicContentLabels
        {
            public const string markerUserName = "%LoginPageUserName%";
            public const string markerOverallError = "%PageErrorOverall%";
            public const string markerActionUrl = "%PageActionUrl%";
            public const string markerPageIntroductionTitle = "%PageIntroductionTitle%";
            public const string markerPageIntroductionText = "%PageIntroductionText%";
            public const string markerPageTitle = "%PageTitle%";
            public const string markerSubmitButton = "%PageSubmitButtonLabel%";
            public const string markerChoiceSuccess = "%ChoiceSuccess%";
            public const string markerChoiceFail = "%ChoiceFail%";
            public const string markerUserChoice = "%UserChoice%";
            public const string markerLoginPageUsername = "%Username%";
            public const string markerLoginPagePasswordLabel = "%LoginPagePasswordLabel%";

            //Freja
            public const string markerView = "%View%";
            public const string markerPageFrejaCivicNumberInPut = "%CivicNumberInput%";
            public const string markerPageFrejaIntroductionTitle = "%PageFrejaIntroductionTitle%";
            public const string markerPageFrejaIntroductionText = "%PageFrejaIntroductionText%";
            public const string markerPageFrejaBanner = "%PageFrejaBanner%";
            public const string markerPageFrejaInstruction = "%PageFrejaInstruction%";
        }

        public static class ResourceNames
        {
            public const string AdminFriendlyName = "AdminFriendlyName";
            public const string Description = "Description";
            public const string FriendlyName = "FriendlyName";
            public const string PageIntroductionTitle = "PageIntroductionTitle";
            public const string PageIntroductionText = "PageIntroductionText";
            public const string AuthPageTemplate = "AuthPage";
            public const string ErrorInvalidSessionId = "ErrorInvalidSessionId";
            public const string ErrorInvalidContext = "ErrorInvalidContext";
            public const string ErrorNoUserIdentity = "ErrorNoUserIdentity";
            public const string ErrorNoAnswerProvided = "ErrorNoAnswerProvided";
            public const string ErrorFailSelected = "ErrorFailSelected";
            public const string ChoiceSuccess = "ChoiceSuccess";
            public const string ChoiceFail = "ChoiceFail";
            public const string UserChoice = "UserChoice";
            public const string FailedLogin = "FailedLogin";
            public const string PageFrejaIntroductionTitle = "PageFrejaIntroductionTitle";
            public const string PageFrejaIntroductionText = "PageFrejaIntroductionText";
            public const string SubmitButtonLabel = "SubmitButtonLabel";
            public const string CivicNumber = "CivicNumber";
            public const string PageFrejaBanner = "PageFrejaBanner";
            public const string AuthPageFrejaTemplate = "AuthPageFreja";
            public const string AuthPageFrejaInstruction = "PageFrejaInstruction";
            public const string PageTitle = "PageTitle";
            public const string ViewPassword = "ViewPassword";
            public const string ViewFreja = "ViewFreja";
            public const string Password = "Password";

        }

        public static class PropertyNames
        {
            public const string UserSelection = "UserSelection";
            public const string AuthenticationMethod = "AuthMethod";
            public const string Password = "PasswordInput";
            public const string Username = "Username";
            //Freja
            public const string View = "View";
            public const string CivicNumber = "CivicNumberInput";
        }
        public static class Lcid
        {
            public const int En = 0x9;  
            public const int Sv = 0x1D; 

        }
    }
}
