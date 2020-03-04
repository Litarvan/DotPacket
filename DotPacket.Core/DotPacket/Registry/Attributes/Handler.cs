using System;

namespace DotPacket.Registry.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class Handler : Attribute
    {
        public Type Packet;

        public Handler(Type packet)
        {
            Packet = packet;
        }
    }
}