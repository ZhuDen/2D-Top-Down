using GameLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Extension
{
    public class TeamMember
    {
        // сокет
        public NetClient netClient;

        public int Team;
        public string Name;
        public int Kills;
        public int Money;

        public int Dmg = 0;
        //public skills[] прописать скилы

    }
}
