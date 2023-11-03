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
    [KnownType(typeof(System.Byte[]))]
    public class TransportHeader:IDisposable
    {
        [DataMember]
        public byte OperationCode { get; set; }
        [DataMember]
        public SendClientFlag Flag = SendClientFlag.Me;
        [DataMember]
        public bool Rpc = false;
        //[DataMember]
        //public string ?UUID = null;

        /// <summary>
        /// Заголовок транспортного пакета
        /// </summary>
        /// <param name="operationCode">Ключ отправки сообщения</param>
        /// <param name="flag">Флаг получателя</param>
        /// <param name="rpc">Если установлен в True, содержимое пакета не будет отслеживаться сервером</param>
        public TransportHeader(object operationCode, SendClientFlag flag = SendClientFlag.Me, bool rpc = false)
        {

            OperationCode = (byte)operationCode;
            Flag = flag;
            Rpc = rpc;

        }
        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
