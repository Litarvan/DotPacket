using System;

namespace DotPacket.Registry
{
    public class UnknownPacketException : Exception
    {
        public byte Id { get; }
        public int State { get; }
        public Type Type { get; }

        public UnknownPacketException(byte id) : base($"An unknown packet with ID '{id}' was received")
        {
            Id = id;
        }

        public UnknownPacketException(int state, byte id) : base(
            $"An unknown packet with ID '{id}' was received/tried to be sent during state '{state}'"
        )
        {
            Id = id;
            State = state;
        }

        public UnknownPacketException(int state) : base($"An unknown state '{state}' was given while sending a packet")
        {
            State = state;
        }
        
        public UnknownPacketException(Type type) : base($"An unknown packet type '{type.FullName}' was trying to be sent")
        {
            Type = type;
        }
    }
}