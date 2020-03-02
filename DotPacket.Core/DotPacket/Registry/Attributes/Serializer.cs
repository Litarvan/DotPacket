using System;

namespace DotPacket.Registry.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Serializer : Attribute
    {
        public Type SerializerClass { get; }

        public Serializer(Type serializerClass)
        {
            SerializerClass = serializerClass;
        }
    }
}