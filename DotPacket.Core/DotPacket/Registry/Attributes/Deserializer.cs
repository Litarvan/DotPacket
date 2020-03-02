using System;

namespace DotPacket.Registry.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Deserializer : Attribute
    {
        private Type _deserializerClass;

        public Type DeserializerClass => _deserializerClass;

        public Deserializer(Type deserializerClass)
        {
            _deserializerClass = deserializerClass;
        }
    }
}