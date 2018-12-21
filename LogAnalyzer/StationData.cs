using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LogAnalyzer
{
    #region 台区数据 -- 每日数据
    public class DailyData
    {
        public string Date;                         // 日期 yyyy-mm-dd
        public string CenterAddr;                   // 集中器地址
        public List<BuildNwkInfo> BuildNwkList;     // 组网信息列表

        public Dictionary<string, SubNodeInfo.AmeterRecord> AmeterDatas;     // 电表读数
        public Dictionary<string, SubNodeInfo.WaterRecord> WaterDatas;       // 水表读数
        public List<SubNodeInfo> ReadFailedAmeters;     // 电表抄读失败的节点
        public List<SubNodeInfo> ReadFailedWaters;      // 水表抄读失败的节点

        public DailyData()
        {
            Date = "";
            CenterAddr = "";
            BuildNwkList = new List<BuildNwkInfo>();
            AmeterDatas = new Dictionary<string, SubNodeInfo.AmeterRecord>();
            WaterDatas = new Dictionary<string, SubNodeInfo.WaterRecord>();
            ReadFailedAmeters = new List<SubNodeInfo>();
            ReadFailedWaters = new List<SubNodeInfo>();
        }

        public TreeNode GetTree()
        {
            TreeNode node = new TreeNode(Date);
            string strTmp = "";

            //汇总信息
            strTmp += "汇总信息：";
            int sumMax = 0, failMax = 0;
            for (int i = 0; i < BuildNwkList.Count; i++)
            {
                sumMax = (BuildNwkList[i].AmeterDocuments.Count > sumMax ? BuildNwkList[i].AmeterDocuments.Count : sumMax);
                failMax = (BuildNwkList[i].AmeterOfflineNodes.Count > failMax ? BuildNwkList[i].AmeterOfflineNodes.Count : failMax);
            }
            strTmp += "组网(" + (sumMax - failMax) + "/" + sumMax + ", " + ((float)(sumMax - failMax) / sumMax * 100).ToString("F2") + "%), ";
            strTmp += "抄电表(" + AmeterDatas.Count + "/" + (AmeterDatas.Count + ReadFailedAmeters.Count)
                    + ", " + ((float)AmeterDatas.Count / (AmeterDatas.Count + ReadFailedAmeters.Count) * 100).ToString("F2") + "%), ";
            strTmp += "抄水表(" + WaterDatas.Count + "/" + (WaterDatas.Count + ReadFailedWaters.Count)
                    + ", " + ((float)WaterDatas.Count / (WaterDatas.Count + ReadFailedWaters.Count) * 100).ToString("F2") + "%)";
            node.Nodes.Add(new TreeNode(strTmp));

            //组网信息
            for (int i = 0; i < BuildNwkList.Count; i++)
            {
                node.Nodes.Add(BuildNwkList[i].GetTree());
            }

            //电表读数
            strTmp = "电表读数(" + AmeterDatas.Count + "/" + (AmeterDatas.Count + ReadFailedAmeters.Count) + ")";
            TreeNode ameterNode = new TreeNode(strTmp);
            node.Nodes.Add(ameterNode);
            for (int i = 0; i < ReadFailedAmeters.Count; i++)
            {
                strTmp = "[" + (i + 1) + "]：" + ReadFailedAmeters[i].LongAddr + " (失败)";
                ameterNode.Nodes.Add(strTmp);
            }
            for (int i = 0; i < AmeterDatas.Count; i++)
            {
                strTmp = "[" + (i + 1) + "]：" + AmeterDatas.ElementAt(i).Key
                        + " (" + AmeterDatas.ElementAt(i).Value.ReadTime
                        + "  " + AmeterDatas.ElementAt(i).Value.DayValue + ")";
                ameterNode.Nodes.Add(strTmp);
            }

            //水表读数
            strTmp = "水表读数(" + WaterDatas.Count + "/" + (WaterDatas.Count + ReadFailedWaters.Count) + ")";
            TreeNode waterNode = new TreeNode(strTmp);
            node.Nodes.Add(waterNode);
            for (int i = 0; i < ReadFailedWaters.Count; i++)
            {
                strTmp = "[" + (i + 1) + "]：" + ReadFailedWaters[i].LongAddr + " (失败)";
                waterNode.Nodes.Add(strTmp);
            }
            for (int i = 0; i < WaterDatas.Count; i++)
            {
                strTmp = WaterDatas.ElementAt(i).Value.CurrValue.Substring(0, 4).TrimStart('0');
                strTmp = (strTmp == "" ? "0" : strTmp) + "." + WaterDatas.ElementAt(i).Value.CurrValue.Substring(4, 2) + "m3";

                strTmp = "[" + (i + 1) + "]：" + WaterDatas.ElementAt(i).Key
                        + " (" + WaterDatas.ElementAt(i).Value.MeterTime
                        + "  " + strTmp + ")";
                waterNode.Nodes.Add(strTmp);
            }

            node.Expand();

            return node;
        }

        public void AddAmeterData(SubNodeInfo subnode)
        {
            if (subnode.AmeterData.ReadTime == "") return;

            SubNodeInfo.AmeterRecord ameter;
            var rs = AmeterDatas.FirstOrDefault(q => q.Key == subnode.LongAddr);
            if (rs.Value == null)
            {
                ameter = new SubNodeInfo.AmeterRecord();
                AmeterDatas.Add(subnode.LongAddr, ameter);
            }
            else
            {
                ameter = rs.Value;
            }

            ameter.DayValue = subnode.AmeterData.DayValue;
            ameter.ReadTime = subnode.AmeterData.ReadTime.Substring(0);
        }
        public void RemoveAmeterData(SubNodeInfo subnode)
        {
            var rs = AmeterDatas.FirstOrDefault(q => q.Key == subnode.LongAddr);
            if (rs.Value != null)
            {
                AmeterDatas.Remove(subnode.LongAddr);
            }
        }

        public void AddWaterData(SubNodeInfo subnode)
        {
            if (subnode.WaterData.DeviceAddr == "") return;

            SubNodeInfo.WaterRecord water;
            var rs = WaterDatas.FirstOrDefault(q => q.Key == subnode.LongAddr);
            if (rs.Value == null)
            {
                water = new SubNodeInfo.WaterRecord();
                WaterDatas.Add(subnode.LongAddr, water);
            }
            else
            {
                water = rs.Value;
            }

            water.DeviceAddr = subnode.WaterData.DeviceAddr;
            water.MeterTime = subnode.WaterData.MeterTime;
            water.MeterType = subnode.WaterData.MeterType;
            water.CurrValue = subnode.WaterData.CurrValue;
            water.CalcDays = subnode.WaterData.CalcDays;
            water.LastValue = subnode.WaterData.LastValue;
            water.MeterStatus = subnode.WaterData.ProductYear;
        }
        public void RemoveWaterData(SubNodeInfo subnode)
        {
            var rs = AmeterDatas.FirstOrDefault(q => q.Key == subnode.LongAddr);
            if (rs.Value != null)
            {
                WaterDatas.Remove(subnode.LongAddr);
            }
        }
    }

    public class StationData
    {
        public string CenterAddr;
        public List<DailyData> DailyDataList;

        public StationData()
        {
            CenterAddr = "";
            DailyDataList = new List<DailyData>();
        }

        public void Add(DailyData data)
        {
            DailyDataList.Add(data);
        }

        public void Remove(DailyData data)
        {
            DailyDataList.Remove(data);
        }

        public TreeNode GetTree()
        {
            TreeNode node = new TreeNode("集中器地址：" + CenterAddr);

            //每日数据
            for (int i = 0; i < DailyDataList.Count; i++ )
            {
                node.Nodes.Add(DailyDataList[i].GetTree());
            }

            node.Expand();

            return node;
        }
    }

    #endregion 
}
