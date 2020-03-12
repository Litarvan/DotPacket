using System;
using System.Reflection;

using DotPacket.IO;
using DotPacket.Serialization;

namespace DotPacket.Registry
{
    public class MethodPacketDeserializer : PacketDeserializer
    {
        private MethodInfo _method;
        private ConstructorInfo _constructor;
        
        public MethodPacketDeserializer(MethodInfo method, Type type) : base(type)
        {
            _method = method;
        }

        public override void Prepare()
        {
            foreach (var cons in Type.GetConstructors())
            {
                if (cons.GetParameters().Length == 0)
                {
                    _constructor = cons;
                }
            }

            if (_constructor == null)
            {
                throw new InvalidRegistryOperationException(
                    $"Packet '{Type.Name}' must have an empty constructor to be used with a method deserializer"
                );
            }
        }

        public override object Deserialize(byte[] data)
        {
            var packet = _constructor.Invoke(new object[0]);
            var stream = new StreamReader(new ByteArrayInputStream(data), DotPacket.DefaultBufferSize);
            
            _method.Invoke(packet, new object[] {stream});
            
            return packet;
        }
    }
}