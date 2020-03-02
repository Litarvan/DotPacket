using System;

namespace DotPacket.Registry
{
    public class UnknownPacketException : Exception
    {
        private uint _id;
        private int _state;

        public uint Id => _id;
        public int State => _state;

        public UnknownPacketException(uint id) : base($"An unknown packet with ID '{id}' was received")
        {
            _id = id;
        }

        public UnknownPacketException(int state, uint id) : base(
            $"An unknown packet with ID '{id}' was received during state '{state}'"
        )
        {
            _id = id;
            _state = state;
        }
    }
}