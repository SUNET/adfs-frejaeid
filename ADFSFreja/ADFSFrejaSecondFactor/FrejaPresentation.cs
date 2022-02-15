using ADFSFreja.Application;
using ADFSFreja.Application.Utils;
using Microsoft.IdentityServer.Web.Authentication.External;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ADFSFrejaSecondFactor
{
    public class FrejaPresentation : IAdapterPresentationForm
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
            {FrejaConstants.DynamicContentLabels.markerView,String.Empty } ,
            {FrejaConstants.DynamicContentLabels.markerPageFrejaInstruction,String.Empty} 
        };
        public FrejaPresentation(string civicNumber)
        {
            _civicNumber = civicNumber;
        }
        public FrejaPresentation(string civicnumber,ExternalAuthenticationException ex)
        {
            _civicNumber = civicnumber;
            _ex = ex;

        }
        public string GetFormHtml(int lcid)
        {
            var dynamicContents = new Dictionary<string, string>(_dynamicContents)
            {
                [FrejaConstants.DynamicContentLabels.markerPageIntroductionTitle] =
                GetResource(FrejaConstants.ResourceNames.PageFrejaIntroductionTitle, lcid),
                [FrejaConstants.DynamicContentLabels.markerPageTitle] = GetPageTitle(lcid),
                [FrejaConstants.DynamicContentLabels.markerSubmitButton] =
                GetResource(FrejaConstants.ResourceNames.SubmitButtonLabel, lcid),
                [FrejaConstants.DynamicContentLabels.markerLoginPagePasswordLabel] =
                GetResource(FrejaConstants.ResourceNames.CivicNumber, lcid),
                [FrejaConstants.DynamicContentLabels.markerPageFrejaBanner] =
                GetResource(FrejaConstants.ResourceNames.PageFrejaBanner, lcid),
                [FrejaConstants.DynamicContentLabels.markerPageFrejaCivicNumberInPut] =MaskCivicnumber( _civicNumber),
                [FrejaConstants.DynamicContentLabels.markerPageFrejaIntroductionText] =
                GetResource(FrejaConstants.ResourceNames.PageFrejaIntroductionText, lcid),
                [FrejaConstants.DynamicContentLabels.markerPageFrejaInstruction] =
                GetResource(FrejaConstants.ResourceNames.AuthPageFrejaInstruction, lcid)
            };
            if (_ex != null)
            {
                //_ex.Message
                Log.WriteEntry("Freja presentationform error: " + _ex.Message, EventLogEntryType.Error, 338);
                dynamicContents[FrejaConstants.DynamicContentLabels.markerPageIntroductionText] = GetResource(_ex.Message, lcid);
                if (_ex.Context != null)
                {
                    //dynamicContents[Constants.DynamicContentLabels.markerPageFrejaCivicNumberInPut] = _ex.Context.Data["CivicNumber"].ToString();
                }
            }
            string authPageTemplate = ResourceHandler.GetPresentationResource(FrejaConstants.ResourceNames.AuthPageFrejaTemplate, lcid);
            
            return Replace(authPageTemplate, dynamicContents);
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
                //Log.WriteEntry("Freja presentationform replace key:  " + key, EventLogEntryType.Information, 335);
                sb.Replace(key, replacements[key]);
            }
            return sb.ToString();
        }

        #region IAdapterPresentationIndirect Members
        public string GetFormPreRenderHtml(int lcid)
        {
            return null;
        }

        public string GetPageTitle(int lcid)
        {
            return GetResource(FrejaConstants.ResourceNames.PageTitle, lcid);
        }

        #endregion
        protected string GetResource(string resourceName, int lcid)
        {
            return ResourceHandler.GetResource(resourceName, lcid);
        }
        private string MaskCivicnumber(string civicNumber)
        {
            return civicNumber.Substring(0, 8)+ "XXXX";
        }
    }
}
