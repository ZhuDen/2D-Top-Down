using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;


    [DataContract]
    public enum MyParameters : byte
    {
        KeyName,
        Ping,
        NickName,
        UseSkill
    }

