using Freja.Settings;
using System.Runtime.Serialization;
 
namespace ADFSFreja.Application.Settings
{
    [DataContract(Name = "FrejaMFASettings")]
    public class FrejaMFASettings
    {
        [DataMember(Name = "FrejaSettings",Order =0)]
        public FrejaSettings FrejaConfig { get; set; }
        [DataMember(Name = "UserLookupMethod")]
        public string UserLookupMethod { get; set; }
        [DataMember(Name = "LdapSettings",Order =1)]
        public LdapSettings LdapConfig { get; set; }
        [DataMember(Name = "SqlSettings")]
        public SqlSettings SqlConfig { get; set; }
    }
}
