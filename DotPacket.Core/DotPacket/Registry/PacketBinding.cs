using System;
using System.Threading.Tasks;

namespace DotPacket.Registry
{
    public abstract class PacketBinding
    {
        public Type Packet { get; }
        public Func<Object, Task> Handler { get; set; }

        protected PacketBinding(Type packet)
        {
            Packet = packet;
        }
    }
}