using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using DotPacket.IO;
using DotPacket.Registry;

namespace DotPacket.Serialization
{
    public class ReflectionDeserializer : PacketDeserializer
    {
        private static readonly Des Byte = async stream => await stream.ReadByte();
        private static readonly Des Boolean = async stream => await stream.ReadBool();
        private static readonly Des Short = async stream => await stream.ReadShort();
        private static readonly Des UShort = async stream => await stream.ReadUnsignedShort();
        private static readonly Des Int = async stream => await stream.ReadInt();
        private static readonly Des UInt = async stream => await stream.ReadUnsignedInt();
        private static readonly Des Long = async stream => await stream.ReadLong();
        private static readonly Des ULong = async stream => await stream.ReadUnsignedLong();
        private static readonly Des Float = async stream => await stream.ReadFloat();
        private static readonly Des Double = async stream => await stream.ReadDouble();
        private static readonly Des Char = async stream => await stream.ReadChar();
        private static readonly Des String = async stream => await stream.ReadString();

        private ConstructorInfo _constructor;
        private readonly Dictionary<FieldInfo, Des> _deserializers;

        public ReflectionDeserializer(Type type) : base(type)
        {
            _deserializers = new Dictionary<FieldInfo, Des>();
        }

        public override void Prepare()
        {
            base.Prepare();

            var cons = Type.GetConstructor(new Type[0] );

            if (cons == null)
            {
                throw new InvalidRegistryOperationException(
                    $"Can't find the empty constructor on Packet class '{Type.FullName}', " +
                    $"required for reflection serialization"
                );
            }

            _constructor = cons;
            
            var fields = Type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                Des des = null;
                
                // TODO: Arrays/Lists

                if (field.FieldType == typeof(byte)) des = Byte;
                else if (field.FieldType == typeof(bool)) des = Boolean;
                else if (field.FieldType == typeof(short)) des = Short;
                else if (field.FieldType == typeof(ushort)) des = UShort;
                else if (field.FieldType == typeof(int)) des = Int;
                else if (field.FieldType == typeof(uint)) des = UInt;
                else if (field.FieldType == typeof(long)) des = Long;
                else if (field.FieldType == typeof(ulong)) des = ULong;
                else if (field.FieldType == typeof(float)) des = Float;
                else if (field.FieldType == typeof(double)) des = Double;
                else if (field.FieldType == typeof(char)) des = Char;
                else if (field.FieldType == typeof(string)) des = String;

                if (des == null)
                {
                    throw new InvalidRegistryOperationException(
                        $"Field '{field.Name}' of packet '{Type.FullName}' " +
                        $"has unknown type '{field.FieldType.FullName}'"
                    );
                }
                
                _deserializers.Add(field, des);
            }
        }

        public override async Task<object> Deserialize(byte[] data)
        {
            var stream = new StreamReader(new ByteArrayInputStream(data), DotPacket.DefaultBufferSize);
            var packet = _constructor.Invoke(new object[0]);
            
            foreach (var entry in _deserializers)
            {
                entry.Key.SetValue(packet, await entry.Value(stream));
            }
            
            return packet;
        }
    }

    internal delegate Task<object> Des(StreamReader stream);
}