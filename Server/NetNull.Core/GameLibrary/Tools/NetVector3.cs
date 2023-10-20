using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Tools
{
    [DataContract]
    public class NetVector3
    {
        private const float MaxValue = 9999.9999f;

        [DataMember]
        private float _x;
        [DataMember]
        private float _y;
        [DataMember]
        private float _z;
        [DataMember]
        public float x
        {
            get { return _x; }
            set { _x = Math.Clamp(value, -MaxValue, MaxValue); }
        }
        [DataMember]
        public float y
        {
            get { return _y; }
            set { _y = Math.Clamp(value, -MaxValue, MaxValue); }
        }
        [DataMember]
        public float z
        {
            get { return _z; }
            set { _z = Math.Clamp(value, -MaxValue, MaxValue); }
        }

        public float magnitude => (float)Math.Sqrt(_x * _x + _y * _y + _z * _z);

        public float sqrMagnitude => _x * _x + _y * _y + _z * _z;

        public static NetVector3 zero => new NetVector3(0, 0, 0);

        public static NetVector3 one => new NetVector3(1, 1, 1);

        public static NetVector3 up => new NetVector3(0, 1, 0);

        public static NetVector3 down => new NetVector3(0, -1, 0);

        public static NetVector3 left => new NetVector3(-1, 0, 0);

        public static NetVector3 right => new NetVector3(1, 0, 0);

        public static NetVector3 forward => new NetVector3(0, 0, 1);

        public static NetVector3 back => new NetVector3(0, 0, -1);

        public NetVector3(float x = 0, float y = 0, float z = 0)
        {
            _x = Math.Clamp(x, -MaxValue, MaxValue);
            _y = Math.Clamp(y, -MaxValue, MaxValue);
            _z = Math.Clamp(z, -MaxValue, MaxValue);
        }

        public NetVector3 normalized
        {
            get
            {
                float mag = magnitude;
                return mag > 0 ? new NetVector3(_x / mag, _y / mag, _z / mag) : zero;
            }
        }

    }
}
