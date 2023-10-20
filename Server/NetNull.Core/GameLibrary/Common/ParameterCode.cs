using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GameLibrary.Common
{
    [DataContract]
    public enum ParameterCode:byte
    {
        Id,
        Transform,
        Parameters,
        Message,
        Name,
        Count,
        Client,
        Clients,
        RotationBody,
        RotationtHead,
        Login,
        Password,

        IconIndex,
        UUID,
        LVL,
        Exp,
        X,
        Y,
        Z,
    }
}
