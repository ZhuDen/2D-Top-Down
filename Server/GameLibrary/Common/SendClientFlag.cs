using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GameLibrary.Common
{
    [DataContract]
    public enum SendClientFlag : byte
    {
        All,
        Me,
        FullRoom,
        MyTeam,
        Personal,
    }
}
