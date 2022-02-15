using ADFSFreja.Application.Model;
using ADFSFreja.Application.Utils;
using Microsoft.IdentityServer.Web.Authentication.External;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace ADFSFreja.Application
{
    public static class ResourceHandler
    {
        public static string GetResource(string resourceName, int lcid)
        {
            Log.WriteEntry("Get resource string: " + resourceName + " with lcid: " + lcid, EventLogEntryType.Information, 335);
            if (lcid != FrejaConstants.Lcid.En && lcid != FrejaConstants.Lcid.Sv)
            {
                lcid = FrejaConstants.Lcid.Sv;
            }
            LangText text = (from tt in texts.Where(t => t.Key == resourceName && t.Lcid == lcid) select tt).SingleOrDefault();
            if (text == null)
            {
                throw new ArgumentNullException();

            }
            return text.Value;
            //if (String.IsNullOrEmpty(resourceName))
            //{
            //    throw new ArgumentNullException("resourceName");
            //}

            //return StringResources.ResourceManager.GetString(resourceName, new CultureInfo(lcid));
        }
        public static string GetPresentationResource(string resourceName, int lcid)
        {
            if (String.IsNullOrEmpty(resourceName))
            {
                throw new ArgumentNullException("resourceName");
            }
            return PresentationResources.ResourceManager.GetString(resourceName, new CultureInfo(lcid));
        }
        private static List<LangText> texts =
            new List<LangText>() {
        //en
        new LangText(){Key="AdminFriendlyName",Lcid=9,Value="Freja"},
        new LangText(){Key="AuthenticationFailed",Lcid=9,Value="AuthenticationFailed"},
        new LangText(){Key="ChoiceFail",Lcid=9,Value="ChoiceFail"},
        new LangText(){Key="ChoiceSuccess",Lcid=9,Value="ChoiceSuccess"},
        new LangText(){Key="CivicNumber",Lcid=9,Value="Civic number"},
        new LangText(){Key="Description",Lcid=9,Value="Freja"},
        new LangText(){Key="ErrorFailSelected",Lcid=9,Value="ErrorFailSelected"},
        new LangText(){Key="ErrorInvalidContext",Lcid=9,Value="ErrorInvalidContext"},
        new LangText(){Key="ErrorInvalidSessionId",Lcid=9,Value="ErrorInvalidSessionId"},
        new LangText(){Key="ErrorMissingCivicNo",Lcid=9,Value="No civicnumber found!"},
        new LangText(){Key="ErrorNoAnswerProvided",Lcid=9,Value="ErrorNoAnswerProvided"},
        new LangText(){Key="ErrorNoUserIdentity",Lcid=9,Value="ErrorNoUserIdentity"},
        new LangText(){Key="FailedLogin",Lcid=9,Value="Incorrect password. Type the correct password, and try again."},
        new LangText(){Key="FrejaError_NoCivicNumber",Lcid=9,Value="Cant proceed with Freja, no civicnumber found."},
        new LangText(){Key="FrejaError_NoMatch",Lcid=9,Value="Civic number does not match in Freja."},
        new LangText(){Key="FrejaResponse_CANCELED",Lcid=9,Value="Freja login canceled by user"},
        new LangText(){Key="FrejaResponse_EXPIRED",Lcid=9,Value="Freja login request has expired"},
        new LangText(){Key="FrejaResponse_REJECTED",Lcid=9,Value="Freja rejected login attempt"},
        new LangText(){Key="FrejaResponse_RP_CANCELED",Lcid=9,Value="Freja login request canceled by relying party."},
        new LangText(){Key="FriendlyName",Lcid=9,Value="Freja eID+"},
        new LangText(){Key="PageFrejaBanner",Lcid=9,Value="Freja eID+"},
        new LangText(){Key="PageFrejaInstruction",Lcid=9,Value="Open the Freja eID app on your phone and follow the instructions to sign in"},
        new LangText(){Key="PageFrejaIntroductionTitle",Lcid=9,Value="Enter civic number"},
        new LangText(){Key="PageFrejaIntroductionText",Lcid=9,Value="Complete the login by verifying your personal identity with Freja eID+. Your Swedish personal identity number is shown below"},
        new LangText(){Key="PageIntroductionTitle",Lcid=9,Value="Enter your password"},
        new LangText(){Key="PageTitle",Lcid=9,Value="PageTitle"},
        new LangText(){Key="Password",Lcid=9,Value="Password"},
        new LangText(){Key="SubmitButtonLabel",Lcid=9,Value="Sign in"},
        new LangText(){Key="UserChoice",Lcid=9,Value="UserChoice"},
        new LangText(){Key="ViewFreja",Lcid=9,Value="Freja"},
        new LangText(){Key="ViewPassword",Lcid=9,Value="Password"},
        //sv
        new LangText(){Key="AdminFriendlyName",Lcid=29,Value="Freja"},
        new LangText(){Key="AuthenticationFailed",Lcid=29,Value="Inloggning misslyckades"},
        new LangText(){Key="ChoiceFail",Lcid=29,Value="ChoiceFail"},
        new LangText(){Key="ChoiceSuccess",Lcid=29,Value="ChoiceSuccess"},
        new LangText(){Key="CivicNumber",Lcid=29,Value="Personnummer"},
        new LangText(){Key="Description",Lcid=29,Value="Freja"},
        new LangText(){Key="ErrorFailSelected",Lcid=29,Value="ErrorFailSelected"},
        new LangText(){Key="ErrorInvalidContext",Lcid=29,Value="ErrorInvalidContext"},
        new LangText(){Key="ErrorInvalidSessionId",Lcid=29,Value="ErrorInvalidSessionId"},
        new LangText(){Key="ErrorMissingCivicNo",Lcid=29,Value="Inget personnummer kunde hittas!"},
        new LangText(){Key="ErrorNoAnswerProvided",Lcid=29,Value="ErrorNoAnswerProvided"},
        new LangText(){Key="ErrorNoUserIdentity",Lcid=29,Value="ErrorNoUserIdentity"},
        new LangText(){Key="FailedLogin",Lcid=29,Value="Felaktigt lösenord. Skriv in lösenord och pröva igen."},
        new LangText(){Key="FrejaError_NoCivicNumber",Lcid=29,Value="Kan inte gå vidare med Freja inloggning, kan inte hitta något personnummer."},
        new LangText(){Key="FrejaError_NoMatch",Lcid=29,Value="Ingen matchning på personnummer i Freja."},
        new LangText(){Key="FrejaResponse_CANCELED",Lcid=29,Value="Inloggning mot freja avbröts av användaren."},
        new LangText(){Key="FrejaResponse_EXPIRED",Lcid=29,Value="Inloggningsbegäran mot Freja har upphört att gälla."},
        new LangText(){Key="FrejaResponse_REJECTED",Lcid=29,Value="Freja avvisade inloggningsförsöket"},
        new LangText(){Key="FrejaResponse_RP_CANCELED",Lcid=29,Value="Inloggningsbegäran mot Freja har avbrutits av tjänsten."},
        new LangText(){Key="FriendlyName",Lcid=29,Value="Freja eID+"},
        new LangText(){Key="PageFrejaBanner",Lcid=29,Value="Freja eID+"},
        new LangText(){Key="PageFrejaInstruction",Lcid=29,Value="Öppna appen Freja eID i din mobiltelefon och följ instruktionerna för att logga in"},
        new LangText(){Key="PageFrejaIntroductionTitle",Lcid=29,Value="Skriv in personnummer"},
        new LangText(){Key="PageFrejaIntroductionText",Lcid=29,Value="Slutför inloggningen genom att verifiera din personliga identitet med Freja eID+. Ditt personnummer visas nedan."},
        new LangText(){Key="PageIntroductionTitle",Lcid=29,Value="Skriv in lösenord"},
        new LangText(){Key="PageTitle",Lcid=29,Value="PageTitle"},
        new LangText(){Key="Password",Lcid=29,Value="Lösenord"},
        new LangText(){Key="SubmitButtonLabel",Lcid=29,Value="Logga in"},
        new LangText(){Key="UserChoice",Lcid=29,Value="UserChoice"},
        new LangText(){Key="ViewFreja",Lcid=29,Value="Freja"},
        new LangText(){Key="ViewPassword",Lcid=29,Value="Password"},

        };
    }
}


