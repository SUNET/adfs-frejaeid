using System.Runtime.Serialization;
namespace ADFSFreja.Application.Settings
{
   [DataContract]
    public class LdapSettings
    { 
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string SearchRoot { get; set; }
        [DataMember]
        public string Filter { get; set; }
        [DataMember]
        public string AttributeToRetrieve { get; set; }

    }
}
