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
    [KnownType(typeof(TransportHeader))]
    [KnownType(typeof(System.Byte[]))]
    public class DataPacket
    {
        [DataMember]
        public byte operationCode { get; set; }
        [DataMember]
        public System.Collections.Generic.Dictionary<byte, object> Data { get; set; }
        [DataMember]
        public GameLibrary.Common.SendClientFlag Flag { get; set; }
        [DataMember]
        public bool Rpc { get; set; }
        [DataMember]
        public TransportHeader Header = null;

        /// <summary>
        /// Пакет пересылки между клиентом и сервером
        /// </summary>
        /// <param name="sendParameters">Ключ отправки сообщения</param>
        /// <param name="data">Контент пакета</param>
        /// <param name="flag">Флаг получаетя пакета</param>
        /// <param name="rpc">Если установлен в True, содержимое пакета не будет отслеживаться сервером</param>
        /// <returns>Метода поддерживаетя, но устарел, используйте TransportHeader для конфигурирования параметров пакета</returns>
        public DataPacket(byte sendParameters, System.Collections.Generic.Dictionary<byte, object> data, SendClientFlag flag = SendClientFlag.Me, bool rpc = false)
        {
            Data = data;
            operationCode = sendParameters;
            Flag = flag;
            Rpc = rpc;
        }

        /// <summary>
        /// Пакет пересылки между клиентом и сервером
        /// </summary>
        /// <param name="header">Заголовок отправляемог опакета, хранит параметры пакета</param>
        /// <param name="data">Контент пакета</param>
        public DataPacket(TransportHeader header, System.Collections.Generic.Dictionary<byte, object> data)
        {
            Header = header;
            Data = data;
        }

    }

}

