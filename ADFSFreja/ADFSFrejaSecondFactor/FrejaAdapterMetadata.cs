using ADFSFreja.Application;
using Microsoft.IdentityServer.Web.Authentication.External;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADFSFrejaSecondFactor
{
    public class FrejaAdapterMetadata : IAuthenticationAdapterMetadata
    {
        private readonly Dictionary<int, string> _descriptions = new Dictionary<int, string>();
        private readonly Dictionary<int, string> _friendlyNames = new Dictionary<int, string>();
        private readonly int[] _supportedLcids = new[] { FrejaConstants.Lcid.En, FrejaConstants.Lcid.Sv };
        public FrejaAdapterMetadata()
        {
            for (int index = 0; index < _supportedLcids.Length; index++)
            {
                int lcid = _supportedLcids[index];
                //_descriptions.Add(lcid, GetMetadataResource(Constants.ResourceNames.Description, lcid));
                //_friendlyNames.Add(lcid, GetMetadataResource(Constants.ResourceNames.FriendlyName, lcid));
                _descriptions.Add(lcid, "Freja eID+");
                _friendlyNames.Add(lcid, "Freja eID+");
                
            }
        }
        protected string GetMetadataResource(string resourceName, int lcid)
        {
            return ResourceHandler.GetResource(resourceName, lcid);
        }
        public string[] AuthenticationMethods{
            get { return new[] { FrejaConstants.FrejaMFA }; }
        }

        public string[] IdentityClaims
        {
            get { return new[] { FrejaConstants.UpnClaimType }; }
        }

        public string AdminName
        {
            get { return "Freja"; }
            //get { return GetMetadataResource(Constants.ResourceNames.AdminFriendlyName, CultureInfo.CurrentUICulture.LCID); }
            //get { return GetMetadataResource(Constants.ResourceNames.AdminFriendlyName, CultureInfo.CurrentUICulture.LCID); }
        }

        public int[] AvailableLcids
        {
            get { return _supportedLcids; }
        }

        public Dictionary<int, string> Descriptions
        {
            get { return _descriptions; }
        }

        public Dictionary<int, string> FriendlyNames
        {
            get { return _friendlyNames; }
        }

        public bool RequiresIdentity
        {
            get { return true; }
        }
    }
}
