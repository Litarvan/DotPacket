using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DotPacket.IO;
using DotPacket.Registry;

namespace DotPacket.Serialization
{
    public class ReflectionSerializer : PacketSerializer
    {
        private static readonly Se Byte = (stream, obj) => stream.WriteByte((byte) obj);
        private static readonly Se Short = (stream, obj) => stream.WriteShort((short) obj);
        private static readonly Se UShort = (stream, obj) => stream.WriteUnsignedShort((ushort) obj);
        private static readonly Se Int = (stream, obj) => stream.WriteInt((int) obj);
        private static readonly Se UInt = (stream, obj) => stream.WriteUnsignedInt((uint) obj);
        private static readonly Se Long = (stream, obj) => stream.WriteLong((long) obj);
        private static readonly Se ULong = (stream, obj) => stream.WriteUnsignedLong((ulong) obj);
        private static readonly Se Float = (stream, obj) => stream.WriteFloat((float) obj);
        private static readonly Se Double = (stream, obj) => stream.WriteDouble((double) obj);
        private static readonly Se Char = (stream, obj) => stream.WriteChar((char) obj);
        private static readonly Se String = (stream, obj) => stream.WriteString((string) obj);

        private readonly Dictionary<FieldInfo, Se> _serializers;
        
        public ReflectionSerializer(Type type) : base(type)
        {
            _serializers = new Dictionary<FieldInfo, Se>();
        }
        
        public override void Prepare()
        {
            base.Prepare();
            
            var fields = Type.GetFields();
            foreach (var field in fields)
            {
                Se se = null;
                
                // TODO: Arrays/Lists

                if (field.FieldType == typeof(byte)) se = Byte;
                else if (field.FieldType == typeof(short)) se = Short;
                else if (field.FieldType == typeof(ushort)) se = UShort;
                else if (field.FieldType == typeof(int)) se = Int;
                else if (field.FieldType == typeof(uint)) se = UInt;
                else if (field.FieldType == typeof(long)) se = Long;
                else if (field.FieldType == typeof(ulong)) se = ULong;
                else if (field.FieldType == typeof(float)) se = Float;
                else if (field.FieldType == typeof(double)) se = Double;
                else if (field.FieldType == typeof(char)) se = Char;
                else if (field.FieldType == typeof(string)) se = String;

                if (se == null)
                {
                    throw new InvalidRegistryOperationException(
                        $"Field '{field.Name}' of packet '{Type.FullName}' " +
                        $"has unknown type '{field.FieldType.FullName}'"
                    );
                }
                
                _serializers.Add(field, se);
            }
        }

        public override async Task<byte[]> Serialize(object packet)
        {
            var output = new ByteArrayOutputStream();
            var stream = new StreamWriter(output, DotPacket.DefaultBufferSize);
            
            foreach (var (field, se) in _serializers)
            {
                await se(stream, field.GetValue(packet));
            }
            
            return output.GetBytes();
        }
    }
    
    internal delegate Task Se(StreamWriter stream, object obj);
}