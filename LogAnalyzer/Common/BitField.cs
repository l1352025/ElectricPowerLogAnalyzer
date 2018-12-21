using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectricPowerDebuger.Common
{
    public abstract class BitField
    {
        #region BitField -- bit len attr

        [global::System.AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public sealed class BitLengthAttribute : Attribute
        {
            private ushort _length;
            public BitLengthAttribute(ushort length)
            {
                _length = length;
            }
            public ushort Length { get { return _length; } }
        }
        #endregion

        #region BitField -- bit info
        sealed class BitInfo
        {
            private System.Reflection.FieldInfo _field;
            private uint _mask;
            private ushort _idx, _offset, _bitLen;
            private byte _shiftLeft, _shiftRight;
            private bool _isUnsigned = false;

            public BitInfo(System.Reflection.FieldInfo field, ushort bitLen, ushort idx, ushort offset)
            {
                _field = field;
                _bitLen = bitLen;
                _idx = idx;
                _offset = offset;
                _mask = (uint)(((1 << _bitLen) - 1) << _offset);
                _shiftLeft = (byte)(32 - _offset - _bitLen);
                _shiftRight = (byte)(32 - _bitLen);

                if (_field.FieldType == typeof(bool)
                    || _field.FieldType == typeof(byte)
                    || _field.FieldType == typeof(char)
                    || _field.FieldType == typeof(uint)
                    || _field.FieldType == typeof(ulong)
                    || _field.FieldType == typeof(ushort))
                {
                    _isUnsigned = true;
                }
            }

            public void EncodeToBuffer(Object obj, byte[] buffer)
            {
                if (_field.FieldType == typeof(byte[]))
                {
                    byte[] bytes = (byte[])(_field.GetValue(obj));
                    Array.Copy(bytes, 0, buffer, _idx, _bitLen / 8);
                }
                else if (_isUnsigned)
                {
                    uint val = (uint)Convert.ChangeType(_field.GetValue(obj), typeof(uint));
                    val = ((uint)(val << _offset) & _mask);
                    buffer[_idx]     |= (byte)(val & 0xFF);
                    if (_idx + 1 < buffer.Length)
                    {
                        buffer[_idx + 1] |= (byte)(val >> 8 & 0xFF);
                    }
                    if (_idx + 2 < buffer.Length)
                    {
                        buffer[_idx + 2] |= (byte)(val >> 16 & 0xFF);
                    }
                    if (_idx + 3 < buffer.Length)
                    {
                        buffer[_idx + 3] |= (byte)(val >> 24 & 0xFF);
                    }
                }
                else
                {
                    uint val = (uint)Convert.ChangeType(_field.GetValue(obj), typeof(int));
                    val = ((uint)(val << _offset) & _mask);
                    buffer[_idx] |= (byte)(val & 0xFF);
                    if (_idx + 1 < buffer.Length)
                    {
                        buffer[_idx + 1] |= (byte)(val >> 8 & 0xFF);
                    }
                    if (_idx + 2 < buffer.Length)
                    {
                        buffer[_idx + 2] |= (byte)(val >> 16 & 0xFF);
                    }
                    if (_idx + 3 < buffer.Length)
                    {
                        buffer[_idx + 3] |= (byte)(val >> 24 & 0xFF);
                    }
                }
            }

            public void DecodeFromBuffer(Object obj, byte[] buffer)
            {
                if (_field.FieldType == typeof(byte[]))
                {
                    byte[] bytes = new byte[_bitLen / 8];
                    Array.Copy(buffer, _idx, bytes, 0, _bitLen / 8);
                    _field.SetValue(obj, bytes);
                }
                else if (_isUnsigned)
                {
                    uint val = (uint)(buffer[_idx]);
                    val += ((_idx + 1) < buffer.Length  ?  (uint)(buffer[_idx + 1] << 8) : 0 );
                    val += ((_idx + 2) < buffer.Length ? (uint)(buffer[_idx + 2] << 16) : 0);
                    val += ((_idx + 3) < buffer.Length ? (uint)(buffer[_idx + 3] << 24) : 0);
                    _field.SetValue(obj, Convert.ChangeType((((uint)(val & _mask)) << _shiftLeft) >> _shiftRight, _field.FieldType));
                }
                else
                {
                    uint val = (uint)(buffer[_idx]);
                    val += ((_idx + 1) < buffer.Length ? (uint)(buffer[_idx + 1] << 8) : 0);
                    val += ((_idx + 2) < buffer.Length ? (uint)(buffer[_idx + 2] << 16) : 0);
                    val += ((_idx + 3) < buffer.Length ? (uint)(buffer[_idx + 3] << 24) : 0);
                    _field.SetValue(obj, Convert.ChangeType((((int)(val & _mask)) << _shiftLeft) >> _shiftRight, _field.FieldType));
                }
            }

        }
        #endregion

        #region BitField -- bit info pool
        sealed class BitInfoPool
        {
            public int _dataLen { get; private set; }
            public BitInfo[] _bitInfos { get; private set; }


            public BitInfoPool(int dataLen, BitInfo[] bitInfos)
            {
                _dataLen = dataLen;
                _bitInfos = bitInfos;
            }

            public byte[] ToArray<T>(T obj)
            {
                byte[] datas = new byte[_dataLen];

                foreach (BitInfo bif in _bitInfos)
                {
                    bif.EncodeToBuffer(obj, datas);
                }

                return datas;
            }

            public void Parse<T>(T obj, byte[] vals)
            {
                foreach (BitInfo bif in _bitInfos)
                {
                    bif.DecodeFromBuffer(obj, vals);
                }
            }
        }
        #endregion


        static Dictionary<Type, BitInfoPool> bitInfoMap = new Dictionary<Type, BitInfoPool>();

        public void Parse<T>(T[] vals)
        {
            GetBitInfoPool().Parse(this, ArrayConverter.Convert<T, byte>(vals));
        }

        public T[] ToArray<T>()
        {
            return ArrayConverter.Convert<byte, T>(GetBitInfoPool().ToArray(this));
        }
        public byte[] ToArray()
        {
            return GetBitInfoPool().ToArray(this);
        }

        private BitInfoPool GetBitInfoPool()
        {
            Type type = this.GetType();

            if (!bitInfoMap.ContainsKey(type))
            {
                List<BitInfo> infos = new List<BitInfo>();

                ushort dataIdx = 0, offset = 0, bitLen;

                foreach (System.Reflection.FieldInfo f in type.GetFields())
                {

                    object[] attrs = f.GetCustomAttributes(typeof(BitLengthAttribute), false);

                    if (attrs.Length == 1)
                    {
                        bitLen = ((BitLengthAttribute)attrs[0]).Length;

                        if (f.FieldType == typeof(byte[]) && offset != 0)
                        {
                            dataIdx += 1;
                            offset = 0;
                        }

                        infos.Add(new BitInfo(f, bitLen, dataIdx, offset));
                        dataIdx += (ushort)((bitLen + offset) / 8);
                        offset = (ushort)((bitLen + offset) % 8);
                    }
                }

                bitInfoMap.Add(type, new BitInfoPool(dataIdx, infos.ToArray()));
            }

            return bitInfoMap[type];
        }

        public override string ToString()
        {
            string str = "";

            foreach (System.Reflection.FieldInfo f in this.GetType().GetFields())
            {
                str += f.Name + " = ";
                if (f.FieldType == typeof(byte[]))
                {
                    foreach (var b in (byte[])f.GetValue(this))
                    {
                        str += b.ToString("X") + " ";
                    }
                }
                else
                {
                    str += f.GetValue(this) + "( 0x" + (Convert.ToUInt32(f.GetValue(this))).ToString("X") + " )";
                }
                str += "\r\n";
            }

            return str;
        }

    }

    public class ArrayConverter
    {
        public static T[] Convert<T>(uint[] val)
        {
            return Convert<uint, T>(val);
        }

        public static T1[] Convert<T0, T1>(T0[] val)
        {
            T1[] rt = null;

            // type is same or length is same

            // refer to http://stackoverflow.com/questions/25759878/convert-byte-to-sbyte

            if (typeof(T0) == typeof(T1))
            {
                rt = (T1[])(Array)val;
            }
            else
            {
                int len = Buffer.ByteLength(val);

                int w = TypeWidth<T1>();

                if (w == 1)
                { // bool
                    rt = new T1[len * 8];
                }
                else if (w == 8)
                {
                    rt = new T1[len];
                }
                else
                { // w > 8

                    int nn = w / 8;

                    int len2 = (len / nn) + ((len % nn) > 0 ? 1 : 0);

                    rt = new T1[len2];
                }

                Buffer.BlockCopy(val, 0, rt, 0, len);
            }

            return rt;
        }

        public static string ToBinary<T>(T[] vals)
        {
            StringBuilder sb = new StringBuilder();

            int width = TypeWidth<T>();

            int len = Buffer.ByteLength(vals);

            for (int i = len - 1; i >= 0; i--)
            {
                sb.Append(System.Convert.ToString(Buffer.GetByte(vals, i), 2).PadLeft(8, '0')).Append(" ");
            }

            return sb.ToString();
        }

        private static int TypeWidth<T>()
        {
            int rt = 0;

            if (typeof(T) == typeof(bool))
            { 
                rt = 1;
            }
            else if (typeof(T) == typeof(byte))
            { 
                rt = 8;
            }
            else if (typeof(T) == typeof(sbyte))
            {
                rt = 8;
            }
            else if (typeof(T) == typeof(ushort))
            { 
                rt = 16;
            }
            else if (typeof(T) == typeof(short))
            {
                rt = 16;
            }
            else if (typeof(T) == typeof(char))
            {
                rt = 16;
            }
            else if (typeof(T) == typeof(uint))
            {
                rt = 32;
            }
            else if (typeof(T) == typeof(int))
            {
                rt = 32;
            }
            else if (typeof(T) == typeof(float))
            {
                rt = 32;
            }
            else if (typeof(T) == typeof(ulong))
            { 
                rt = 64;
            }
            else if (typeof(T) == typeof(long))
            {
                rt = 64;
            }
            else if (typeof(T) == typeof(double))
            {
                rt = 64;
            }
            else
            {
                throw new Exception("Unsupport type : " + typeof(T).Name);
            }

            return rt;
        }
    }

}
