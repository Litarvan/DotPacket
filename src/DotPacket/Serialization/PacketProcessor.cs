using System;

namespace DotPacket.Serialization
{
    public abstract class PacketProcessor
    {
        private Type _type;

        public Type Type => _type;

        protected PacketProcessor(Type type)
        {
            _type = type;
        }
        
        public virtual void Prepare()
        {
        }
    }
}