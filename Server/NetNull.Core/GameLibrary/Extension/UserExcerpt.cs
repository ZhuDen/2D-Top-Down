using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GameLibrary.Extension
{
    [DataContract]
    public class UserExcerpt
    {

        [DataMember]
        public string UUID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public NetClient Client { get; set; }

    }
}
