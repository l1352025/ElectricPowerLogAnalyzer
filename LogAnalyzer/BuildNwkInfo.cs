using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LogAnalyzer
{
    #region 组网信息
    public class BuildNwkInfo
    {
        public string IsBuildNwkFinished;  // 组网完成标志
        public string BuildNwkStartTime;   // 组网启动时间
        public string BuildNwkUsedTime;    // 组网花费时间
        public string WorkChanelGrp;       // 工作信道组

        public Dictionary<string, SubNodeInfo> NodeDocuments;   // 子节点档案
        public List<SubNodeInfo> AmeterDocuments;   // 电表档案
        public List<SubNodeInfo> WaterDocuments;    // 水表档案
        public List<SubNodeInfo> AmeterOfflineNodes;     // 入网失败节点
        public List<SubNodeInfo> WaterOfflineNodes;      // 入网失败节点
        public List<SubNodeInfo>[] AmeterNwkNodes;  // 网络分布情况： 1-7级为已入网，0级为未入网 
        public List<SubNodeInfo>[] WaterNwkNodes;   // 网络分布情况： 1-7级为已入网，0级为未入网 

        public BuildNwkInfo()
        {
            NodeDocuments = new Dictionary<string, SubNodeInfo>();
            AmeterDocuments = new List<SubNodeInfo>();
            WaterDocuments = new List<SubNodeInfo>();
            AmeterOfflineNodes = new List<SubNodeInfo>();
            WaterOfflineNodes = new List<SubNodeInfo>();

            AmeterNwkNodes = new List<SubNodeInfo>[8]
            {
                new List<SubNodeInfo>(), new List<SubNodeInfo>(), new List<SubNodeInfo>(), new List<SubNodeInfo>(),
                new List<SubNodeInfo>(), new List<SubNodeInfo>(), new List<SubNodeInfo>(), new List<SubNodeInfo>()
            };

            WaterNwkNodes = new List<SubNodeInfo>[8]
            {
                new List<SubNodeInfo>(), new List<SubNodeInfo>(), new List<SubNodeInfo>(), new List<SubNodeInfo>(),
                new List<SubNodeInfo>(), new List<SubNodeInfo>(), new List<SubNodeInfo>(), new List<SubNodeInfo>()
            };

        }

        public TreeNode GetTree()
        {
            DateTime time = DateTime.Parse(this.BuildNwkStartTime);
            string strTmp = (time.Hour >= 21 ? "昨天" + time.ToString("HH:mm") : "今天" + time.ToString("HH:mm"))
                            + " 组网(" + (AmeterDocuments.Count - AmeterOfflineNodes.Count) + "/" + AmeterDocuments.Count
                            + ")";
            TreeNode node = new TreeNode( strTmp );

            strTmp = "完成标志：" + this.IsBuildNwkFinished;
            node.Nodes.Add(strTmp);
            strTmp = "启动时间：" + this.BuildNwkStartTime;
            node.Nodes.Add(strTmp);
            strTmp = "花费时间：" + this.BuildNwkUsedTime + "分钟";
            node.Nodes.Add(strTmp);
            strTmp = "工作信道组：" + this.IsBuildNwkFinished;
            node.Nodes.Add(strTmp);

            // 电表档案
            TreeNode tAmeterDoc = new TreeNode("电表档案 (入网 " 
                                + (AmeterDocuments.Count - AmeterOfflineNodes.Count) + "/" + AmeterDocuments.Count + ")");
            {
                TreeNode tNodeInfo;
                for (int i = 0; i < AmeterDocuments.Count; i++)
                {
                    tNodeInfo = AmeterDocuments[i].GetTree();
                    tNodeInfo.Text = "[" + (i) + "]：" + tNodeInfo.Text;
                    tAmeterDoc.Nodes.Add(tNodeInfo);
                }
            }
            node.Nodes.Add(tAmeterDoc);

            // 水表档案
            TreeNode tWaterDoc = new TreeNode("水表档案 (入网 " 
                                + (WaterDocuments.Count - WaterOfflineNodes.Count) + "/" + WaterDocuments.Count + ")");
            {
                TreeNode tNodeInfo;
                for (int i = 0; i < WaterDocuments.Count; i++)
                {
                    tNodeInfo = WaterDocuments[i].GetTree();
                    tNodeInfo.Text = "[" + (i) + "]：" + tNodeInfo.Text;
                    tWaterDoc.Nodes.Add(tNodeInfo);
                }
            }
            node.Nodes.Add(tWaterDoc);

            return node;
        }
    }
    #endregion
}
