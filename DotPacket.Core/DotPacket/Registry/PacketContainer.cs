using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DotPacket.Registry.Attributes;
using DotPacket.Serialization;

namespace DotPacket.Registry
{
    public class PacketContainer
    {
        private readonly Dictionary<uint, InputPacketBinding> _inputPackets;
        private readonly Dictionary<Type, OutputPacketBinding> _outputPackets;

        // TODO: Method (de)serializers
        
        public PacketContainer()
        {
            _inputPackets = new Dictionary<uint, InputPacketBinding>();
            _outputPackets = new Dictionary<Type, OutputPacketBinding>();
        }

        public void Register(byte id, PacketBindindSide side, Type packet)
        {
            var attrs = packet.GetCustomAttributes(typeof(ReflectionSerializing), true);
            var useReflection = attrs.Length != 0;

            switch (side)
            {
                case PacketBindindSide.Input:
                    var deserializer = GetProcessor<PacketDeserializer>(packet, typeof(Deserializer), useReflection);
                    _inputPackets[id] = new InputPacketBinding(id, packet, deserializer);
                    break;
                case PacketBindindSide.Output:
                    var serializer = GetProcessor<PacketSerializer>(packet, typeof(Serializer), useReflection);
                    _outputPackets[packet] = new OutputPacketBinding(id, packet, serializer);
                    break;
            }
        }

        public bool SetHandler(Type packet, Handler handler)
        {
            foreach (var (_, binding) in _inputPackets)
            {
                if (binding.Packet == packet)
                {
                    binding.Handler = handler;
                    return true;
                }
            }

            return false;
        }

        public Task Input(ConnectionContext context, byte id, byte[] data)
        {
            var binding = _inputPackets[id];
            if (binding == null)
            {
                throw new UnknownPacketException(id);
            }
            
            return binding.ReceiveAndHandle(context, data);
        }

        public (byte, Task<byte[]>) Output(object packet)
        {
            var binding = _outputPackets[packet.GetType()];
            if (binding == null)
            {
                throw new UnknownPacketException(packet.GetType());
            }

            return (binding.Id, binding.Serialize(packet));
        }

        private static P GetProcessor<P>(Type packet, Type attributeType, bool useReflection)
            where P: PacketProcessor
        {
            PacketProcessor processor = useReflection ? new ReflectionDeserializer(packet) : null; 
            var attrs = packet.GetCustomAttributes(attributeType, true);
                    
            if (attrs.Length > 0)
            {
                if (useReflection)
                {
                    throw new InvalidRegistryOperationException(
                        $"Class '{packet.FullName}' has both [UseReflection] attribute and [{attributeType.Name}]"
                    );
                }

                var desType = (Type) attrs[0];
                if (!desType.IsSubclassOf(typeof(P)))
                {
                    throw new InvalidRegistryOperationException(
                        $"class '{desType.FullName}' does not extend {typeof(P).Name}"
                    );
                }

                var cons = desType.GetConstructor(new[] {typeof(Type)});
                if (cons == null)
                {
                    throw new InvalidRegistryOperationException(
                        $"class '{desType.FullName}' should have a constructor with a single 'Type' parameter"
                    );
                }

                processor = (PacketDeserializer) cons.Invoke(new object[]{packet});
            }
            else if (processor == null)
            {
                throw new InvalidRegistryOperationException(
                    $"Class '{packet.FullName}' has no deserializer. " +
                    "Use attribute [UseReflection], or [Deserializer] and [Serializer]"
                );
            }

            processor.Prepare();
            return (P) processor;
        }
    }
}