using ADFSFreja.Application.Interfaces;
using ADFSFreja.Application.Settings;
using ADFSFreja.Application.Utils;
using System;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;

namespace ADFSFreja.Application.Services
{
    public class PersonServiceLdap : IPersonService
    {
        private readonly LdapSettings _ldapSettings;
        private DirectoryEntry _entry;
        public PersonServiceLdap(LdapSettings settings)
        {
            _ldapSettings = settings;
        }
        public string GetCivicNumber(string uid)
        {
            string civicNumber = "";
            var result = Search(GetUidWithoutDomain(uid));
            if (result != null)
            {
                civicNumber = result.Properties[_ldapSettings.AttributeToRetrieve][0].ToString();
            }
            return civicNumber;
        }

        private SearchResult Search(string uid)
        {
            SearchResult result = null;
            _entry = new DirectoryEntry(GetRootPath(), _ldapSettings.UserName, _ldapSettings.Password, AuthenticationTypes.SecureSocketsLayer);
            DirectorySearcher mySearcher = new DirectorySearcher(_entry, string.Format(_ldapSettings.Filter, uid), new string[] { _ldapSettings.AttributeToRetrieve }, SearchScope.Subtree);
            try
            {
                result = mySearcher.FindOne();
            }
            catch (Exception ex)
            {
                Log.WriteEntry("Error searching for CivicNumber: " + ex.Message, EventLogEntryType.Error, 335);
            }

            return result;
        }
        private string GetUidWithoutDomain(string uid)
        {
            if (uid.Contains("@"))
            {
                return uid.Substring(0, uid.IndexOf("@"));
            }
            return uid;
        }
        private string GetRootPath()
        {
            var path = "";
            if (!_ldapSettings.SearchRoot.ToUpper().Contains("LDAP://"))
            {
                path = "LDAP://" + _ldapSettings.SearchRoot;
            }
            return path;
        }
    }
}
