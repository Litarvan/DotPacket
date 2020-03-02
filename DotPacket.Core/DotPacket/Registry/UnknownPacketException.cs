using System;

namespace DotPacket.Registry
{
    public class UnknownPacketException : Exception
    {
        public byte Id { get; }
        public int State { get; }

        public UnknownPacketException(byte id) : base($"An unknown packet with ID '{id}' was received")
        {
            Id = id;
        }

        public UnknownPacketException(int state, byte id) : base(
            $"An unknown packet with ID '{id}' was received during state '{state}'"
        )
        {
            Id = id;
            State = state;
        }
    }
}