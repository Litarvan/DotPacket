using System;

namespace DotPacket.Registry
{
    public abstract class PacketBinding
    {
        public byte Id { get; }
        public Type Packet { get; }

        protected PacketBinding(byte id, Type packet)
        {
            Id = id;
            Packet = packet;
        }
    }
}