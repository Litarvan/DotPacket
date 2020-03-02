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
        private readonly Dictionary<uint, OutputPacketBinding> _outputPackets;

        public PacketContainer()
        {
            _inputPackets = new Dictionary<uint, InputPacketBinding>();
            _outputPackets = new Dictionary<uint, OutputPacketBinding>();
        }

        public void Register(byte id, PacketBindindSide side, Type packet)
        {
            var attrs = packet.GetCustomAttributes(typeof(ReflectionSerializing), true);
            var useReflection = attrs.Length != 0;

            switch (side)
            {
                case PacketBindindSide.Input:
                    var deserializer = GetProcessor<PacketDeserializer>(packet, typeof(Deserializer), useReflection);
                    _inputPackets[id] = new InputPacketBinding(packet, deserializer);
                    break;
                case PacketBindindSide.Output:
                    var serializer = GetProcessor<PacketSerializer>(packet, typeof(Serializer), useReflection);
                    _outputPackets[id] = new OutputPacketBinding(packet, serializer);
                    break;
            }
        }

        public Task Input(byte id, byte[] data)
        {
            var binding = _inputPackets[id];
            if (binding == null)
            {
                throw new UnknownPacketException(id);
            }
            
            return binding.ReceiveAndHandle(data);
        }

        public Task<byte[]> Output(byte id, object packet)
        {
            var binding = _outputPackets[id];
            if (binding == null)
            {
                throw new UnknownPacketException(id);
            }

            if (packet.GetType() != binding.Packet)
            {
                throw new WrongPacketException(id, packet.GetType());
            }

            return binding.Serialize(packet);
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