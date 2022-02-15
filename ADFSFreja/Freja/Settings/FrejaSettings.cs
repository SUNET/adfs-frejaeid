using System.Runtime.Serialization;

namespace Freja.Settings
{

    [DataContract]
    public class FrejaSettings
    {
        [DataMember]
        public string RPCertificateThumbprint { get; set; }
        [DataMember]
        public string AuthURL { get; set; }
        [DataMember]
        public string AuthOneResultURL { get; set; }
        [DataMember]
        public string CancelRequestURL { get; set; }
        [DataMember]
        public string QRCodeURL { get; set; }
        
    }
}
