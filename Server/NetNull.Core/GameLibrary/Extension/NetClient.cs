using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GameLibrary.Extension;
using GameLibrary.Tools;
using GameLibrary.SocketServer;
using System.Runtime.Serialization;

namespace GameLibrary.Extension
{
    [DataContract]
    //[KnownType(typeof(ClientData))]
    public class NetClient : PeerPack
    {
        [DataMember]
        public DateTime LastActivityTime { get; set; }
        [DataMember]
        public GameLibrary.Extension.ClientData Data { get; set; }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override void Connect()
        {
            throw new NotImplementedException();
        }

        public override void Disconnect()
        {
            throw new NotImplementedException();
        }

        public override bool IsConnected()
        {
            throw new NotImplementedException();
        }

        public override bool IsDisconnected()
        {
            throw new NotImplementedException();
        }
    }
}
