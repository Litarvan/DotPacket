using System;
using System.Collections.Generic;

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
            foreach (var entry in _states)
            {
                if (entry.Value.SetHandler(packet, handler))
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
                        || !typeof(ConnectionContext).IsAssignableFrom(pars[0].ParameterType)
                        || !pars[1].ParameterType.IsAssignableFrom(packet)
                )
                {
                    throw new InvalidRegistryOperationException(
                        $"Handler method '{method.Name}' of handler class '{cl.GetType().FullName}' " +
                        "should take only two parameters where the first one is or is a sub-type of ConnectionContext, " +
                        $"and the second one is one that can be assignable from the packet type '{packet.FullName}'"
                    );
                }
                
                SetHandler(packet, (context, o) =>
                {
                    method.Invoke(cl, new[] {context, o});
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

                if (packet.Bound == NetworkSide.Both)
                {
                    container.Register(packet.Id, PacketBindindSide.Input, type);
                    container.Register(packet.Id, PacketBindindSide.Output, type);
                }
                else
                {
                    container.Register(packet.Id, packet.Bound == side ? PacketBindindSide.Input : PacketBindindSide.Output, type);
                }
            }
        }

        public void Input(ConnectionContext context, byte id, byte[] data)
        {
            if (!_states.ContainsKey(context.State))
            {
                throw new UnknownPacketException(context.State, id);
            }

            try
            {
                _states[context.State].Input(context, id, data);
            }
            catch (UnknownPacketException e)
            {
                throw new UnknownPacketException(context.State, e.Id);
            }
        }

        public (byte, byte[]) Output(int state, object packet)
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
                if (e.Type != null)
                {
                    throw new UnknownPacketException(state, e.Type);
                }

                throw new UnknownPacketException(state, e.Id);
            }
        }
    }
}