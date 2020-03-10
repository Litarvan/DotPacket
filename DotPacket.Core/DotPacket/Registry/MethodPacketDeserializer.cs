using System;
using System.Reflection;
using System.Threading.Tasks;
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

        public override async Task<object> Deserialize(byte[] data)
        {
            var packet = _constructor.Invoke(new object[0]);
            
            var stream = new StreamReader(new ByteArrayInputStream(data), DotPacket.DefaultBufferSize);
            var result = _method.Invoke(packet, new object[] {stream});

            if (result is Task task)
            {
                await task;
            }

            return packet;
        }
    }
}