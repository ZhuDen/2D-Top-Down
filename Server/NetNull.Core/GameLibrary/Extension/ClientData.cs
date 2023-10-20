using GameLibrary.Tools;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GameLibrary.Extension
{
    [DataContract]
    [KnownType(typeof(NetTransform))]
    [KnownType(typeof(System.Byte[]))]
    public class ClientData
    {
        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Icon { get; set; }

        [DataMember]
        public string Lvl { get; set; }

        [DataMember]
        public string Exp { get; set; }

        [DataMember]
        public NetTransform NetTransform { get; set; }

        [DataMember]
        public PlayerParameters PlayerParameters { get; set; }

        [DataMember]
        public List<UserExcerpt> Party { get; set; }

        [DataMember]
        public List<UserExcerpt> Friends { get; set; }

    }
}
