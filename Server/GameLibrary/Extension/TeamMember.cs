using GameLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Extension
{
    [DataContract]
    public class TeamMember
    {
        // сокет
        [DataMember]
        public NetClient netClient;
        [DataMember]
        public int Team;
        [DataMember]
        public string Name;
        [DataMember]
        public int Kills;
        [DataMember]
        public int Money;
        [DataMember]
        public int Dmg = 0;
        //public skills[] прописать скилы

    }
}
