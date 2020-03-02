using System;

namespace DotPacket.Registry
{
    public class InvalidRegistryOperationException : Exception
    {
        public InvalidRegistryOperationException(string message) : base(message)
        {
        }
    }
}