using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GameLibrary.Tools;

namespace GameLibrary.Extension
{
    /// <summary>
    /// Добавление полей делает их доступными на сервере и на клиенте для синхронизации
    /// </summary>
    /// 
    [DataContract]
    public class PlayerParameters
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
}
