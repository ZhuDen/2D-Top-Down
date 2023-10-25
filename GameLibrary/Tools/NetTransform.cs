using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GameLibrary.Tools
{
    [DataContract]
    public class NetTransform
    {
        [DataMember]
        public NetVector3 Position { get; set; }

        [DataMember]
        public NetVector3 Rotation { get; set; }

        [DataMember]
        public NetVector3 Scale { get; set; }

        public float DistanceTo(NetTransform otherTransform)
        {
            float dx = Position.x - otherTransform.Position.x;
            float dy = Position.y - otherTransform.Position.y;
            float dz = Position.z - otherTransform.Position.z;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public float DistanceTo(ref NetTransform otherTransform)
        {
            float dx = Position.x - otherTransform.Position.x;
            float dy = Position.y - otherTransform.Position.y;
            float dz = Position.z - otherTransform.Position.z;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public float DistanceTo(NetVector3 otherPosition)
        {
            float dx = Position.x - otherPosition.x;
            float dy = Position.y - otherPosition.y;
            float dz = Position.z - otherPosition.z;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
    }
}
