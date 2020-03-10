using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DotPacket.IO;
using DotPacket.Registry.Attributes;
using DotPacket.Serialization;

namespace DotPacket.Registry
{
    public class PacketContainer
    {
        private readonly Dictionary<uint, InputPacketBinding> _inputPackets;
        private readonly Dictionary<Type, OutputPacketBinding> _outputPackets;
        
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
                    var deserializer = GetProcessor<PacketDeserializer>(packet, ProcessorType.Deserializer, useReflection);
                    _inputPackets[id] = new InputPacketBinding(id, packet, deserializer);
                    break;
                case PacketBindindSide.Output:
                    var serializer = GetProcessor<PacketSerializer>(packet, ProcessorType.Serializer, useReflection);
                    _outputPackets[packet] = new OutputPacketBinding(id, packet, serializer);
                    break;
            }
        }

        public bool SetHandler(Type packet, Handler handler)
        {
            foreach (var entry in _inputPackets)
            {
                if (entry.Value.Packet == packet)
                {
                    entry.Value.Handler = handler;
                    return true;
                }
            }

            return false;
        }

        public Task Input(ConnectionContext context, byte id, byte[] data)
        {
            if (!_inputPackets.ContainsKey(id))
            {
                throw new UnknownPacketException(id);
            }

            return _inputPackets[id].ReceiveAndHandle(context, data);
        }

        public (byte, Task<byte[]>) Output(object packet)
        {
            if (!_outputPackets.ContainsKey(packet.GetType()))
            {
                throw new UnknownPacketException(packet.GetType());
            }

            var binding = _outputPackets[packet.GetType()];
            return (binding.Id, binding.Serialize(packet));
        }

        private static P GetProcessor<P>(Type packet, ProcessorType processorType, bool useReflection)
            where P: PacketProcessor
        {
            var classAttribute = processorType == ProcessorType.Serializer ? typeof(ExternalSerializer) : typeof(ExternalDeserializer);
            var attrs = packet.GetCustomAttributes(classAttribute, true);
            var processor = useReflection
                ? processorType == ProcessorType.Serializer
                    ? (PacketProcessor) new ReflectionSerializer(packet)
                    : new ReflectionDeserializer(packet)
                : null;
                    
            if (attrs.Length > 0)
            {
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

                processor = (P) cons.Invoke(new object[]{packet});
            }
            else if (processor == null)
            {
                var methods = packet.GetMethods();
                MethodInfo processorMethod = null;
                
                foreach (var method in methods)
                {
                    var attrType = processorType == ProcessorType.Serializer ? typeof(Serializer) : typeof(Deserializer);
                    var streamType = processorType == ProcessorType.Serializer ? typeof(StreamWriter) : typeof(StreamReader);
                    
                    var attr = method.GetCustomAttribute(attrType);
                    var pars = method.GetParameters();

                    if (attr != null)
                    {
                        if (pars.Length != 0 || pars[0].ParameterType != streamType)
                        {
                            throw new InvalidRegistryOperationException(
                                $"To be used as ${attrType.Name.ToLower()} of type '${packet.FullName}', " +
                                $"method '${method.Name}' must take one ${streamType.Name} parameter"
                            );
                        }

                        processorMethod = method;
                    }
                   
                }

                if (processorMethod == null)
                {
                    throw new InvalidRegistryOperationException(
                        $"Class '{packet.FullName}' has no serializer or deserializer. " +
                        "Use attribute [UseReflection], or [Serializer] and [Deserializer], " +
                        "or [ExternalSerializer] and [ExternalDeserializer]"
                    );
                }

                if (processorType == ProcessorType.Serializer)
                {
                    processor = new MethodPacketSerializer(processorMethod, packet);
                }
                else
                {
                    processor = new MethodPacketDeserializer(processorMethod, packet);
                }
            }

            processor.Prepare();
            return (P) processor;
        }
    }
}