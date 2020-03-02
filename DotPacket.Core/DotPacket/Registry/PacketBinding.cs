using System;
using System.Threading.Tasks;

namespace DotPacket.Registry
{
    public abstract class PacketBinding
    {
        private Type _packet;
        private Func<Object, Task> _handler;

        public Type Packet => _packet;
        public Func<Object, Task> Handler
        {
            get { return _handler; }
            set { _handler = value; }
        }

        protected PacketBinding(Type packet)
        {
            _packet = packet;
        }
    }
}