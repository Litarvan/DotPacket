using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DotPacket.Registry.Attributes;

namespace DotPacket.Registry
{
    public class PacketRegistry
    {
        private readonly Dictionary<int, PacketContainer> _states;
        private readonly List<(Packet, Type)> _packets;

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

        public void SetHandler(Type packet, Handler handler)
        {
            foreach (var (_, container) in _states)
            {
                if (container.SetHandler(packet, handler))
                {
                    return;
                }
            }
            
            throw new InvalidRegistryOperationException(
                $"Can't find input-bound packet of type '{packet.FullName}', did you call SetupFor ?"
            );
        }

        public void AddHandlerClass(object cl)
        {
            foreach (var method in cl.GetType().GetMethods())
            {
                var attrs = method.GetCustomAttributes(typeof(Attributes.Handler), true);
                if (attrs.Length == 0)
                {
                    continue;
                }

                var packet = ((Attributes.Handler) attrs[0]).Packet;
                var pars = method.GetParameters();

                if (
                    pars.Length != 2
                        || !pars[0].ParameterType.IsAssignableFrom(typeof(ConnectionContext))
                        || !pars[1].ParameterType.IsAssignableFrom(packet)
                )
                {
                    throw new InvalidRegistryOperationException(
                        $"Handler method '{method.Name}' of handler class '{cl.GetType().FullName}' " +
                        "should take only two parameters where the first one is or is a sub-type of ConnectionContext, " +
                        $"and the second one is one that can be assignable from the packet type '{packet.FullName}'"
                    );
                }
                
                SetHandler(packet, async (context, o) =>
                {
                    var result = method.Invoke(cl, new[] {context, o});
                    if (result is Task task)
                    {
                        await task;
                    }
                });
            }
        }

        public void SetupFor(NetworkSide side)
        {
            if (_states.Count != 0)
            {
                return;
            }
            
            foreach (var (packet, type) in _packets)
            {
                PacketContainer container;
                if (_states.ContainsKey(packet.State))
                {
                    container = _states[packet.State];
                }
                else
                {
                    container = _states[packet.State] = new PacketContainer();
                }
                
                container.Register(packet.Id, packet.Bound == side ? PacketBindindSide.Input : PacketBindindSide.Output, type);
            }
        }

        public Task Input(ConnectionContext context, byte id, byte[] data)
        {
            if (!_states.ContainsKey(context.State))
            {
                throw new UnknownPacketException(context.State, id);
            }

            try
            {
                return _states[context.State].Input(context, id, data);
            }
            catch (UnknownPacketException e)
            {
                throw new UnknownPacketException(context.State, e.Id);
            }
        }

        public (byte, Task<byte[]>) Output(int state, object packet)
        {
            if (!_states.ContainsKey(state))
            {
                throw new UnknownPacketException(state);
            }
            
            try
            {
                return _states[state].Output(packet);
            }
            catch (UnknownPacketException e)
            {
                throw new UnknownPacketException(state, e.Id);
            }
        }
    }
}