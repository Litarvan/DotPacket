using System;

namespace DotPacket.Serialization
{
    public abstract class PacketProcessor
    {
        public Type Type { get; }

        protected PacketProcessor(Type type)
        {
            Type = type;
        }
        
        public virtual void Prepare()
        {
        }
    }
}