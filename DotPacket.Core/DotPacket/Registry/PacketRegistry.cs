using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotPacket.Registry.Attributes;

namespace DotPacket.Registry
{
    public class PacketRegistry
    {
        private Dictionary<int, PacketContainer> _states;
        private List<(Packet, Type)> _packets;

        public PacketRegistry()
        {
            _states = new Dictionary<int, PacketContainer>();
            _packets = new List<(Packet, Type)>();
        }

        public void Register(params Type[] packets)
        {
            foreach (var packet in packets)
            {
                var attrs = packet.GetCustomAttributes(typeof(Packet), true);
                if (attrs.Length == 0)
                {
                    throw new InvalidRegistryOperationException(
                        $"Packet class '{packet.FullName}' is missing the [Packet] attribute"
                    );
                }

                _packets.Add(((Packet) attrs[0], packet));
            }
        }

        public void SetupFor(NetworkSide side)
        {
            foreach (var (packet, type) in _packets)
            {
                var container = _states[packet.State];
                if (container == null)
                {
                    container = _states[packet.State] = new PacketContainer();
                }
                
                container.Register(packet.Id, packet.Bound == side ? PacketBindindSide.Input : PacketBindindSide.Output, type);
            }
        }

        public Task Input(int state, uint id, byte[] data)
        {
            var container = _states[state];
            if (container == null)
            {
                throw new UnknownPacketException(state, id);
            }

            try
            {
                return container.Input(id, data);
            }
            catch (UnknownPacketException e)
            {
                throw new UnknownPacketException(state, e.Id);
            }
        }

        public Task<byte[]> Output(int state, uint id, object packet)
        {
            var container = _states[state];
            if (container == null)
            {
                throw new UnknownPacketException(state, id);
            }

            try
            {
                return container.Output(id, packet);
            }
            catch (UnknownPacketException e)
            {
                throw new UnknownPacketException(state, e.Id);
            }
        }
        
        // TODO: Handler defining
    }
}