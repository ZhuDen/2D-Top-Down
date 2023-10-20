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
    [KnownType(typeof(NetTransform))]
    [KnownType(typeof(System.Byte[]))]
    public class DataPacket
    {
        [DataMember]
        public GameLibrary.Common.OperationCode operationCode { get; set; }
        [DataMember]
        public System.Collections.Generic.Dictionary<GameLibrary.Common.ParameterCode, object> Data { get; set; }

        public DataPacket(GameLibrary.Common.OperationCode sendParameters, System.Collections.Generic.Dictionary<GameLibrary.Common.ParameterCode, object> data)
        {
            Data = data;
            operationCode = sendParameters;
        }

    }
}

