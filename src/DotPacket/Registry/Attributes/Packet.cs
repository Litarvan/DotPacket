using System;

namespace DotPacket.Registry.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Packet : Attribute
    {
        private uint _id;
        private int _state;
        private NetworkSide _bound;

        public uint Id => _id;
        public int State => _state;
        public NetworkSide Bound => _bound;

        public Packet(uint id, int state, NetworkSide bound)
        {
            _id = id;
            _state = state;
            _bound = bound;
        }
    }
}