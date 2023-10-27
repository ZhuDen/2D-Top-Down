using System;
using System.Runtime.Serialization;

namespace GameLibrary.Common
{
    [DataContract]
    public enum OperationCode:byte
    {
        Unknown,
        Disconnect,
        Message,
        Connect,
        Transform,
        Position,
        Vector3,
        Quaternion,
        Name,
        Parameters,
        MyTransform,
        SetDamage,
        GetAllClients,
        GetClientsInfo,
        Authorisation,
        Registration,

        SetTeam,
        GetTeam,
        GetInfoRoom,
        RoomConnectInfo,
    }
}