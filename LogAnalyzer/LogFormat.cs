using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogAnalyzer
{
    public class LogFormat
    {
        public ushort _logPageId;      // 日志页ID: 0x55AA - 首帧页标识 ，0x55BB - 后续页标识
        public uint _nextLogAddr;      // 下一条记录地址
        public ushort _length;         // 帧长度: 日志页ID --> CRC16 的长度
        public byte _eccError;         // ECC错误
        public DateTime _logTime;      // 日志时间：6 byte （年、月、日、时、分、秒）
        public byte _logType { get; set; }           // 日志类型：1 - 无线数据，2 - 串口数据，3 - 调试数据
        public byte[] _data { get; set; }            // 日志数据
        public ushort _crc16 { get; set; }          // CRC16: 日志页ID --> 日志数据 的CRC-ITU校验


    }
}
