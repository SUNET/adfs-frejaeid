// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ADFSFreja.Application;
using Microsoft.IdentityServer.Web.Authentication.External;


namespace ADFSFrejaMFA
{
    internal class UsernamePasswordPresentation : IAdapterPresentationForm
    {
        private readonly ExternalAuthenticationException _ex = null;

        private readonly string _username = string.Empty;

        private readonly Dictionary<string, string> _dynamicContents = new Dictionary<string, string>()
        {
            {FrejaConstants.DynamicContentLabels.markerUserName, String.Empty},
            {FrejaConstants.DynamicContentLabels.markerOverallError, String.Empty},
            {FrejaConstants.DynamicContentLabels.markerActionUrl, String.Empty},
            {FrejaConstants.DynamicContentLabels.markerPageIntroductionTitle, String.Empty},
            {FrejaConstants.DynamicContentLabels.markerPageIntroductionText, String.Empty},
            {FrejaConstants.DynamicContentLabels.markerPageTitle, String.Empty},
            {FrejaConstants.DynamicContentLabels.markerView,String.Empty }
        };

        public UsernamePasswordPresentation(string username)
        {
            _username = username;
        }

        public UsernamePasswordPresentation(string username, ExternalAuthenticationException ex) : this(username)
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
                GetPresentationResource(FrejaConstants.ResourceNames.PageIntroductionTitle, lcid),
                [FrejaConstants.DynamicContentLabels.markerPageIntroductionText] =
                GetPresentationResource(FrejaConstants.ResourceNames.PageIntroductionText, lcid),
                [FrejaConstants.DynamicContentLabels.markerPageTitle] = GetPageTitle(lcid),
                [FrejaConstants.DynamicContentLabels.markerSubmitButton] =
                GetPresentationResource(FrejaConstants.ResourceNames.SubmitButtonLabel, lcid),
                [FrejaConstants.DynamicContentLabels.markerLoginPagePasswordLabel] = 
                GetPresentationResource(FrejaConstants.ResourceNames.Password,lcid),
                [FrejaConstants.DynamicContentLabels.markerView] =
                GetPresentationResource(FrejaConstants.ResourceNames.ViewPassword, lcid)
            };

            if (_ex != null)
            {
                dynamicContents[FrejaConstants.DynamicContentLabels.markerPageIntroductionText] = GetPresentationResource(FrejaConstants.ResourceNames.FailedLogin, lcid);
            }

            dynamicContents[FrejaConstants.DynamicContentLabels.markerLoginPageUsername] = _username;

            string authPageTemplate = ResourceHandler.GetPresentationResource(FrejaConstants.ResourceNames.AuthPageTemplate, lcid);

            return Replace(authPageTemplate, dynamicContents);
        }

        #endregion

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

