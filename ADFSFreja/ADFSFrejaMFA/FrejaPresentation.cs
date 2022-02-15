using ADFSFreja.Application;
using ADFSFreja.Application.Utils;
using Microsoft.IdentityServer.Web.Authentication.External;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ADFSFrejaMFA
{
    internal class FrejaPresentation : IAdapterPresentationForm
    {
        private readonly ExternalAuthenticationException _ex = null;
        private readonly string _civicNumber;
        private readonly Dictionary<string, string> _dynamicContents = new Dictionary<string, string>()
        {
            {FrejaConstants.DynamicContentLabels.markerPageFrejaBanner, String.Empty},
            {FrejaConstants.DynamicContentLabels.markerOverallError, String.Empty},
            {FrejaConstants.DynamicContentLabels.markerActionUrl, String.Empty},
            {FrejaConstants.DynamicContentLabels.markerPageIntroductionTitle, String.Empty},
            {FrejaConstants.DynamicContentLabels.markerPageIntroductionText, String.Empty},
            {FrejaConstants.DynamicContentLabels.markerPageTitle, String.Empty},
            {FrejaConstants.DynamicContentLabels.markerView,String.Empty }
        };

        public FrejaPresentation(string civicNumber)
        {
            _civicNumber = civicNumber;
        }
        public FrejaPresentation(ExternalAuthenticationException ex)
        {
            _ex = ex;
            
        }

        /// <summary>
        /// Replace template markers with explicitly given replacements.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="replacements"></param>
        /// <returns></returns>
        private static string Replace(string input, Dictionary<string, string> replacements)
        {
            if (string.IsNullOrEmpty(input) || null == replacements)
            {
                return input;
            }

            // Use StringBuiler and allocate buffer 3 times larger
            StringBuilder sb = new StringBuilder(input, input.Length * 3);
            foreach (string key in replacements.Keys)
            {
                sb.Replace(key, replacements[key]);
            }
            return sb.ToString();
        }
        #region IAdapterPresentationForm Members
        public string GetFormHtml(int lcid)
        {
            var dynamicContents = new Dictionary<string, string>(_dynamicContents)
            {
                [FrejaConstants.DynamicContentLabels.markerPageIntroductionTitle] =
                GetPresentationResource(FrejaConstants.ResourceNames.PageFrejaIntroductionTitle, lcid),
                [FrejaConstants.DynamicContentLabels.markerPageIntroductionText] =
                GetPresentationResource(FrejaConstants.ResourceNames.PageIntroductionText, lcid),
                [FrejaConstants.DynamicContentLabels.markerPageTitle] = GetPageTitle(lcid),
                [FrejaConstants.DynamicContentLabels.markerSubmitButton] =
                GetPresentationResource(FrejaConstants.ResourceNames.SubmitButtonLabel, lcid),
                [FrejaConstants.DynamicContentLabels.markerLoginPagePasswordLabel] = 
                GetPresentationResource(FrejaConstants.ResourceNames.CivicNumber,lcid) ,
                [FrejaConstants.DynamicContentLabels.markerView] =
                GetPresentationResource(FrejaConstants.ResourceNames.ViewFreja, lcid),
                [FrejaConstants.DynamicContentLabels.markerPageFrejaBanner] = 
                GetPresentationResource(FrejaConstants.ResourceNames.PageFrejaBanner,lcid),
                [FrejaConstants.DynamicContentLabels.markerPageFrejaCivicNumberInPut]=_civicNumber
            };
            if (_ex != null)
            {
                //_ex.Message
                Log.WriteEntry("Freja presentationform error: " + _ex.Message, EventLogEntryType.Error, 338);
                dynamicContents[FrejaConstants.DynamicContentLabels.markerPageIntroductionText] = GetPresentationResource(_ex.Message, lcid);
                if (_ex.Context != null)
                {
                   dynamicContents[FrejaConstants.DynamicContentLabels.markerPageFrejaCivicNumberInPut]=_ex.Context.Data["CivicNumber"].ToString();
                }
            }
            string authPageTemplate = ResourceHandler.GetPresentationResource(FrejaConstants.ResourceNames.AuthPageFrejaTemplate, lcid);
            Log.WriteEntry("Freja presentationform med pnr: " + _civicNumber, EventLogEntryType.Information, 338);
            return Replace(authPageTemplate, dynamicContents);
        }
        #endregion IAdapterPresentationForm Members

        #region IAdapterPresentationIndirect Members
        public string GetFormPreRenderHtml(int lcid)
        {
            return null;
        }

        public string GetPageTitle(int lcid)
        {
            return GetPresentationResource(FrejaConstants.ResourceNames.PageTitle, lcid);
        }

        #endregion
        protected string GetPresentationResource(string resourceName, int lcid)
        {
            return ResourceHandler.GetResource(resourceName, lcid);
        }
    }
}
