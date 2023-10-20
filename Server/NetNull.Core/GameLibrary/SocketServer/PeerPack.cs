using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace GameLibrary.SocketServer
{
    [DataContract]
    public abstract class PeerPack : IDisposable
    {
        private string? id;
        [IgnoreDataMember]
        private Socket? socket;

        [DataMember]
        public string Id
        {
            get { return id!; }
            set { id = value; }
        }

        [IgnoreDataMember]
        public Socket Socket
        {
            get { return socket!; }
            set { socket = value; }
        }

        public abstract void Close();
        public abstract void Connect();
        public abstract void Disconnect();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public abstract bool IsConnected();
        public abstract bool IsDisconnected();
    }
}
