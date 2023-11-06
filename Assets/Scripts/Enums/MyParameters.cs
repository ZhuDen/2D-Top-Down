using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;


    [DataContract]
    public enum MyParameters : byte
    {
        KeyName = 101,
        Ping = 102,
        NickName = 103,
        UseSkill = 104,
        NameSkill = 105,
        Damage = 106,
        MousePos = 107,
        StatsHP = 108
    }

