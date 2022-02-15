using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ADFSFreja.Application.Settings
{

        [DataContract]
        public class SqlSettings
        {
        public SqlSettings()
        {
            Settings = new List<SqlSetting>();
        }
            [DataMember]
            public List<SqlSetting> Settings { get; set; }
        }


    
}
