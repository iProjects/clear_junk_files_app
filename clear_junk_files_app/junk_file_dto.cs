using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace clear_junk_files_app
{
    [DataContract]
    public class junk_file_dto
    {
        [DataMember]
        public string file_id { get; set; }
        [DataMember]
        public string full_name { get; set; }
        [DataMember]
        public string size { get; set; }
        [DataMember]
        public string extension { get; set; }
        [DataMember]
        public string created_date { get; set; }

    }
}
