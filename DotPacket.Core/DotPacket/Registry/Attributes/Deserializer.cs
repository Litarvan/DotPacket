using System;

namespace DotPacket.Registry.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Deserializer : Attribute
    {
        public Type DeserializerClass { get; }

        public Deserializer(Type deserializerClass)
        {
            DeserializerClass = deserializerClass;
        }
    }
}