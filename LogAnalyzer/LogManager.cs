using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Runtime.InteropServices;

namespace LogAnalyzer
{
    using ElectricPowerDebuger.Common;
    using ElectricPowerDebuger.Protocol;
    public partial class LogManager : UserControl
    {
        private string _configPath;
        private SerialCom _scom;
        private ConcurrentQueue<byte[]> _recvQueue;
        private ConcurrentQueue<Command> _sendQueue;
        private Thread _thrTransceiver;
        private bool _IsSendNewCmd;
        private int _currPacketNo;
        private byte _FrameSn;
        private bool _isLogLoading;
        private bool _isLogAutoSave;
        private NwkAnalysis _netInfo;
        private string _logFileName;
        private List<StationData> _stationList;
        private StationData _stationData;
        private DailyData _dailyData;
        
        private static readonly string[] _logReadAckTypeTbl = new string[]
        {
            "记录未结束",
            "正常结束",
            "异常结束",
            "记录不存在",
            "包序号错误",
        };

        public LogManager()
            : this(Application.ExecutablePath.Substring(0, Application.ExecutablePath.Length - 4) + ".cfg")
        {
        }
        public LogManager(string configFilePath)
        {
            InitializeComponent();
            dgvLog.DoubleBuffered(true);
            this.Dock = DockStyle.Fill;

            _configPath = configFilePath;
            _scom = new SerialCom();
            _scom.DataReceivedEvent += serialPort_DataReceived;
            _recvQueue = new ConcurrentQueue<byte[]>();
            _sendQueue = new ConcurrentQueue<Command>();
            _netInfo = new NwkAnalysis();
            _stationList = new List<StationData>();
            _thrTransceiver = new Thread(TransceiverHandle);
            _thrTransceiver.IsBackground = true;
            _thrTransceiver.Start();

            combPortNum.Text = XmlHelper.GetNodeDefValue(_configPath, "/Config/PortName", "");
            combPortBaud.Text = XmlHelper.GetNodeDefValue(_configPath, "/Config/Baudrate", "9600");
            combPortChk.Text = XmlHelper.GetNodeDefValue(_configPath, "/Config/BitAndCheck", "8E1");
            string strTmp = "";
            strTmp = XmlHelper.GetNodeDefValue(_configPath, "/Config/LogReadType", "hour");
            if ("month" == strTmp) rbReadByMth.Checked = true;
            else if ("day" == strTmp) rbReadByDay.Checked = true;
            else if ("hour" == strTmp) rbReadByHour.Checked = true;

            strTmp = XmlHelper.GetNodeDefValue(_configPath, "/Config/LogTimeHourStart", DateTime.Now.Hour.ToString("D2"));
            combHourStart.SelectedIndex = Convert.ToInt32(strTmp);

            strTmp = XmlHelper.GetNodeDefValue(_configPath, "/Config/LogTimeHourEnd", DateTime.Now.Hour.ToString("D2"));
            combHourEnd.SelectedIndex = Convert.ToInt32(strTmp);

            strTmp = XmlHelper.GetNodeDefValue(_configPath, "/Config/LogAutoSave", "True");
            chkAutoSave.Checked = strTmp == "True" ? true : false;

        }

        private delegate void CommandHandler(Command cmd);
        private class Command
        {
            public string Name;
            public byte[] TxBuf;
            public byte[] RxBuf;
            public List<string> Params;
            public int TimeWaitMS;
            public int RetryTimes;
            public bool IsEnable;
            public CommandHandler SendFunc;
            public CommandHandler RecvFunc;

            public Command()
                :this(null, null, null, 0, 3)
            { 
            }
            public Command(string cmdName, CommandHandler sendFunc, CommandHandler recvFunc, int timeOut, int retryTimes)
            {
                Name = cmdName;
                TimeWaitMS = timeOut;
                RetryTimes = retryTimes;
                SendFunc = sendFunc;
                RecvFunc = recvFunc;
                Params = new List<string>();
            }
        };

        private Command GetCmdSendHandler(string cmdName)
        {
            Command cmd = new Command();

            cmd.Name = cmdName;

            switch (cmdName)
            {
                case "查询微功率无线从节点信息":
                    cmd.TimeWaitMS = 500;
                    cmd.RetryTimes = 3;
                    cmd.SendFunc = ReadMicroWirelessSubNodeInfoCmd;
                    cmd.RecvFunc = ReadMicroWirelessSubNodeInfoResponse;
                    break;

                case "读台区概况":
                    cmd.TimeWaitMS = 150;
                    cmd.RetryTimes = 3;
                    cmd.SendFunc = ReadNetSummeryInfoCmd;
                    cmd.RecvFunc = ReadNetSummeryInfoResponse;
                    break;

                case "读取中心节点邻居表":
                    cmd.TimeWaitMS = 250;
                    cmd.RetryTimes = 3;
                    cmd.SendFunc = ReadMainNodeNeighborTblCmd;
                    cmd.RecvFunc = ReadMainNodeNeighborTblResponse;
                    break;

                case "读取子节点参数信息：档案信息":
                    cmd.TimeWaitMS = 150;
                    cmd.RetryTimes = 3;
                    cmd.SendFunc = ReadSubNodeParamsInfoCmd;
                    cmd.RecvFunc = ReadSubNodeParamsInfoResponse;
                    break;
                case "读取子节点参数信息：邻居表":
                    cmd.TimeWaitMS = 400;
                    cmd.RetryTimes = 3;
                    cmd.SendFunc = ReadSubNodeParamsInfoCmd;
                    cmd.RecvFunc = ReadSubNodeParamsInfoResponse;
                    break;
                case "读取子节点参数信息：路径表":
                    cmd.TimeWaitMS = 250;
                    cmd.RetryTimes = 3;
                    cmd.SendFunc = ReadSubNodeParamsInfoCmd;
                    cmd.RecvFunc = ReadSubNodeParamsInfoResponse;
                    break;

                default: break;
            }

            return cmd;
        }

        private void ExplainCmdRecvHandler(Command cmd)
        {
            string fnName;

            byte afn = cmd.RxBuf[10];
            byte fn = ProtoLocal_North.DataIdToFn((ushort)(cmd.RxBuf[11] + cmd.RxBuf[12] * 256));
            fnName = ProtoLocal_North.ExplainFn(afn, fn); 

            switch(fnName)
            {
                case "查询微功率无线从节点信息": ReadMicroWirelessSubNodeInfoResponse(cmd);  break;

                case "查询厂商代码和版本信息": ReadNetSummeryInfoResponse(cmd); break;
                case "查询主节点地址": ReadNetSummeryInfoResponse(cmd); break;
                case "查询无线通信参数": ReadNetSummeryInfoResponse(cmd); break;
                case "查询场强门限": ReadNetSummeryInfoResponse(cmd); break;
                case "查询路由运行状态": ReadNetSummeryInfoResponse(cmd); break;
                case "读取子节点概要信息": ReadNetSummeryInfoResponse(cmd); break;

                case "读取中心节点邻居表": ReadMainNodeNeighborTblResponse(cmd); break;

                case "读取子节点参数信息": ReadSubNodeParamsInfoResponse(cmd); break;

                default: break;
            }
        }
       
        
        
        #region 串口通信
        
        //串口选择
        private void combPortNum_Click(object sender, EventArgs e)
        {
            combPortNum.Items.Clear();
            combPortNum.Items.AddRange(SerialPort.GetPortNames());
        }
        
        //串口打开/关闭
        private void btPortCtrl_Click(object sender, EventArgs e)
        {
            if(combPortNum.Text == "" || combPortBaud.Text == "" || combPortChk.Text == "")
            {
                MessageBox.Show("请设置端口后打开");
                return;
            }

            if (btPortCtrl.Text == "打开" && true == PortCtrl(true))
            {
                btPortCtrl.Text = "关闭";
                btPortCtrl.BackColor = Color.GreenYellow;
                combPortNum.Enabled = false;
                combPortBaud.Enabled = false;
                combPortChk.Enabled = false;
                btLogRead.Enabled = true;
                btReadNetInfo.Enabled = true;

                XmlHelper.SetNodeValue(_configPath, "/Config", "PortName", combPortNum.Text);
                XmlHelper.SetNodeValue(_configPath, "/Config", "Baudrate", combPortBaud.Text);
                XmlHelper.SetNodeValue(_configPath, "/Config", "BitAndCheck", combPortChk.Text);
            }
            else
            {
                PortCtrl(false);
                btPortCtrl.Text = "打开";
                btPortCtrl.BackColor = Color.Silver;
                combPortNum.Enabled = true;
                combPortBaud.Enabled = true;
                combPortChk.Enabled = true;
                btLogRead.Enabled = false;
                btReadNetInfo.Enabled = false;
            }
        }

