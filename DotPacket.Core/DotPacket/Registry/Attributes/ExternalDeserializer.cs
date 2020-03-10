using System;

namespace DotPacket.Registry.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExternalDeserializer : Attribute
    {
        public Type DeserializerClass { get; }

        public ExternalDeserializer(Type deserializerClass)
        {
            DeserializerClass = deserializerClass;
        }
    }
}