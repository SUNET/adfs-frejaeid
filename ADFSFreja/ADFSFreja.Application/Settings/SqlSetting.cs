using System.Runtime.Serialization;

namespace ADFSFreja.Application.Settings
{
    public class SqlSetting
    {
        [DataMember]
        public string ConnectionString { get; set; }
        [DataMember]
        public string Command { get; set; }
    }
}
