using ElectricPowerDebuger.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LogAnalyzer
{
    #region  子节点信息类
    public class SubNodeInfo
    {
        #region 档案信息
        public class DocumentInfo : BitField
        {
            [BitLength(56)]
            public byte[] LongAddr;

            [BitLength(1)]
            public byte Flg_Deleted;

            [BitLength(1)]
            public byte Flg_Named;

            [BitLength(1)]
            public byte Flg_Configed;

            [BitLength(1)]
            public byte Flg_Find;

            [BitLength(1)]
            public byte Flg_Event;

            [BitLength(1)]
            public byte Flg_SaveFailed;

            [BitLength(1)]
            public byte Flg_Maintain;

            [BitLength(1)]
            public byte Flg_Reserve;

            [BitLength(3)]
            public byte NodeType;

            [BitLength(3)]
            public byte PathCnt;

            [BitLength(3)]
            public byte ReadAmeterPathPrio;

            [BitLength(3)]
            public byte CurrPathPrio;

            [BitLength(4)]
            public byte Counter;

            [BitLength(4)]
            public byte Flg_Update;

            [BitLength(4)]
            public byte ClassProperty;

            [BitLength(1)]
            public byte Flg_BroadcastMaintain;

            [BitLength(7)]
            public byte NeighborCnt;

            [BitLength(11)]
            public ushort PathCost;

            [BitLength(4)]
            public byte Layer;

            [BitLength(1)]
            public byte Flg_WaterGasMeterNamed;

            [BitLength(10)]
            public ushort TSNo;

            [BitLength(1)]
            public byte Flg_Online;

            [BitLength(1)]
            public byte Flg_AmeterNamed;

            [BitLength(1)]
            public byte Flg_ReBuildPath;

            [BitLength(1)]
            public byte Flg_Normal;

            [BitLength(1)]
            public byte Flg_ReadAmeterChGrp;

            [BitLength(1)]
            public byte Flg_ReadAutoReportMeter;

            [BitLength(8)]
            public byte ReadAmeterSuccessRate;

            [BitLength(4)]
            public byte Backup_ReadAmeterChGrp;

            [BitLength(4)]
            public byte Backup_ReadAmeterPathPrio;

            [BitLength(16)]
            public ushort ReadAmeterSuccessCnt;

            [BitLength(16)]
            public ushort ReadAmeterTotalCnt;

            public DocumentInfo()
            {
            }

            public DocumentInfo(byte[] buf)
            {
                this.Parse(buf);
            }
        }
        #endregion

        #region 邻居信息
        public class NeighborInfo : BitField
        {
            [BitLength(12)]
            public ushort NodeIndex;

            [BitLength(4)]
            public byte Reserve;

            [BitLength(8)]
            public byte UpRssi;

            [BitLength(8)]
            public byte DownRssi;

            public NeighborInfo()
            {
            }

            public NeighborInfo(byte[] buf)
            {
                this.Parse(buf);
            }
        }
        #endregion

        #region 路径信息
        public class RouteInfo : BitField
        {
            [BitLength(96)]
            public byte[] Path;     // 路径: 每个节点2byte * 6

            [BitLength(5)]
            public byte RelateCnt;

            [BitLength(11)]
            public ushort Cost;

            [BitLength(4)]
            public byte Prio;

            [BitLength(4)]
            public byte Jump;

            [BitLength(7)]
            public byte SuccessCnt;

            [BitLength(1)]
            public byte Flg_RssiBeyond;

            [BitLength(7)]
            public byte FailedCnt;

            [BitLength(1)]
            public byte Flg_NeighborPathSucced;

            [BitLength(4)]
            public byte Relate_Path1;

            [BitLength(4)]
            public byte Relate_Path2;

            [BitLength(4)]
            public byte Relate_Path3;

            [BitLength(4)]
            public byte Relate_Path4;

            [BitLength(4)]
            public byte Relate_Path5;

            [BitLength(4)]
            public byte Relate_Path6;

            public RouteInfo()
            {
            }

            public RouteInfo(byte[] buf)
            {
                this.Parse(buf);
            }
        }
        #endregion

        #region 电表数据
        public class AmeterRecord
        {
            public string DayValue;         // 日冻结值
            public string ReadTime;         // 抄读时间
        }
        #endregion

        #region 水表数据
        public class WaterRecord
        {
            public string DeviceAddr;       // 水表地址
            public string MeterTime;        // 上报/抄读时间
            public string MeterType;        // 表类型
            public string CurrValue;        // 当前累计
            public string CalcDays;         // 统计天数
            public string LastValue;        // 上次累计
            public string MeterStatus;      // 表状态
            public string ProductYear;      // 出厂年份
        }
        #endregion

        private string longAddr;                 
        public string LongAddr
        {
            get { return longAddr; }
            set { longAddr = value; ShortAddr = string.IsNullOrEmpty(value) ? "" : value.Substring(8, 4); }
        }
        public string ShortAddr { get; set; }
        public byte RelayLevel;
        public string SignalQuality;
        public string Phase123;
        public string ProtoType;
        public string UpgradeFlag;
        public string AppVer;
        public string LoaderVer;
        public string BadReason;              // 备注：组网失败或通信成功率低的原因
        public AmeterRecord AmeterData;       // 电表数据
        public WaterRecord WaterData;         // 水表数据

        public DocumentInfo DocInfo;            // 档案信息
        public List<NeighborInfo> NeighborTbl;  // 邻居表
        public List<RouteInfo> RouteTbl;        // 路径表

        public TreeNode treeDocInfo;
        public TreeNode treeNeighbors;
        public TreeNode treeRoutes;

        public SubNodeInfo()
            : this("", "")
        {
            
        }
        public SubNodeInfo(string longAddr, string proto)
        {
            LongAddr = longAddr;
            ProtoType = proto;
            AmeterData = new AmeterRecord();
            WaterData = new WaterRecord();
        }

        public TreeNode GetTree()
        {
            TreeNode retNode;
            string strTmp = " ( " + ((RelayLevel == 0) ? "离网" : (RelayLevel + "级")) + " )";

            TreeNode nodeInfo = new TreeNode(LongAddr + strTmp);
            {
                // strTmp = "信号品质：" + SignalQuality;
                // nodeInfo.Nodes.Add(strTmp);
                // strTmp = "相位：" + Phase123;
                // nodeInfo.Nodes.Add(strTmp);
                strTmp = "协议类型：" + ProtoType;
                nodeInfo.Nodes.Add(strTmp);
                strTmp = "升级标志：" + UpgradeFlag;
                nodeInfo.Nodes.Add(strTmp);
                strTmp = "软件版本：" + AppVer;
                nodeInfo.Nodes.Add(strTmp);
                strTmp = "Boot版本：" + LoaderVer;
                nodeInfo.Nodes.Add(strTmp);

                if (treeDocInfo != null)
                {
                    nodeInfo.Nodes.Add(treeDocInfo);
                }

                if (treeNeighbors != null)
                {
                    nodeInfo.Nodes.Add(treeNeighbors);
                }

                if (treeRoutes != null)
                {
                    nodeInfo.Nodes.Add(treeRoutes);
                }

                if(ProtoType.Contains("07电表"))
                {
                    strTmp = "电表读数：" + AmeterData.DayValue;
                    nodeInfo.Nodes.Add(strTmp);
                    strTmp = "抄表时间：" + AmeterData.ReadTime;
                    nodeInfo.Nodes.Add(strTmp);
                }
                else if (ProtoType.Contains("单向水表"))
                {
                    strTmp = "水表地址：" + WaterData.DeviceAddr;
                    nodeInfo.Nodes.Add(strTmp);
                    strTmp = "上报时间：" + WaterData.MeterTime;
                    nodeInfo.Nodes.Add(strTmp);
                    strTmp = "水表分类：" + WaterData.MeterType;
                    nodeInfo.Nodes.Add(strTmp);
                    strTmp = "当前用量：" + WaterData.CurrValue;
                    nodeInfo.Nodes.Add(strTmp);
                    strTmp = "统计天数：" + WaterData.CalcDays;
                    nodeInfo.Nodes.Add(strTmp);
                    strTmp = "上月用量：" + WaterData.LastValue;
                    nodeInfo.Nodes.Add(strTmp);
                    strTmp = "水表状态：" + WaterData.MeterStatus;
                    nodeInfo.Nodes.Add(strTmp);
                    strTmp = "出厂年份：" + WaterData.ProductYear;
                    nodeInfo.Nodes.Add(strTmp);
                }
            }

            retNode = (TreeNode)nodeInfo.Clone();
            retNode.Nodes[retNode.Nodes.Count - 2].Expand();
            retNode.Nodes[retNode.Nodes.Count - 1].Expand();

            return retNode;
        }
    }
    #endregion
}
