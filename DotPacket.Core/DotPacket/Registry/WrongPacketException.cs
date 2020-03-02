using System;

namespace DotPacket.Registry
{
    public class WrongPacketException : Exception
    {
        public WrongPacketException(byte id, Type type) : base(
            $"Given ID {id} is not the one of the packet '{type.FullName}'"
        )
        {
        }
    }
}