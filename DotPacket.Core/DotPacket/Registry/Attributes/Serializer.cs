using System;

namespace DotPacket.Registry.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Serializer : Attribute
    {
        private Type _serializerClass;

        public Type SerializerClass => _serializerClass;

        public Serializer(Type serializerClass)
        {
            _serializerClass = serializerClass;
        }
    }
}