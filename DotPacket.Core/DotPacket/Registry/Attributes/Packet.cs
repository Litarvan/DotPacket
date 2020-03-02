using System;

namespace DotPacket.Registry.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Packet : Attribute
    {
        public byte Id { get; }
        public int State { get; }
        public NetworkSide Bound { get; }

        public Packet(byte id, int state, NetworkSide bound)
        {
            Id = id;
            State = state;
            Bound = bound;
        }
    }
}