        private bool PortCtrl(bool ctrl)
        {
            if (true == ctrl)
            {
                if (_scom.IsOpen == false)
                {
                    try
                    {
                        _scom.Config(combPortNum.Text, Convert.ToInt32(combPortBaud.Text), combPortChk.Text);
                        _scom.Open();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("打开通信端口失败" + "," + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            else
            {
                try
                {
                    _scom.Close();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("关闭通信端口失败" + "," + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        //串口发送
        private void serialPort_SendData(byte[] buf)
        {
            if (true == _scom.IsOpen)
            {
                try
                {
                    _scom.WritePort(buf, 0, buf.Length);

                    LogToFile(Util.GetStringHexFromByte(buf, 0, buf.Length, " "));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("发送错误:" + ex.Message);
                }
            }
        }

        //串口接收
        private void serialPort_DataReceived(byte[] buf)
        {
            LogToFile(Util.GetStringHexFromByte(buf, 0, buf.Length, " "));

            if(buf[0] != 0x68  ||  buf[buf.Length - 1] != 0x16)
            {
                return;
            }

            _recvQueue.Enqueue(buf);
        }

        #endregion

        #region 命令处理--发送、接收

        // 发送、接收处理
        private void TransceiverHandle()
        {
            Command cmd = null;
            TimeSpan timeWait = TimeSpan.MaxValue;
            DateTime lastSendTime = DateTime.Now;

            while (Thread.CurrentThread.IsAlive)
            {
                // send a new command
                if (_IsSendNewCmd && _sendQueue.Count > 0 && _sendQueue.TryDequeue(out cmd))
                {
                    cmd.IsEnable = true;
                    timeWait = TimeSpan.MaxValue;
                }

                // send and retry
                if (cmd != null && cmd.IsEnable && timeWait.TotalMilliseconds > cmd.TimeWaitMS)
                {
                    if (cmd.RetryTimes > 0)
                    {
                        cmd.SendFunc(cmd);

                        // show msg
                        ShowStatus("[ " + cmd.Name + " ] 已发送：" + (3 - cmd.RetryTimes + 1), Color.Blue);

                        cmd.RetryTimes--;
                        lastSendTime = DateTime.Now;

                        _IsSendNewCmd = false;
                    }
                    else
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;
                    }
                }

                // wait
                Thread.Sleep(100);

                // receive
                if (cmd != null && _recvQueue.Count > 0 && _recvQueue.TryDequeue(out cmd.RxBuf))
                {
                    // show msg
                    ShowStatus("[ " + cmd.Name + " ] 收到数据", Color.Green);

                    cmd.RecvFunc(cmd);

                    cmd.IsEnable = true;
                }

                timeWait = DateTime.Now - lastSendTime;
            }
        }
        #endregion

        #region 日志管理--读取、保存、导入、清空

        //读取规则变化
        private void rbReadByHour_CheckedChanged(object sender, EventArgs e)
        {
            combHourStart.Enabled = rbReadByHour.Checked;
            combHourEnd.Enabled = rbReadByHour.Checked;
        }

        private void combHourStart_SelectedIndexChanged(object sender, EventArgs e)
        {
            combHourEnd.SelectedIndex = combHourStart.SelectedIndex;
        }

        //读取日志
        private void btLogRead_Click(object sender, EventArgs e)
        {
            int index = 0, hourIndex = 0;
            byte hourStart = 0, hourEnd = 0, crc;
            Command sendCmd = new Command();

            sendCmd.TimeWaitMS = 500;
            sendCmd.RetryTimes = 3;
            sendCmd.SendFunc = LogReadCmd;
            sendCmd.RecvFunc = LogReadResponse;
            
            byte[] tmpBuf = new byte[128];

            tmpBuf[index++] = 0x68;         //起始符
            tmpBuf[index++] = 0;            //长度，累加后再设置
            tmpBuf[index++] = 0;
            tmpBuf[index++] = 0x4A;         //控制域
            tmpBuf[index++] = 0x00;         //信息域
            tmpBuf[index++] = 0x00;
            tmpBuf[index++] = 0x00;
            tmpBuf[index++] = 0x00;
            tmpBuf[index++] = 0x00;
            tmpBuf[index++] = _FrameSn++;
            tmpBuf[index++] = 0xF0;         //AFN
            tmpBuf[index++] = 0x01;         //DT1
            tmpBuf[index++] = 0x00;         //DT2

            if(rbReadByMth.Checked)
            {
                sendCmd.Name = string.Format("读取日志( {0}年{1}月 )", 
                                dtPicker.Value.Year, dtPicker.Value.Month);
                
                tmpBuf[index++] = 0x02;      //读取规则 - 按月
                tmpBuf[index++] = (byte)dtPicker.Value.Month;    // 月
                tmpBuf[index++] = 0x00;                          // 日
                tmpBuf[index++] = 0x00;                          // 时

                XmlHelper.SetNodeValue(_configPath, "/Config", "LogReadType", "month");
            }
            else if (rbReadByDay.Checked)
            {
                sendCmd.Name = string.Format("读取日志( {0}年{1}月{2}日 )", 
                                dtPicker.Value.Year, dtPicker.Value.Month, dtPicker.Value.Day);

                tmpBuf[index++] = 0x01;     //读取规则 - 按日
                tmpBuf[index++] = (byte)dtPicker.Value.Month;    // 月
                tmpBuf[index++] = (byte)dtPicker.Value.Day;      // 日
                tmpBuf[index++] = 0x00;                          // 时

                XmlHelper.SetNodeValue(_configPath, "/Config", "LogReadType", "day");
            }
            else if (rbReadByHour.Checked)
            {
                if( combHourStart.SelectedIndex < 0)
                {
                    MessageBox.Show("[小时] 时间段未设置！");
                    return;
                }
                hourStart = Convert.ToByte(combHourStart.SelectedIndex);
                hourEnd = Convert.ToByte(combHourEnd.SelectedIndex);
                sendCmd.Name = string.Format("读取日志( {0}-{1}-{2} {3:D2}时 )",
                                dtPicker.Value.Year, dtPicker.Value.Month, dtPicker.Value.Day, hourStart);

                tmpBuf[index++] = 0x00;     //读取规则 - 按时
                tmpBuf[index++] = (byte)dtPicker.Value.Month;    // 月
                tmpBuf[index++] = (byte)dtPicker.Value.Day;      // 日
                hourIndex = index;
                tmpBuf[index++] = hourStart;                     // 时

                XmlHelper.SetNodeValue(_configPath, "/Config", "LogReadType", "hour");
                XmlHelper.SetNodeValue(_configPath, "/Config", "LogTimeHourStart", hourStart.ToString());
                XmlHelper.SetNodeValue(_configPath, "/Config", "LogTimeHourEnd", hourEnd.ToString());
            }
            tmpBuf[index++] = 0;             // 记录包号
            tmpBuf[index++] = 0;
            tmpBuf[index++] = 0;
            tmpBuf[index++] = 0;
            crc = 0;
            for (int i = 3; i < index; i++)
            {
                crc += tmpBuf[i];
            }
            tmpBuf[index++] = crc;          // crc
            tmpBuf[index++] = 0x16;         // 结束符

            tmpBuf[1] = (byte)(index & 0xFF);   // 长度
            tmpBuf[2] = (byte)((index >> 8) & 0xFF);

            //拷贝到发送缓存，压入命令发送队列
            sendCmd.TxBuf = new byte[index];
            Array.Copy(tmpBuf, sendCmd.TxBuf, sendCmd.TxBuf.Length);
            _sendQueue.Enqueue(sendCmd);

            if (rbReadByHour.Checked)
            {
                int remain = hourStart <= hourEnd ? (hourEnd - hourStart) : (hourEnd - hourStart + 24);
                int day, hour;

                for (int i = hourStart + 1; i <= hourStart + remain; i++)
                {
                    day = i / 24 + dtPicker.Value.Day;
                    hour = i % 24;

                    Command nextSendCmd = new Command();
                    nextSendCmd.TimeWaitMS = 500;
                    nextSendCmd.RetryTimes = 3;
                    nextSendCmd.SendFunc = LogReadCmd;
                    nextSendCmd.RecvFunc = LogReadResponse;
                    nextSendCmd.Name = string.Format("读取日志( {0}-{1}-{2} {3:D2}时 )",
                                dtPicker.Value.Year, dtPicker.Value.Month, day, hour.ToString("D2"));

                    nextSendCmd.TxBuf = new byte[sendCmd.TxBuf.Length];
                    sendCmd.TxBuf.CopyTo(nextSendCmd.TxBuf, 0);

                    //修改 帧序号、日、时 、校验和
                    nextSendCmd.TxBuf[9] = _FrameSn++;      // 帧序号
                    nextSendCmd.TxBuf[hourIndex - 1] = (byte)day;       // 日
                    nextSendCmd.TxBuf[hourIndex] = (byte)hour;          // 时
                    
                    crc = 0;
                    for (int k = 3; k < nextSendCmd.TxBuf.Length - 2; k++)
                    {
                        crc += nextSendCmd.TxBuf[k];
                    }
                    nextSendCmd.TxBuf[nextSendCmd.TxBuf.Length - 2] = crc;  // crc

                    _sendQueue.Enqueue(nextSendCmd);
                }
            }

            _IsSendNewCmd = true;

        }
        /// <summary>
        /// 日志读取-命令
        /// </summary>
        /// 发送帧结构：起始符1 + 帧长2 + 控制域1 + 信息域6 + AFN1 + DTI2 + 读取规则1 + 读取类型3 + 记录包号4 + 校验和1 + 结束符1
        private void LogReadCmd(Command cmd)
        {
            if (cmd.Name.Contains("读取日志"))
            {
                if (_IsSendNewCmd)
                {
                    _currPacketNo = 0;
                }

                cmd.TxBuf[17] = (byte)(_currPacketNo);
                cmd.TxBuf[18] = (byte)(_currPacketNo >> 8);
                cmd.TxBuf[19] = (byte)(_currPacketNo >> 16);
                cmd.TxBuf[20] = (byte)(_currPacketNo >> 24);

                byte crc = 0;
                for (int i = 3; i < cmd.TxBuf.Length - 2; i++)
                {
                    crc += cmd.TxBuf[i];
                }
                cmd.TxBuf[cmd.TxBuf.Length - 2] = crc;

                serialPort_SendData(cmd.TxBuf);
            }
        }
        /// <summary>
        /// 日志读取-响应
        /// </summary>
        /// 接收帧结构：起始符1 + 帧长2 + 控制域1 + 信息域6 + AFN1 + DTI2 + 回复标识1 + 记录包号4 + 记录数据N + 校验和1 + 结束符1
        private void LogReadResponse(Command cmd)
        {
            if (cmd.Name.Contains("读取日志"))  //读日志命令
            {
                // 非读日志响应帧
                if (cmd.RxBuf[0] != 0x68 || cmd.RxBuf[cmd.RxBuf.Length - 1] != 0x16
                    || (cmd.RxBuf[1] + (cmd.RxBuf[2] << 8)) != cmd.RxBuf.Length     // length
                    || cmd.RxBuf[3] != 0x8A         // ctrl word
                    || cmd.RxBuf[10] != 0xF0        // AFN
                    || cmd.RxBuf[11] != 0x01        // DT1
                    || cmd.RxBuf[12] != 0x00        // DT0
                    || cmd.RxBuf.Length < 16)
                {
                    _IsSendNewCmd = true;
                    cmd.IsEnable = false;

                    Log2List(cmd.RxBuf, 0, cmd.RxBuf.Length);
                    return;
                }

                byte readLogCmdAck = cmd.RxBuf[13];

                ShowStatus("[ " + cmd.Name + " ] ：" + _logReadAckTypeTbl[readLogCmdAck], Color.Green); // show ack

                if (0 == readLogCmdAck)
                {
                    // 读取未结束
                    _IsSendNewCmd = false;     // next packet

                    if ((cmd.RxBuf.Length > 20 + 18) && (cmd.RxBuf[18 + 6] + (cmd.RxBuf[18 + 7] << 8)) == cmd.RxBuf.Length - 20)
                    {
                        //当前包正确接收，读取下一包
                        _currPacketNo++;

                        cmd.RetryTimes = 3;

                        Log2List(cmd.RxBuf, 14, (cmd.RxBuf.Length - 14 - 2));
                    }
                }
                else
                {
                    // 读取已结束：正常结束、异常结束、记录不存在、包序号错误
                    cmd.RetryTimes = 0;     // next cmd

                    Log2List(cmd.RxBuf, 0, cmd.RxBuf.Length);
                }
            }
        }

        //保存日志
        private void btLogSave_Click(object sender, EventArgs e)
        {
            string strDirectory;
            string strFileName;

            if (0 == dtLog.Rows.Count)
            {
                MessageBox.Show("没有可保存的日志!");
                return;
            }

            strDirectory = XmlHelper.GetNodeDefValue(_configPath, "/Config/LogPath", Application.StartupPath);
            saveFileDlg.Filter = "*.txt(文本文件)|*.txt";
            saveFileDlg.DefaultExt = "txt";
            saveFileDlg.FileName = "";
            saveFileDlg.ShowDialog();

            strFileName = saveFileDlg.FileName;
            if (strFileName.Length == 0)
            {
                return;
            }

            if (strDirectory != Path.GetDirectoryName(strFileName))
            {
                strDirectory = Path.GetDirectoryName(strFileName);
                XmlHelper.SetNodeValue(_configPath, "/Config", "LogPath", strDirectory);
            }
            StreamWriter sw = new StreamWriter(strFileName, false, Encoding.UTF8);

            if (dgvLog.SelectedRows.Count > 1)     // save selected
            {
                foreach (DataGridViewRow row in dgvLog.SelectedRows)
                {
                    string strLine = DateTime.Now.ToString("[ yyyy-MM-dd HH:mm:ss ] ");

                    if (dtLog.Rows[row.Index]["时间"].ToString() != "")
                    {
                        strLine = "[ " + dtLog.Rows[row.Index]["时间"].ToString() + " ] ";
                    }

                    strLine += dtLog.Rows[row.Index]["序号"].ToString() + " ";
                    byte[] databuf = (byte[])dtLog.Rows[row.Index]["日志帧"];

                    foreach (byte data in databuf)
                    {
                        strLine += data.ToString("X2") + " ";
                    }

                    sw.WriteLine(strLine);
                }
            }
            else    // save all
            {
                foreach (DataGridViewRow row in dgvLog.Rows)
                {
                    string strLine = DateTime.Now.ToString("[ yyyy-MM-dd HH:mm:ss ] ");

                    if (dtLog.Rows[row.Index]["时间"].ToString() != "")
                    {
                        strLine = "[ " + dtLog.Rows[row.Index]["时间"].ToString() + " ] ";
                    }

                    strLine += dtLog.Rows[row.Index]["序号"].ToString() + " ";
                    byte[] databuf = (byte[])dtLog.Rows[row.Index]["日志帧"];

                    foreach (byte data in databuf)
                    {
                        strLine += data.ToString("X2") + " ";
                    }

                    sw.WriteLine(strLine);
                }
            }
            sw.Close();
            MessageBox.Show("保存档案成功！");
        }
        private void 保存日志ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btLogSave_Click(sender, e);
        }
        //自动保存选项
        private void chkAutoSave_CheckedChanged(object sender, EventArgs e)
        {
            XmlHelper.SetNodeValue(_configPath, "Config", "LogAutoSave", chkAutoSave.Checked.ToString());
            _isLogAutoSave = chkAutoSave.Checked;
        }

        //载入日志
        private void btLogLoad_Click(object sender, EventArgs e)
        {
            string strDirectory, strFileName, strRead;

            _isLogLoading = true;

            strDirectory = XmlHelper.GetNodeDefValue(_configPath, "/Config/LogPath", Application.StartupPath);
            openFileDlg.InitialDirectory = strDirectory;
            openFileDlg.Filter = "*.*(所有文件)|*.*|*.TXT(文本文件)|*.TXT";
            openFileDlg.DefaultExt = "*";
            openFileDlg.FileName = "";
            if (DialogResult.OK != openFileDlg.ShowDialog())
            {
                _isLogLoading = false;
                return;
            }

            strFileName = openFileDlg.FileName;
            if (strFileName.Length == 0)
            {
                _isLogLoading = false;
                return;
            }

            if (strDirectory != Path.GetDirectoryName(strFileName))
            {
                strDirectory = Path.GetDirectoryName(strFileName);
                XmlHelper.SetNodeValue(_configPath, "/Config", "LogPath", strDirectory);
            }

            if (_dailyData == null)
            {
                _dailyData = new DailyData();
            }

            StreamReader sr = new StreamReader(strFileName, Encoding.UTF8);

            while ((strRead = sr.ReadLine()) != null)
            {
                if (strRead == "") continue;

                try
                {
                    DateTime dt;

                    if (DateTime.TryParse(strRead.Substring(0, "2018 01/22 07:38:27".Length), out dt))
                    {
                        // 南京新联-集中器日志：
                        if (strRead.Contains("Send") || strRead.Contains("Recv"))
                        {
                            // plccom.log 文件格式：2018 01/22 07:38:27 Send:682f004a04001500008781510000089917078401300013010002000010681707840130006811043433393890168c16
                            //                      2018 01/22 07:38:29 Recv:6810008a0000000000870002000c1f16 
                            ParsePlccomLog(strRead);
                        }
                        else
                        {
                            // plcrec.log 文件格式：2018 01/30 18:55:02 已抄测量点个数:20
                            //                      2018 01/30 18:55:02 Pn=62,关键数据项抄读完成,关键任务抄表成功数:22
                            ParsePlcrecLog(strRead);
                        }
                    }
                    else
                    {
                        // 上海桑锐-主模块日志：
                        // 保存的日志格式：[ 2018-05-11 18:14:47 ] 序号6 + 包号4 + 页ID2 + 下一条地址4 + 帧长2 + ECC错误1 + 时间6 + 类别1 + 数据N + FCS2
                        strRead = strRead.Substring(strRead.IndexOf(']') + 8).Trim();   // 从包号开始
                        string[] strSplit = strRead.Split(' ');
                        byte[] byteArray = new byte[strSplit.Length];
                        for (int iLoop = 0; iLoop < strSplit.Length; iLoop++)
                        {
                            byteArray[iLoop] = Convert.ToByte(strSplit[iLoop], 16);
                        }

                        Log2List(byteArray, 0, byteArray.Length);
                    }
                }
                catch (Exception ex)
                {
                    //ShowStatus("载入日志异常：" + ex.Message + "\r\n" + strRead , Color.Red);
                    //break;
                }
            }
            sr.Close();

            dgvLog.FirstDisplayedScrollingRowIndex = dgvLog.RowCount - 1;

            ShowStatus("载入日志完成", Color.Green);

            _isLogLoading = false;

            combStationList.Items.Clear();
            for (int i = 0; i < _stationList.Count; i++ )
            {
                if (_stationList.ElementAt(i).CenterAddr != null)
                {
                    combStationList.Items.Add(_stationList.ElementAt(i).CenterAddr);
                }
            }
        }
        private void 导入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btLogLoad_Click(sender, e);
        }

        //清空日志
        private void 清空ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dtLog.Clear();
        }
        #endregion

        #region 日志显示
        
        //添加到日志列表
        delegate void Invoke_AddToList(byte[] buf, int logStartIndex, int logLength);
        private void Log2List(byte[] buf, int logStartIndex, int logLength)
        {
            if(this.InvokeRequired)
            {
                Invoke(new Invoke_AddToList(Log2List), new object[] { buf, logStartIndex, logLength});
                return;
            }

            const int LogFixedLen = 22;     // 包号4 + 页ID2 + 下一条地址4 + 帧长2 + ECC错误1 + 时间6 + 类别1 + 数据N + FCS2
            DataRow row = dtLog.NewRow();

            // 读日志响应帧无效
            if (logLength < LogFixedLen
                || (buf[logStartIndex + 10] + (buf[logStartIndex + 11] << 8) + 4) != logLength)
            {
                row["序号"] = (dtLog.Rows.Count + 1).ToString("D6");

                string type = "无法识别";

                if (buf.Length > 14
                    && buf[3] == 0x8A         // ctrl word
                    && buf[10] == 0xF0        // AFN
                    && buf[11] == 0x01        // DT1
                    && buf[12] == 0x00)       // DT0
                {
                    type = (buf[13] < _logReadAckTypeTbl.Length ? _logReadAckTypeTbl[buf[13]] : "记录错误");
                }
                row["类别"] = type; 

                byte[] data = new byte[buf.Length];
                buf.CopyTo(data, 0);
                row["日志数据"] = data;

                row["日志帧"] = buf;      // 保存整个日志帧 ( 包号 --> FCS )

                dtLog.Rows.Add(row);

            }
            else    // 有效日志帧
            {

                int index = logStartIndex;

                row["序号"] = (dtLog.Rows.Count + 1).ToString("D6");

                row["包号"] = BitConverter.ToUInt32(buf, index).ToString();
                index += 4;

                row["页ID"] = BitConverter.ToUInt16(buf, index).ToString("X4");
                index += 2;

                row["下一条记录"] = BitConverter.ToUInt32(buf, index).ToString("X8");
                index += 4;

                row["帧长"] = BitConverter.ToUInt16(buf, index).ToString();
                index += 2;

                row["ECC错误"] = buf[index++].ToString();

                DateTime dt = new DateTime(DateTime.Now.Year / 100 * 100 + buf[index],
                                            buf[index + 1],
                                            buf[index + 2],
                                            buf[index + 3],
                                            buf[index + 4],
                                            buf[index + 5]);
                row["时间"] = dt.ToString("yyyy-MM-dd HH:mm:ss");
                index += 6;

                byte logType = buf[index++];
                byte[] logData = new byte[logLength - LogFixedLen];
                Array.Copy(buf, index, logData, 0, logData.Length);

                Color frameColor = Color.Black;
                string strLogType = "", strDstAddr = "", strSrcAddr = "", strFrameType = "", strComment = "";

                switch (logType)
                {
                    case 0x01:
                        {
                            strLogType = "无线数据";
                            ProtoWireless_NiBoEr.FrameFormat Frame = ProtoWireless_NiBoEr.ExplainRxPacket(logData);
                            strSrcAddr = Frame.Mac.SrcAddr == null ?
                                        "" : Util.GetStringHexFromByte(Frame.Mac.SrcAddr, 0, Frame.Mac.SrcAddr.Length, "", true);
                            strDstAddr = Frame.Mac.DstAddr == null ?
                                        "" : Util.GetStringHexFromByte(Frame.Mac.DstAddr, 0, Frame.Mac.DstAddr.Length, "", true);
                            ProtoWireless_NiBoEr.GetFrameTypeAndColor(Frame, out strFrameType, out frameColor);
                        }
                        break;

                    case 0x02:
                        {
                            strLogType = "串口数据";
                            ProtoLocal_North.FrameFormat localFrame = ProtoLocal_North.ExplainRxPacket(logData);
                            strSrcAddr = localFrame.SrcAddr == null ?
                                         "" : Util.GetStringHexFromByte(localFrame.SrcAddr, 0, localFrame.SrcAddr.Length, "", true);
                            strDstAddr = localFrame.DstAddr == null ?
                                         "" : Util.GetStringHexFromByte(localFrame.DstAddr, 0, localFrame.DstAddr.Length, "", true);
                            ProtoLocal_North.GetFrameTypeAndColor(localFrame, out strFrameType, out frameColor);
                        }
                        break;

                    case 0x03:
                        {
                            strLogType = "调试数据";
                            frameColor = Color.Brown;
                        }
                        break;

                    default:
                        {
                            strLogType = "无法识别";
                            frameColor = Color.DarkGray;
                        }
                        break;
                }
                row["类别"] = strLogType;
                row["日志数据"] = logData;
                row["源地址"] = strSrcAddr;
                row["目的地址"] = strDstAddr;
                row["帧类型"] = strFrameType;
                row["备注"] = strComment;

                byte[] logFrame = new byte[logLength];
                Array.Copy(buf, logStartIndex, logFrame, 0, logFrame.Length);
                row["日志帧"] = logFrame;      // 保存整个日志帧 ( 包号 --> FCS )

                dtLog.Rows.Add(row);
                dgvLog.Rows[dgvLog.Rows.Count - 1].DefaultCellStyle.ForeColor = frameColor;
                dgvLog.FirstDisplayedScrollingRowIndex = dgvLog.RowCount - 1;

                if (strLogType == "串口数据")
                {
                    AddToStationData(dt, logData);
                }
            }

            // 自动保存
            if (true == _isLogAutoSave && false == _isLogLoading)
            {
                using (StreamWriter sw = new StreamWriter(DateTime.Now.ToString("yyyyMMdd") + "_Log_AutoSave.txt", true, Encoding.UTF8))
                {
                    string strLine = DateTime.Now.ToString("[ yyyy-MM-dd HH:mm:ss.fff ] ");
                    strLine += dtLog.Rows[dtLog.Rows.Count - 1]["序号"].ToString() + " ";
                    byte[] databuf = (byte[])dtLog.Rows[dtLog.Rows.Count - 1]["日志帧"];

                    foreach (byte data in databuf)
                    {
                        strLine += data.ToString("X2") + " ";
                    }

                    sw.WriteLine(strLine);
                }
            }

        }

        // 解析plccom.log
        private void ParsePlccomLog(string strLog)
        {
            DataRow row = dtLog.NewRow();
            Color frameColor = Color.Black;
            string strLogType = "", strDstAddr = "", strSrcAddr = "", strFrameType = "", strComment = "";
            DateTime dt;
            byte[] logData;

            dt = DateTime.Parse(strLog.Substring(0, "2018 01/22 07:38:27".Length));
            logData = Util.GetByteFromStringHex(strLog.Substring("2018 01/22 07:38:27 Send:".Length));

            strLogType = "串口数据";
            ProtoLocal_North.FrameFormat localFrame = ProtoLocal_North.ExplainRxPacket(logData);
            strSrcAddr = localFrame.SrcAddr == null ?
                         "" : Util.GetStringHexFromByte(localFrame.SrcAddr, 0, localFrame.SrcAddr.Length, "", true);
            strDstAddr = localFrame.DstAddr == null ?
                         "" : Util.GetStringHexFromByte(localFrame.DstAddr, 0, localFrame.DstAddr.Length, "", true);
            ProtoLocal_North.GetFrameTypeAndColor(localFrame, out strFrameType, out frameColor);

            row.BeginEdit();
            row["序号"] = (dtLog.Rows.Count + 1).ToString("D6");
            row["时间"] = dt.ToString("yyyy-MM-dd HH:mm:ss");
            row["类别"] = strLogType;
            row["日志数据"] = logData;
            row["源地址"] = strSrcAddr;
            row["目的地址"] = strDstAddr;
            row["帧类型"] = strFrameType;
            row.EndEdit();
            dtLog.Rows.Add(row);
            dgvLog.Rows[dgvLog.Rows.Count - 1].DefaultCellStyle.ForeColor = frameColor;
            //dgvLog.FirstDisplayedScrollingRowIndex = dgvLog.RowCount - 1;

            //AddToStationData(dt, logData);
        }

        // 解析plcrec.log
        private void ParsePlcrecLog(string str)
        {

        }


        //列表被选中
        private void dgvLog_SelectionChanged(object sender, EventArgs e)
        {
            if(dgvLog.SelectedRows.Count <= 0 )
            {
                return;
            }

            Log2Text(dgvLog.SelectedRows[0].Index);
            Log2Tree(dgvLog.SelectedRows[0].Index);
        }

        //添加到文本框
        private void Log2Text(int rowIndex)
        {
            byte[] buf = (byte[])dtLog.Rows[rowIndex]["日志数据"];
            rtbLogText.Clear();

            string strData = dtLog.Rows[rowIndex]["类别"] + "：" + Util.GetStringHexFromByte(buf, 0, buf.Length, " ");
            RichTextBoxAppand(rtbLogText, strData, Color.Blue);
        }

        private delegate void UpdateRichTextCallback(RichTextBox rtb, string msg, Color fgColor);
        private void RichTextBoxAppand(RichTextBox rtb, string str, Color foreColor)
        {
            if(this.InvokeRequired)
            {
                Invoke(new UpdateRichTextCallback(RichTextBoxAppand), new object[] { rtb, str, foreColor});
                return;
            }

            int iStart = rtb.Text.Length;
            rtb.AppendText(str);
            rtb.Select(iStart, rtb.Text.Length);
            rtb.SelectionColor = foreColor;
        }

        //添加到协议树
        private void Log2Tree(int rowIndex)
        {
            DataRow curRow = dtLog.Rows[rowIndex];
            byte[] databuf = (byte[])curRow["日志数据"];

            treeProtocol.BeginUpdate();
            treeProtocol.Nodes.Clear();

            // root--数据包
            TreeNode PacketNode = new TreeNode("数据包");

            // root--数据包--属性
            TreeNode AttrNode = new TreeNode("属性");
            {
                AttrNode.ForeColor = Color.Black;
                AttrNode.Nodes.Add("时间：" + curRow["时间"]);
                AttrNode.Nodes.Add("长度：" + databuf.Length);
                AttrNode.Nodes.Add("日志类型：" + curRow["类别"]);
                AttrNode.Expand();
            }
            PacketNode.Nodes.Add(AttrNode);

            // root--数据包--协议帧格式
            if ("无线数据" == curRow["类别"].ToString())
            {
                TreeNode wirelessTree = ProtoWireless_NiBoEr.GetProtoTree(databuf);
                foreach (TreeNode node in wirelessTree.Nodes)
                {
                    PacketNode.Nodes.Add(node);
                }
            }
            else if ("串口数据" == curRow["类别"].ToString())
            {
                TreeNode scomTree = ProtoLocal_North.GetProtoTree(databuf);
                foreach (TreeNode node in scomTree.Nodes)
                {
                    PacketNode.Nodes.Add(node);
                }
            }
            else  // 调试数据
            {
                //暂不解析
                TreeNode node = new TreeNode("暂不解析");
                PacketNode.Nodes.Add(node);
            }

            treeProtocol.Nodes.Add(PacketNode);
            PacketNode.Expand();

            treeProtocol.EndUpdate();
        }

        #endregion

        #region 台区分析

        public const byte MinSuccRate = 75;
        public const string NetInfoFileName = "NetInfo_AutoSave.txt";

        // 读取台区信息
        private void btReadNetInfo_Click(object sender, EventArgs e)
        {
            int index = 0, afnIndex = 0;
            byte crc;
            byte[] tmpBuf = new byte[128];
            Command sendCmd;

            treeNwkAnalysis.Nodes.Clear();
            _netInfo.Clear();
            _logFileName = NetInfoFileName;
            LogToFile(_logFileName, "读取台区信息-开始", false);

            tmpBuf[index++] = 0x68;         //起始符
            tmpBuf[index++] = 0;            //长度，累加后再设置
            tmpBuf[index++] = 0;
            tmpBuf[index++] = 0x4A;         //控制域
            tmpBuf[index++] = 0x00;         //信息域 6 byte
            tmpBuf[index++] = 0x00;
            tmpBuf[index++] = 0x00;
            tmpBuf[index++] = 0x00;
            tmpBuf[index++] = 0x00;
            tmpBuf[index++] = _FrameSn++;   // 帧序号
            afnIndex = index;

            // 读微功率无线从节点信息  AFN--0x10  Fn--F101
            {
                index = afnIndex;
                tmpBuf[index++] = 0x10;                     //AFN
                tmpBuf[index++] = (byte)(1 << (100 % 8));   //DT1 
                tmpBuf[index++] = (byte)(100 / 8);          //DT2
                crc = 0;
                for (int i = 3; i < index; i++)
                {
                    crc += tmpBuf[i];
                }
                tmpBuf[index++] = crc;                      // crc
                tmpBuf[index++] = 0x16;                     // 结束符

                tmpBuf[1] = (byte)(index & 0xFF);           // 长度
                tmpBuf[2] = (byte)((index >> 8) & 0xFF);

                sendCmd = GetCmdSendHandler("查询微功率无线从节点信息");
                sendCmd.TxBuf = new byte[index];
                Array.Copy(tmpBuf, sendCmd.TxBuf, sendCmd.TxBuf.Length);
                
                _sendQueue.Enqueue(sendCmd);
            }

            // 读台区概况            AFN--0x**  Fn--F**  多条指令组合
            {
                index = afnIndex;
                tmpBuf[index++] = 0x10;                     //AFN
                tmpBuf[index++] = (byte)(1 << (100 % 8));   //DT1 
                tmpBuf[index++] = (byte)(100 / 8);          //DT2
                crc = 0;
                for (int i = 3; i < index; i++)
                {
                    crc += tmpBuf[i];
                }
                tmpBuf[index++] = crc;                      // crc
                tmpBuf[index++] = 0x16;                     // 结束符

                tmpBuf[1] = (byte)(index & 0xFF);           // 长度
                tmpBuf[2] = (byte)((index >> 8) & 0xFF);

                sendCmd = GetCmdSendHandler("读台区概况");
                sendCmd.TxBuf = new byte[index];
                Array.Copy(tmpBuf, sendCmd.TxBuf, sendCmd.TxBuf.Length);

                sendCmd.Params.Add("AFN=03 , Fn=1 ,  查询厂商代码和版本信息");
                sendCmd.Params.Add("AFN=03 , Fn=4 ,  查询主节点地址");
                sendCmd.Params.Add("AFN=03 , Fn=8 ,  查询无线通信参数");
                sendCmd.Params.Add("AFN=03 , Fn=100, 查询场强门限");
                sendCmd.Params.Add("AFN=10 , Fn=4 ,  查询路由运行状态");
                sendCmd.Params.Add("AFN=F0 , Fn=40 , 读取子节点概要信息");

                _sendQueue.Enqueue(sendCmd);
            }

             // 读中心节点邻居表  AFN--0xF0  Fn--F50
            {
                index = afnIndex;
                tmpBuf[index++] = 0xF0;                    //AFN
                tmpBuf[index++] = (byte)(1 << (49 % 8));   //DT1 
                tmpBuf[index++] = (byte)(49 / 8);          //DT2

                tmpBuf[index++] = 0x00;                     // 当前页序号

                crc = 0;
                for (int i = 3; i < index; i++)
                {
                    crc += tmpBuf[i];
                }
                tmpBuf[index++] = crc;                      // crc
                tmpBuf[index++] = 0x16;                     // 结束符

                tmpBuf[1] = (byte)(index & 0xFF);           // 长度
                tmpBuf[2] = (byte)((index >> 8) & 0xFF);

                sendCmd = GetCmdSendHandler("读取中心节点邻居表");
                sendCmd.TxBuf = new byte[index];
                Array.Copy(tmpBuf, sendCmd.TxBuf, sendCmd.TxBuf.Length);
                sendCmd.Params.Add("0");    // 当前页序号

                _sendQueue.Enqueue(sendCmd);
            }

            // 读取子节点参数信息     AFN--0xF0  Fn--F30
            {
                index = afnIndex;
                tmpBuf[index++] = 0xF0;                    //AFN
                tmpBuf[index++] = (byte)(1 << (29 % 8));   //DT1 
                tmpBuf[index++] = (byte)(29 / 8);          //DT2

                tmpBuf[index++] = 0x00;                    // 0 -- 读档案信息  1 -- 读邻居表  2 -- 读路径表
                tmpBuf[index++] = 0x01;                    // 子节点地址，先随便填充
                tmpBuf[index++] = 0x02;
                tmpBuf[index++] = 0x03;
                tmpBuf[index++] = 0x04;
                tmpBuf[index++] = 0x05;
                tmpBuf[index++] = 0x06;
                crc = 0;
                for (int i = 3; i < index; i++)
                {
                    crc += tmpBuf[i];
                }
                tmpBuf[index++] = crc;                      // crc
                tmpBuf[index++] = 0x16;                     // 结束符

                tmpBuf[1] = (byte)(index & 0xFF);           // 长度
                tmpBuf[2] = (byte)((index >> 8) & 0xFF);

                // 00 读子节点档案信息
                {
                    tmpBuf[afnIndex + 3] = 0x00;                    // 0 -- 读档案信息
                    sendCmd = GetCmdSendHandler("读取子节点参数信息：档案信息");
                    sendCmd.TxBuf = new byte[index];
                    Array.Copy(tmpBuf, sendCmd.TxBuf, sendCmd.TxBuf.Length);
                    sendCmd.Params.Add("0");    // 档案起始序号

                    _sendQueue.Enqueue(sendCmd);
                }
                // 01 读子节点邻居表
                {
                    tmpBuf[afnIndex + 3] = 0x01;                    // 1 -- 读邻居表
                    sendCmd = GetCmdSendHandler("读取子节点参数信息：邻居表");
                    sendCmd.TxBuf = new byte[index];
                    Array.Copy(tmpBuf, sendCmd.TxBuf, sendCmd.TxBuf.Length);
                    sendCmd.Params.Add("0");    // 档案起始序号

                    _sendQueue.Enqueue(sendCmd);
                }
                // 02 读子节点路径表
                {
                    tmpBuf[afnIndex + 3] = 0x02;                    // 2 -- 读路径表
                    sendCmd = GetCmdSendHandler("读取子节点参数信息：路径表");
                    sendCmd.TxBuf = new byte[index];
                    Array.Copy(tmpBuf, sendCmd.TxBuf, sendCmd.TxBuf.Length);
                    sendCmd.Params.Add("0");    // 档案起始序号

                    _sendQueue.Enqueue(sendCmd);
                }
            }

            // 显示台区分析结果
            {
                sendCmd = new Command();                  
                sendCmd.Name = "显示台区分析结果";
                sendCmd.TimeWaitMS = 300;             
                sendCmd.RetryTimes = 1;
                sendCmd.SendFunc = ShowNwkAnalysisResultTreeView;
                sendCmd.RecvFunc = null;
                _sendQueue.Enqueue(sendCmd);
            }

            _IsSendNewCmd = true;
        }

        /// <summary>
        /// 读微功率无线从节点信息
        /// </summary>
        private void ReadMicroWirelessSubNodeInfoCmd(Command cmd)
        {
            if (cmd.Name.Contains("查询微功率无线从节点信息"))
            {
                serialPort_SendData(cmd.TxBuf);
            }
        }
        /// <summary>
        /// 读微功率无线从节点信息-响应
        /// </summary>
        private void ReadMicroWirelessSubNodeInfoResponse(Command cmd)
        {
            if (cmd.RxBuf[0] != 0x68 || cmd.RxBuf[cmd.RxBuf.Length - 1] != 0x16
                || (cmd.RxBuf[1] + (cmd.RxBuf[2] << 8)) != cmd.RxBuf.Length     // length
                || cmd.RxBuf[3] != 0x8A )        // ctrl word
            {
                _IsSendNewCmd = true;
                cmd.IsEnable = false;
                return;
            }

            ProtoLocal_North.FrameFormat frame = ProtoLocal_North.ExplainRxPacket(cmd.RxBuf);

            string FnName = ProtoLocal_North.ExplainFn(frame);
            TreeNode node = ProtoLocal_North.ExplainFrameData(frame);
            string strTmp = "";
                

            if (FnName.Contains("查询微功率无线从节点信息"))
            {
                SubNodeInfo nodeInfo;
                TreeNode curNode;
                for(int i = 2; i < node.Nodes.Count; i++)
                {
                    nodeInfo = new SubNodeInfo();
                    nodeInfo.DocInfo = new SubNodeInfo.DocumentInfo();
                    nodeInfo.NeighborTbl = new List<SubNodeInfo.NeighborInfo>();
                    nodeInfo.RouteTbl = new List<SubNodeInfo.RouteInfo>();

                    curNode = node.Nodes[i];

                    strTmp = curNode.Text;
                    nodeInfo.LongAddr = strTmp.Substring(strTmp.IndexOf("：") + 1);

                    strTmp = curNode.Nodes[0].Text;
                    nodeInfo.RelayLevel = Convert.ToByte(strTmp.Substring(strTmp.IndexOf("：") + 1));

                    strTmp = curNode.Nodes[1].Text;
                    nodeInfo.SignalQuality = strTmp.Substring(strTmp.IndexOf("：") + 1);

                    strTmp = curNode.Nodes[2].Text;
                    nodeInfo.Phase123 = strTmp.Substring(strTmp.IndexOf("：") + 1);

                    strTmp = curNode.Nodes[3].Text;
                    nodeInfo.ProtoType = strTmp.Substring(strTmp.IndexOf("：") + 1);

                    strTmp = curNode.Nodes[4].Text;
                    nodeInfo.UpgradeFlag = strTmp.Substring(strTmp.IndexOf("：") + 1);

                    strTmp = curNode.Nodes[5].Text;
                    nodeInfo.AppVer = strTmp.Substring(strTmp.IndexOf("：") + 1);

                    strTmp = curNode.Nodes[6].Text;
                    nodeInfo.LoaderVer = strTmp.Substring(strTmp.IndexOf("：") + 1);

                    if (string.IsNullOrEmpty(_netInfo.SummaryInfo.SubNodeVer))
                    {
                        _netInfo.SummaryInfo.SubNodeVer = nodeInfo.AppVer;
                    }
                    else if (nodeInfo.AppVer != "0.00" 
                        && _netInfo.SummaryInfo.SubNodeVer.Contains(nodeInfo.AppVer) == false)
                    {
                        _netInfo.SummaryInfo.SubNodeVer += " , " + nodeInfo.AppVer;
                    }

                    var rs = _netInfo.NodeDocuments.FirstOrDefault( q => q.Key == nodeInfo.LongAddr);
                    if (rs.Value == null)
                    {
                        _netInfo.NodeDocuments.Add(nodeInfo.LongAddr, nodeInfo);
                        _netInfo.NwkNodes[nodeInfo.RelayLevel & 0x07].Add(nodeInfo);
                    }
                }
            }

            if (cmd.Name.Contains("查询微功率无线从节点信息"))
            {
                cmd.RetryTimes = 0;     
                cmd.TimeWaitMS += 2000;      // 继续等待,可能后续有多包应答
            }
        }

        /// <summary>
        /// 读台区概况
        /// </summary>
        private void ReadNetSummeryInfoCmd(Command cmd)
        {
            if (cmd.Name.Contains("读台区概况"))
            {
                if (cmd.Params.Count <= 0)
                {
                    return;
                }

                string[] strParams = cmd.Params[0].Replace(" ", "").Split(',');
                byte afn = Convert.ToByte(strParams[0].Substring(strParams[0].IndexOf("=") + 1), 16);
                byte fn = Convert.ToByte(strParams[1].Substring(strParams[1].IndexOf("=") + 1));
                cmd.Name = "读台区概况：" + strParams[2];

                int index = 10;
                cmd.TxBuf[index++] = afn;                           //AFN
                cmd.TxBuf[index++] = (byte)(1 << ((fn - 1) % 8));    //DT1 
                cmd.TxBuf[index++] = (byte)((fn - 1) / 8);          //DT2
                byte crc = 0;
                for (int i = 3; i < index; i++)
                {
                    crc += cmd.TxBuf[i];
                }
                cmd.TxBuf[index++] = crc;                           // crc
                cmd.TxBuf[index++] = 0x16;                          // 结束符

                serialPort_SendData(cmd.TxBuf);
            }
        }
        /// <summary>
        /// 读台区概况-响应
        /// </summary>
        private void ReadNetSummeryInfoResponse(Command cmd)
        {
            if (cmd.RxBuf[0] != 0x68 || cmd.RxBuf[cmd.RxBuf.Length - 1] != 0x16
                || (cmd.RxBuf[1] + (cmd.RxBuf[2] << 8)) != cmd.RxBuf.Length     // length
                || cmd.RxBuf[3] != 0x8A)         // ctrl word
            {
                _IsSendNewCmd = true;
                cmd.IsEnable = false;
                return;
            }

            ProtoLocal_North.FrameFormat frame = ProtoLocal_North.ExplainRxPacket(cmd.RxBuf);

            string FnName = ProtoLocal_North.ExplainFn(frame);
            TreeNode node = ProtoLocal_North.ExplainFrameData(frame);
            string strTmp = "";

            if (FnName.Contains("查询厂商代码和版本信息") && node.Nodes.Count >= 4)
            {
                strTmp = node.Nodes[2].Text;
                _netInfo.SummaryInfo.ReleaseDate = strTmp.Substring(strTmp.IndexOf("：") + 1);
                strTmp = node.Nodes[3].Text;
                _netInfo.SummaryInfo.MainNodeVer = strTmp.Substring(strTmp.IndexOf("：") + 1);
            }
            else if (FnName.Contains("查询主节点地址") && node.Nodes.Count >= 1)
            {
                strTmp = node.Nodes[0].Text;
                _netInfo.SummaryInfo.ConcAddr = strTmp.Substring(strTmp.IndexOf("：") + 1);
            }
            else if (FnName.Contains("查询无线通信参数") && node.Nodes.Count >= 1)
            {
                strTmp = node.Nodes[0].Text;
                _netInfo.SummaryInfo.ChanelGrp = strTmp.Substring(strTmp.IndexOf("：") + 1);
            }
            else if (FnName.Contains("查询场强门限") && node.Nodes.Count >= 1)
            {
                strTmp = node.Nodes[0].Text;
                _netInfo.SummaryInfo.RssiThreshold = strTmp.Substring(strTmp.IndexOf("：") + 1);
            }
            else if (FnName.Contains("查询路由运行状态") && node.Nodes.Count >= 6)
            {
                int totalCnt, onlineCnt;

                strTmp = node.Nodes[0].Text;
                strTmp = strTmp.Substring(strTmp.IndexOf("路由学习标志：") + 7);
                _netInfo.SummaryInfo.IsBuildNwkFinished = strTmp;

                strTmp = node.Nodes[4].Text;
                strTmp = strTmp.Substring(strTmp.IndexOf("从节点总数量：") + 7);
                _netInfo.SummaryInfo.DocumentCnt = strTmp;
                totalCnt = Convert.ToInt32(strTmp);

                strTmp = node.Nodes[5].Text;
                strTmp = strTmp.Substring(strTmp.IndexOf("入网节点数量：") + 7);
                _netInfo.SummaryInfo.BuildNwkSuccessCnt = strTmp;
                onlineCnt = Convert.ToInt32(strTmp);

                _netInfo.SummaryInfo.BuildNwkFailureCnt = (totalCnt - onlineCnt).ToString();

                _netInfo.SummaryInfo.BuildNwkSuccessRate = ((float)onlineCnt * 100 / totalCnt).ToString("F2") + "%";
            }
            else if (FnName.Contains("读取子节点概要信息") && node.Nodes.Count >= 10)
            {
                strTmp = node.Nodes[1].Text;
                strTmp = strTmp.Substring(strTmp.IndexOf("发现数量：") + 5);
                _netInfo.SummaryInfo.DiscovedNodeCnt = strTmp;

                strTmp = node.Nodes[2].Text;
                strTmp = strTmp.Substring(strTmp.IndexOf("点名数量：") + 5);
                _netInfo.SummaryInfo.NamedNodeCnt = strTmp;

                strTmp = node.Nodes[3].Text;
                strTmp = strTmp.Substring(strTmp.IndexOf("配置数量：") + 5);
                _netInfo.SummaryInfo.ConfigedNodeCnt = strTmp;

                strTmp = node.Nodes[4].Text;
                strTmp = strTmp.Substring(strTmp.IndexOf("维护数量：") + 5);
                _netInfo.SummaryInfo.MaintainedNodeCnt = strTmp;

                strTmp = node.Nodes[5].Text;
                strTmp = strTmp.Substring(strTmp.IndexOf("中心邻居数量：") + 7);
                _netInfo.SummaryInfo.MainNodeNeighborCnt = strTmp;

                strTmp = node.Nodes[6].Text;
                strTmp = strTmp.Substring(strTmp.IndexOf("组网时间：") + 5);
                _netInfo.SummaryInfo.BuildNwkUsedTime = strTmp;

                strTmp = node.Nodes[7].Text;
                strTmp = strTmp.Substring(strTmp.IndexOf("工作信道组：") + 6);
                _netInfo.SummaryInfo.ChanelGrp = strTmp;

                strTmp = node.Nodes[8].Text;
                strTmp = strTmp.Substring(strTmp.IndexOf("广播维护开关：") + 7);
                _netInfo.SummaryInfo.BroadcastMaintainFlag = strTmp;

                strTmp = node.Nodes[9].Text;
                strTmp = strTmp.Substring(strTmp.IndexOf("广播维护时间：") + 7);
                _netInfo.SummaryInfo.BroadcastMaintainTime = strTmp;
            }
            else
            {
                strTmp = "无效应答";
                // do nothing 
            }

            if (cmd.Name.Contains("读台区概况"))
            {
                cmd.Params.RemoveAt(0);

                if (cmd.Params.Count > 0 && strTmp != "无效应答")
                {
                    cmd.RetryTimes = 3;
                }
                else
                {
                    cmd.RetryTimes = 0;     // next cmd
                }
            }
        }

        /// <summary>
        /// 读取中心节点邻居表
        /// </summary>
        private void ReadMainNodeNeighborTblCmd(Command cmd)
        {
            if (cmd.Name.Contains("读取中心节点邻居表"))
            {
                int index = 13;
                cmd.TxBuf[index++] = Convert.ToByte(cmd.Params[0]);     // 当前页序号
                byte crc = 0;
                for (int i = 3; i < index; i++)
                {
                    crc += cmd.TxBuf[i];
                }
                cmd.TxBuf[index++] = crc;                           // crc
                cmd.TxBuf[index++] = 0x16;                          // 结束符

                serialPort_SendData(cmd.TxBuf);
            }
        }
        /// <summary>
        /// 读取中心节点邻居表-响应
        /// </summary>
        private void ReadMainNodeNeighborTblResponse(Command cmd)
        {
            if (cmd.RxBuf[0] != 0x68 || cmd.RxBuf[cmd.RxBuf.Length - 1] != 0x16
                || (cmd.RxBuf[1] + (cmd.RxBuf[2] << 8)) != cmd.RxBuf.Length     // length
                || cmd.RxBuf[3] != 0x8A)        // ctrl word
            {
                _IsSendNewCmd = true;
                cmd.IsEnable = false;
                return;
            }

            ProtoLocal_North.FrameFormat frame = ProtoLocal_North.ExplainRxPacket(cmd.RxBuf);

            string FnName = ProtoLocal_North.ExplainFn(frame);
            TreeNode node = ProtoLocal_North.ExplainFrameData(frame);

            if (FnName.Contains("读取中心节点邻居表") && node.Nodes.Count >= 3)
            {
                TreeNode neighbors = node.Nodes[2];
                string addr;

                foreach (TreeNode nbNode in neighbors.Nodes)
                {
                    addr = nbNode.Text.Substring(nbNode.Text.IndexOf("：") + 1, 12);

                    if (_netInfo.LocalNwkNodes.Contains(addr) == false)
                    {
                        _netInfo.LocalNwkNodes.Add(addr);
                    }

                    if (_netInfo.MainNodeNeighbors.Contains(addr) == false)
                    {
                        _netInfo.MainNodeNeighbors.Add(nbNode.Text.Substring(nbNode.Text.IndexOf("：") + 1));
                    }
                }
            }
            else
            {
                // do nothing 
            }

            if (cmd.Name.Contains("读取中心节点邻居表") && FnName.Contains("读取中心节点邻居表"))
            {
                byte totalPage = frame.DataBuf[0];
                byte currPage = frame.DataBuf[1];

                if(currPage < totalPage -1)
                {
                    cmd.Params[0] = (currPage + 1).ToString();
                    cmd.RetryTimes = 3; 
                }
                else
                {
                    cmd.RetryTimes = 0;     // next cmd
                }
            }
        }
       
        /// <summary>
        /// 读取子节点参数信息
        /// </summary>
        private void ReadSubNodeParamsInfoCmd(Command cmd)
        {
            if (cmd.Name.Contains("读取子节点参数信息"))
            {
                UInt16 docNo = Convert.ToUInt16(cmd.Params[0]);

                if(docNo > _netInfo.NodeDocuments.Count -1)
                {
                    cmd.RetryTimes = 0;
                    return;
                }

                string addr = _netInfo.NodeDocuments.ElementAt(docNo).Key;

                int index = 14;
                for (int i = 0; i < 6; i++ )                        // 子节点地址
                {
                    cmd.TxBuf[index++] = Convert.ToByte(addr.Substring(10 - i * 2, 2), 16); 
                }
                byte crc = 0;
                for (int i = 3; i < index; i++)
                {
                    crc += cmd.TxBuf[i];
                }
                cmd.TxBuf[index++] = crc;                           // crc
                cmd.TxBuf[index++] = 0x16;                          // 结束符

                serialPort_SendData(cmd.TxBuf);
            }
        }
        /// <summary>
        /// 读取子节点参数信息-响应
        /// </summary>
        private void ReadSubNodeParamsInfoResponse(Command cmd)
        {
            if (cmd.RxBuf[0] != 0x68 || cmd.RxBuf[cmd.RxBuf.Length - 1] != 0x16
                || (cmd.RxBuf[1] + (cmd.RxBuf[2] << 8)) != cmd.RxBuf.Length     // length
                || cmd.RxBuf[3] != 0x8A )        // ctrl word
            {
                _IsSendNewCmd = true;
                cmd.IsEnable = false;
                return;
            }

            ProtoLocal_North.FrameFormat frame = ProtoLocal_North.ExplainRxPacket(cmd.RxBuf);

            string FnName = ProtoLocal_North.ExplainFn(frame);
            TreeNode treeNode = ProtoLocal_North.ExplainFrameData(frame);
            string nodeAddr = treeNode.Nodes[1].Text.Substring("节点地址：".Length);

            treeNode = treeNode.Nodes.Count == 3 ? treeNode.Nodes[2] : null;
            SubNodeInfo nodeInfo = _netInfo.NodeDocuments[nodeAddr];

            if (FnName.Contains("读取子节点参数信息") && treeNode != null )
            {
                if (treeNode.Text.Contains("档案信息"))
                {
                    // 读子节点档案信息-响应
                    byte[] info = new byte[22];
                    Array.Copy(frame.DataBuf, 7, info, 0, info.Length); // 跳过 类型1 + 地址6
                    nodeInfo.DocInfo.Parse(info);
                    nodeInfo.treeDocInfo = treeNode;

                    if (nodeInfo.DocInfo.ReadAmeterSuccessRate < MinSuccRate)
                    {
                        _netInfo.LowSuccessRateNodes.Add(nodeInfo);     // 抄表成功率低节点
                    }

                    // 原因分析：节点是否发现
                    if (nodeInfo.DocInfo.Flg_Find == 0)
                    {
                        nodeInfo.BadReason = " 1.节点未发现";
                        _netInfo.OfflineNodes.Add(nodeInfo);        // 离网节点
                    }
                }
                else if (treeNode.Text.Contains("邻居个数"))
                {
                    // 读子节点邻居表-响应
                    SubNodeInfo.NeighborInfo neighbor;
                    byte cnt = frame.DataBuf[7];
                    byte[] data;
                    for(int i = 0; i < cnt; i++)
                    {
                        neighbor = new SubNodeInfo.NeighborInfo();
                        data = new byte[4];
                        Array.Copy(frame.DataBuf, (8 + i*4 ), data, 0, data.Length);
                        neighbor.Parse(data);
                        nodeInfo.NeighborTbl.Add(neighbor);
                    }

                    // 索引替换成长地址
                    string addr, strTmp;
                    int docIdx;
                    SubNodeInfo nbNodeInfo;
                    foreach (TreeNode nbNode in treeNode.Nodes)
                    {
                        strTmp = nbNode.Text;    // 例如：邻居xx： 索引 xx (上行 98 , 下行 99)
                        docIdx = Convert.ToInt32(strTmp.Split(' ')[2]);

                        if (docIdx < _netInfo.NodeDocuments.Count)
                        {
                            nbNodeInfo = _netInfo.NodeDocuments.ElementAt(docIdx).Value;
                            addr = nbNodeInfo.LongAddr;
                            if (_netInfo.LocalNwkNodes.Contains(addr) == false)
                            {
                                _netInfo.LocalNwkNodes.Add(addr);
                            }
                            strTmp = strTmp.Split(' ')[0] + addr + strTmp.Substring(strTmp.IndexOf(" (上"))
                                    + " " + (nbNodeInfo.RelayLevel == 0 ? "离网" : (nbNodeInfo.RelayLevel + "级"));
                        }
                        else
                        {
                            strTmp = strTmp.Split(' ')[0] + "中心节点地址" + strTmp.Substring(strTmp.IndexOf(" (上"));
                        }
                        nbNode.Text = strTmp;
                    }
                    nodeInfo.treeNeighbors = treeNode;

                    // 原因分析：邻居场强是否超标
                    byte rssiBadCnt = 0;
                    byte maxRssi = Convert.ToByte(_netInfo.SummaryInfo.RssiThreshold);
                    foreach (SubNodeInfo.NeighborInfo nb in nodeInfo.NeighborTbl)
                    {
                        if ((nb.UpRssi != 255 && nb.UpRssi > maxRssi)
                            || (nb.DownRssi != 255 && nb.DownRssi > maxRssi))
                        {
                            rssiBadCnt++;
                        }
                    }

                    if (rssiBadCnt == 0)
                    {
                        // do nothing 
                    }
                    else if (rssiBadCnt < nodeInfo.NeighborTbl.Count && rssiBadCnt > 0)
                    {
                        nodeInfo.BadReason += " [2]部分邻居场强超标";
                    }
                    else
                    {
                        nodeInfo.BadReason += " [2]全部邻居场强超标";
                        _netInfo.AloneNodes.Add(nodeInfo);          // 孤立节点
                    }  
                }
                else if (treeNode.Text.Contains("路径条数"))
                {
                    // 读子节点路径表-响应
                    byte cnt = frame.DataBuf[7];
                    SubNodeInfo.RouteInfo route;
                    byte[] data;
                    for (int i = 0; i < cnt; i++)
                    {
                        route = new SubNodeInfo.RouteInfo();
                        data = new byte[20];
                        Array.Copy(frame.DataBuf, (8 + i * 20), data, 0, data.Length);
                        route.Parse(data);
                        nodeInfo.RouteTbl.Add(route);
                    }

                    // 短地址替换成长地址
                    string addr, strTmp;
                    string[] strs;
                    int jumps, docIdx;
                    foreach (TreeNode rtNode in treeNode.Nodes)
                    {
                        strTmp = rtNode.Text;   // 例如：路径xx [xx跳]：->[1]->[5]->[12]->中心
                        strs = strTmp.Split('[');
                        jumps = strs.Length -1;

                        strTmp = strs[0] + "[" + jumps + "跳]：-> ";
                        for (int i = 0; i < jumps -1; i++ )
                        {
                            docIdx = Convert.ToInt32(strs[i + 2].Substring(0, strs[i + 2].IndexOf("]")));
                            addr = _netInfo.NodeDocuments.ElementAt(docIdx).Value.LongAddr;
                            strTmp += addr + "-> ";
                        }
                        strTmp += "中心";
                        rtNode.Text = strTmp;
                    }
                    nodeInfo.treeRoutes = treeNode;

                    // 原因分析：路径场强是否超标
                    foreach (SubNodeInfo.RouteInfo rt in nodeInfo.RouteTbl)
                    {
                        if (rt.Flg_RssiBeyond == 1)
                        {
                            nodeInfo.BadReason += " [3]路径中场强超标";
                        }
                    }
                }
                else
                {
                    // do nothing
                }
            }
            else
            {
                // do nothing 
            }


            if (cmd.Name.Contains("读取子节点参数信息"))
            {
                ushort docNo = Convert.ToUInt16(cmd.Params[0]);
                if (docNo < _netInfo.NodeDocuments.Count - 1)
                {
                    // 读下一个节点
                    cmd.Params[0] = (docNo + 1).ToString();
                    cmd.RetryTimes = 3;
                }
                else
                {
                    cmd.RetryTimes = 0;     // next cmd
                }
            }
        }

        // 保存组网信息
        private void btSaveNetInfo_Click(object sender, EventArgs e)
        {
            string strDirectory;
            string strFileName;

            if (0 == treeNwkAnalysis.Nodes.Count)
            {
                MessageBox.Show("没有可保存的组网信息!");
                return;
            }

            strDirectory = XmlHelper.GetNodeDefValue(_configPath, "/Config/LogPath", Application.StartupPath);
            saveFileDlg.Filter = "*.txt(文本文件)|*.txt";
            saveFileDlg.DefaultExt = "txt";
            saveFileDlg.FileName = "";
            saveFileDlg.ShowDialog();

            strFileName = saveFileDlg.FileName;
            if (strFileName.Length == 0)
            {
                return;
            }

            if (strDirectory != Path.GetDirectoryName(strFileName))
            {
                strDirectory = Path.GetDirectoryName(strFileName);
                XmlHelper.SetNodeValue(_configPath, "/Config", "LogPath", strDirectory);
            }

            // 保存分析结果
            _netInfo.SaveNwkAnalysisInfo(strFileName);

            // 保存通信数据
            StreamWriter sw = new StreamWriter(strFileName, true , Encoding.UTF8);
            sw.WriteLine("【通信记录】\r\n");
            string strRead;
            StreamReader sr = new StreamReader(NetInfoFileName, Encoding.UTF8);
            while ((strRead = sr.ReadLine()) != null)
            {
                sw.WriteLine(strRead);
            }
            sr.Close();
            sw.Close();

            ShowStatus("保存成功！", Color.Green);
        }

        // 导入组网信息
        private void btLoadNetInfo_Click(object sender, EventArgs e)
        {
            string strDirectory, strFileName, strRead;

            _isLogLoading = true;

            strDirectory = XmlHelper.GetNodeDefValue(_configPath, "/Config/LogPath", Application.StartupPath);
            openFileDlg.InitialDirectory = strDirectory;
            openFileDlg.Filter = "*.TXT(文本文件)|*.TXT";
            openFileDlg.DefaultExt = "TXT";
            openFileDlg.FileName = "";
            if (DialogResult.OK != openFileDlg.ShowDialog())
            {
                _isLogLoading = false;
                return;
            }

            strFileName = openFileDlg.FileName;
            if (strFileName.Length == 0)
            {
                _isLogLoading = false;
                return;
            }

            if (strDirectory != Path.GetDirectoryName(strFileName))
            {
                strDirectory = Path.GetDirectoryName(strFileName);
                XmlHelper.SetNodeValue(_configPath, "/Config", "LogPath", strDirectory);
            }

            StreamReader sr = new StreamReader(strFileName, Encoding.UTF8);

            treeNwkAnalysis.Nodes.Clear();
            _netInfo.Clear();

            string[] strSplit;
            byte[] bytes;
            Command cmd = new Command();
            
            while ((strRead = sr.ReadLine()) != null)
            {
                try
                {
                    // 文本日志格式：[ 2018-07-20 09:43:29.864 ] 68 0F 00 4A 00 00 00 00 00 00 10 10 0C 76 16 
                    if (!strRead.StartsWith("[ ") || strRead.Contains("读取台区信息"))
                    {
                        continue;
                    }
                    strRead = strRead.Substring(strRead.IndexOf(']') + 1).Trim();
                    strSplit = strRead.Split(' ');
                    bytes = new byte[strSplit.Length];
                    for (int iLoop = 0; iLoop < strSplit.Length; iLoop++)
                    {
                        bytes[iLoop] = Convert.ToByte(strSplit[iLoop], 16);
                    }

                    if(bytes[0] == 0x68 && bytes[bytes.Length -1] == 0x16)
                    {
                        cmd.Name = "";
                        cmd.RxBuf = bytes;
                        ExplainCmdRecvHandler(cmd); // 解析数据
                    }
                }
                catch (Exception ex)
                {
                    ShowStatus("载入日志异常：" + ex.Message + "\r\n" + strRead, Color.Red);
                }
            }
            sr.Close();

            UpdateNwkAnalysisTreeView();            // 显示分析结果
        }

        // 显示台区分析结果
        delegate void Invoke_UpdateUI();

        private void UpdateNwkAnalysisTreeView()
        {
            if (this.InvokeRequired)
            {
                Invoke(new Invoke_UpdateUI(UpdateNwkAnalysisTreeView));
                return;
            }

            treeNwkAnalysis.BeginUpdate();
            treeNwkAnalysis.Nodes.Clear();

            TreeNode tNode = _netInfo.GetNwkAnalysisTree();

            foreach(TreeNode node in tNode.Nodes)
            {
                treeNwkAnalysis.Nodes.Add(node);
            }
            treeNwkAnalysis.EndUpdate();
        }
        private void ShowNwkAnalysisResultTreeView(Command cmd)
        {
            if (cmd.Name.Contains("显示台区分析结果"))
            {
                UpdateNwkAnalysisTreeView();
                LogToFile(_logFileName, "读取台区信息-结束");
                _logFileName = "";

                _IsSendNewCmd = true;      // next new cmd
                cmd.IsEnable = false;
            }
        }

        #endregion 

        #region 台区数据
        private void AddToStationData(DateTime dt, byte[] comData)
        {
            if (comData[0] != 0x68 || comData[comData.Length - 1] != 0x16
               || (comData[1] + (comData[2] << 8)) != comData.Length     // length
                ) 
            {
                return;
            }

            ProtoLocal_North.FrameFormat frame = ProtoLocal_North.ExplainRxPacket(comData);

            string FnName = ProtoLocal_North.ExplainFn(frame);
            TreeNode node = ProtoLocal_North.ExplainFrameData(frame);
            string strTmp = "";
            BuildNwkInfo nwkInfo;
            SubNodeInfo nodeInfo;



            if (_dailyData.Date == "")
            {
                if (FnName.Equals("启动组网") && dt.Hour >= 21)
                {
                    _dailyData.Date = dt.AddDays(1).ToString("yyyy-MM-dd");
                    nwkInfo = new BuildNwkInfo();
                    nwkInfo.BuildNwkStartTime = dt.ToString("yyyy-MM-dd HH:mm");
                    _dailyData.BuildNwkList.Add(nwkInfo);
                    _stationData = new StationData();
                    _stationData.Add(_dailyData);
                    _stationList.Add(_stationData);
                }

                return;
            }
            else if (dt.Day < DateTime.Parse(_dailyData.Date).Day - 1) // 导入的是另一个台区数据
            {
                _dailyData = new DailyData();
                return;
            }

            if (_stationData == null) return;

            if (FnName.Equals("启动组网"))
            {
                if (dt.Hour >= 21 && dt.Day >= DateTime.Parse(_dailyData.Date).Day)
                {
                    _dailyData = new DailyData();
                    _dailyData.Date = dt.AddDays(1).ToString("yyyy-MM-dd");
                    _stationData.Add(_dailyData);
                }

                nwkInfo = new BuildNwkInfo();
                nwkInfo.BuildNwkStartTime = dt.ToString("yyyy-MM-dd HH:mm");
                _dailyData.BuildNwkList.Add(nwkInfo);
            }
            else if (FnName.Equals("查询路由运行状态-应答"))
            {
                strTmp = node.Nodes[0].Text;

                nwkInfo = _dailyData.BuildNwkList.ElementAt(_dailyData.BuildNwkList.Count - 1);
                nwkInfo.IsBuildNwkFinished = strTmp.Substring(strTmp.IndexOf("路由学习标志：") + 7);
                if (nwkInfo.IsBuildNwkFinished == "已完成")
                {
                    nwkInfo.BuildNwkUsedTime = (dt - DateTime.Parse(nwkInfo.BuildNwkStartTime)).TotalMinutes.ToString();
                }
            }
            else if (FnName.Equals("查询微功率无线从节点信息-应答"))
            {
                TreeNode curNode;
                for (int i = 2; i < node.Nodes.Count; i++)
                {
                    curNode = node.Nodes[i];

                    nodeInfo = new SubNodeInfo();
                    strTmp = curNode.Text;
                    nodeInfo.LongAddr = strTmp.Substring(strTmp.IndexOf("：") + 1);

                    strTmp = curNode.Nodes[0].Text;
                    nodeInfo.RelayLevel = Convert.ToByte(strTmp.Substring(strTmp.IndexOf("：") + 1));
                    strTmp = curNode.Nodes[1].Text;
                    nodeInfo.SignalQuality = strTmp.Substring(strTmp.IndexOf("：") + 1);
                    strTmp = curNode.Nodes[2].Text;
                    nodeInfo.Phase123 = strTmp.Substring(strTmp.IndexOf("：") + 1);
                    strTmp = curNode.Nodes[3].Text;
                    nodeInfo.ProtoType = strTmp.Substring(strTmp.IndexOf("：") + 1);
                    strTmp = curNode.Nodes[4].Text;
                    nodeInfo.UpgradeFlag = strTmp.Substring(strTmp.IndexOf("：") + 1);
                    strTmp = curNode.Nodes[5].Text;
                    nodeInfo.AppVer = strTmp.Substring(strTmp.IndexOf("：") + 1);
                    strTmp = curNode.Nodes[6].Text;
                    nodeInfo.LoaderVer = strTmp.Substring(strTmp.IndexOf("：") + 1);

                    nwkInfo = _dailyData.BuildNwkList.ElementAt(_dailyData.BuildNwkList.Count - 1);
                    var rs = nwkInfo.NodeDocuments.FirstOrDefault(q => q.Key == nodeInfo.LongAddr);
                    if (rs.Value == null)
                    {
                        nwkInfo.NodeDocuments.Add(nodeInfo.LongAddr, nodeInfo);

                        if(nodeInfo.ProtoType.Contains("电表"))
                        {
                            nwkInfo.AmeterDocuments.Add(nodeInfo);
                            nwkInfo.AmeterNwkNodes[nodeInfo.RelayLevel & 0x07].Add(nodeInfo);
                            if(nodeInfo.RelayLevel == 0)
                            {
                                nwkInfo.AmeterOfflineNodes.Add(nodeInfo);
                            }
                            _dailyData.ReadFailedAmeters.Add(nodeInfo);
                        }
                        else if (nodeInfo.ProtoType == "双向水表")
                        {
                            nwkInfo.WaterDocuments.Add(nodeInfo);
                            nwkInfo.WaterNwkNodes[nodeInfo.RelayLevel & 0x07].Add(nodeInfo);
                            if (nodeInfo.RelayLevel == 0)
                            {
                                nwkInfo.WaterOfflineNodes.Add(nodeInfo);
                            }
                            _dailyData.ReadFailedWaters.Add(nodeInfo);
                        }
                        else if (nodeInfo.ProtoType == "单向水表")
                        {
                            nwkInfo.WaterDocuments.Add(nodeInfo);
                            nwkInfo.WaterOfflineNodes.Add(nodeInfo);
                            _dailyData.ReadFailedWaters.Add(nodeInfo);
                        }
                    }
                }
            }
            else if (FnName.Equals("查询无线通信参数-应答") && node.Nodes.Count >= 1)
            {
                strTmp = node.Nodes[0].Text;
                nwkInfo = _dailyData.BuildNwkList.ElementAt(_dailyData.BuildNwkList.Count - 1);
                nwkInfo.WorkChanelGrp = strTmp.Substring(strTmp.IndexOf("：") + 1);
            }
            else if (FnName.Equals("路由数据转发-应答"))
            {
                _dailyData.CenterAddr = Util.GetStringHexFromByte(frame.DstAddr, 0, 6, "", true);

                for(int i = 0; i < node.Nodes.Count ; i++)
                {
                    if(node.Nodes[i].Text.Contains("07报文"))
                    {
                        if (node.Nodes[i].Nodes[3].Text.Contains("正向电能"))
                        {
                            strTmp = node.Nodes[i].Nodes[4].Text;
                            nwkInfo = _dailyData.BuildNwkList.ElementAt(_dailyData.BuildNwkList.Count - 1);
                            nodeInfo = nwkInfo.NodeDocuments[Util.GetStringHexFromByte(frame.SrcAddr, 0, 6, "", true)];
                            nodeInfo.AmeterData.DayValue = strTmp.Substring(strTmp.IndexOf("：") + 1);
                            nodeInfo.AmeterData.ReadTime = dt.ToString("yyyy-MM-dd HH:mm:ss");
                            _dailyData.AddAmeterData(nodeInfo);
                            _dailyData.ReadFailedAmeters.Remove(nodeInfo);
                        }
                        break;
                    }
                }

                if (_stationData.CenterAddr == "")
                {
                    var rs = _stationList.FirstOrDefault(q => q.CenterAddr == _dailyData.CenterAddr);
                    if(rs != null)
                    {
                        _stationList.Remove(_stationData);
                        _stationData = null;
                    }
                    else
                    {
                        _stationData.CenterAddr = _dailyData.CenterAddr;
                    }
                }
                else if (_stationData.CenterAddr != _dailyData.CenterAddr)
                {
                    _stationData.Remove(_dailyData);
                    _stationData = new StationData();
                    _stationData.CenterAddr = _dailyData.CenterAddr;
                    _stationData.Add(_dailyData);
                    _stationList.Add(_stationData);
                }

            }
            else if (FnName.Equals("主节点上报水表数据"))
            {
                TreeNode curNode;
                for (int i = 2; i < node.Nodes.Count; i++)
                {
                    curNode = node.Nodes[i];
                    strTmp = curNode.Text.Substring(curNode.Text.IndexOf("：") + 1);
                    nwkInfo = _dailyData.BuildNwkList.ElementAt(_dailyData.BuildNwkList.Count - 1);
                    var rs = nwkInfo.NodeDocuments.FirstOrDefault(q => q.Key == strTmp.Substring(2));
                    if(rs.Value == null)
                    {
                        return;
                    }
                    nodeInfo = rs.Value;
                    nodeInfo.WaterData.DeviceAddr = strTmp;

                    strTmp = curNode.Nodes[0].Text;
                    nodeInfo.WaterData.MeterTime = strTmp.Substring(strTmp.IndexOf("：") + 1);
                    strTmp = curNode.Nodes[1].Text;
                    nodeInfo.WaterData.MeterType = strTmp.Substring(strTmp.IndexOf("：") + 1);
                    strTmp = curNode.Nodes[2].Text;
                    nodeInfo.WaterData.CurrValue = strTmp.Substring(strTmp.IndexOf("：") + 1);
                    strTmp = curNode.Nodes[3].Text;
                    nodeInfo.WaterData.CalcDays = strTmp.Substring(strTmp.IndexOf("：") + 1);
                    strTmp = curNode.Nodes[4].Text;
                    nodeInfo.WaterData.LastValue = strTmp.Substring(strTmp.IndexOf("：") + 1);
                    strTmp = curNode.Nodes[5].Text;
                    nodeInfo.WaterData.MeterStatus = strTmp.Substring(strTmp.IndexOf("：") + 1);
                    strTmp = curNode.Nodes[6].Text;
                    nodeInfo.WaterData.ProductYear = strTmp.Substring(strTmp.IndexOf("：") + 1);

                    _dailyData.AddWaterData(nodeInfo);
                    _dailyData.ReadFailedWaters.Remove(nodeInfo);
                }
            }

        }

        #endregion

        #region 状态栏显示

        private delegate void ShowMsgCallback(string msg, Color fgColor);
        private void ShowStatus(string msg, Color fgColor)
        {
            if (this.InvokeRequired)
            {
                Invoke(new ShowMsgCallback(ShowStatus), new object[] { msg, fgColor });
            }
            else
            {
                tStrpLabel.Text = msg;
                tStrpLabel.ForeColor = fgColor;
                statusBar.Refresh();
            }
        }
        #endregion 

        #region 写日志到文件
        private void LogToFile(string text)
        {
            string fname;
            if(string.IsNullOrEmpty(_logFileName))
            {
                // not write log to file
                return;
            }
            else if (_logFileName == "default")
            {
                fname = DateTime.Now.ToString("yyyy-MM-dd HH") + "点.txt";
            }
            else
            {
                fname = _logFileName;
            }

            LogToFile(fname, text);
        }
        private void LogToFile(string fileName, string text, bool isAppend = true)
        {
            using (StreamWriter sw = new StreamWriter(fileName, isAppend, Encoding.UTF8))
            {
                sw.WriteLine(DateTime.Now.ToString("[ yyyy-MM-dd HH:mm:ss.fff ] ") + text);
            }
        }
        #endregion

        #region 关键字查找
        private void btFindKeyWord_Click(object sender, EventArgs e)
        {
            if(txtKeyWord.Text == "" || treeNwkAnalysis.SelectedNode == null)
            {
                MessageBox.Show("请输入关键字，并在数视图中选择要查找的起始位置");
                return;
            }


            TreeNode node = FindTreeNode(treeNwkAnalysis.SelectedNode, txtKeyWord.Text.Trim());
            if(node != null)
            {
                treeNwkAnalysis.SelectedNode = node;
                treeNwkAnalysis.Focus();
            }
            else
            {
                MessageBox.Show("关键字未找到！请尝试从其他位置开始查找");
            }
        }

        private TreeNode FindTreeNode(TreeNode node, string text)
        {
            TreeNode retNode = null;

            if(node.Text.Contains(text))
            {
                retNode = node;
            }
            else if(node.Nodes.Count > 0)
            {
                foreach (TreeNode subNode in node.Nodes)
                {
                    if (subNode.Text.Contains(text))
                    {
                        retNode = subNode;
                        break;
                    }
                }

                if (retNode == null)
                {
                    foreach (TreeNode subNode in node.Nodes)
                    {
                        if ((retNode = FindTreeNode(subNode, text)) != null)
                        {
                            break;
                        }
                    }
                }
            }

            return retNode;
        }
        #endregion

        #region 台区近况详情查看
        private void combStationList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combStationList.SelectedIndex < 0) return;

            ShowStationData(combStationList.SelectedIndex);
        }
        private void btShowStationData_Click(object sender, EventArgs e)
        {
            if (combStationList.SelectedIndex < 0) return;

            ShowStationData(combStationList.SelectedIndex);
        }
        private void ShowStationData(int index)
        {
            if (index < 0 || index >= _stationList.Count) return;

            TreeNode tree = _stationList.ElementAt(index).GetTree();
            treeProtocol.BeginUpdate();
            treeProtocol.Nodes.Clear();
            treeProtocol.Nodes.Add(tree);
            treeProtocol.EndUpdate();
        }
        #endregion

        private void btFolderSelct_Click(object sender, EventArgs e)
        {
            openFileDlg.Title = "请选择 plccom.log 所在目录的任意文件";

            if (DialogResult.OK != openFileDlg.ShowDialog() || openFileDlg.FileName == "")
            {
                return;
            }

            string logPath = Path.GetDirectoryName(openFileDlg.FileName);

            if(Directory.EnumerateFiles(logPath, "plccom.log", SearchOption.TopDirectoryOnly).Count() > 0
                && Directory.EnumerateFiles(logPath, "plcrec.log", SearchOption.TopDirectoryOnly).Count() > 0)
            {
                DateTime startTime = DateTime.Now;

                string exePath = Path.GetDirectoryName(Application.ExecutablePath);
                Util.ExecuteWindowsCmd(exePath + "\\merge_plccomlog.bat", logPath, exePath);

                if (!File.Exists(logPath + "\\plccomlog\\plccom.log"))
                {
                    return;
                }

                string unpack = (DateTime.Now - startTime).TotalMilliseconds + "ms";

                _isLogLoading = true;

                if (_dailyData == null)
                {
                    _dailyData = new DailyData();
                }

                StreamReader sr = new StreamReader(logPath + "\\plccomlog\\plccom.log", Encoding.UTF8);
                string strRead;
                int cnt = 0;

                while ((strRead = sr.ReadLine()) != null)
                {
                    if (strRead == "") continue;

                    ParsePlccomLog(strRead);
                    cnt++;
                    if(cnt % 5000 == 0)
                    {
                        ShowStatus("载入日志中。。 " + cnt, Color.Green);
                    }
                }

                sr.Close();

                dgvLog.FirstDisplayedScrollingRowIndex = dgvLog.RowCount - 1;

                string total = (DateTime.Now - startTime).TotalMilliseconds + "ms";

                ShowStatus("载入日志完成 " + cnt + "unpack:" + unpack + " " + "total:" + total, Color.Green);

                _isLogLoading = false;

                //combStationList.Items.Clear();
                //for (int i = 0; i < _stationList.Count; i++)
                //{
                    //if (_stationList.ElementAt(i).CenterAddr != null)
                    //{
                   //     combStationList.Items.Add(_stationList.ElementAt(i).CenterAddr);
                    //}
                //}

            }
            else
            {
                ShowStatus("请选择正确的日志路径\r\n", Color.Red);
            }
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
