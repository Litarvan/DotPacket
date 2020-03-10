using System;

namespace DotPacket.Registry.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExternalSerializer : Attribute
    {
        public Type SerializerClass { get; }

        public ExternalSerializer(Type serializerClass)
        {
            SerializerClass = serializerClass;
        }
    }
}