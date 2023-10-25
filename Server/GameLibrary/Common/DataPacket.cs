using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using GameLibrary.Tools;
using GameLibrary.Common;
using GameLibrary.Extension;
using GameLibrary.SocketServer;

namespace GameLibrary.Common
{

    [DataContract]
    [KnownType(typeof(ClientData))]
    [KnownType(typeof(TeamMember))]
    [KnownType(typeof(NetClient))]
    [KnownType(typeof(NetTransform))]
    [KnownType(typeof(Room))]
    [KnownType(typeof(List<TeamMember>))]
    [KnownType(typeof(System.Byte[]))]
    public class DataPacket
    {
        [DataMember]
        public GameLibrary.Common.OperationCode operationCode { get; set; }
        [DataMember]
        public System.Collections.Generic.Dictionary<GameLibrary.Common.ParameterCode, object> Data { get; set; }
        [DataMember]
        public GameLibrary.Common.SendClientFlag Flag { get; set; }

        public DataPacket(GameLibrary.Common.OperationCode sendParameters, System.Collections.Generic.Dictionary<GameLibrary.Common.ParameterCode, object> data, SendClientFlag flag = SendClientFlag.Me)
        {
            Data = data;
            operationCode = sendParameters;
            Flag = flag;
        }

    }
}

