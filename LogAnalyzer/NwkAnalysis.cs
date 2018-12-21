using ElectricPowerDebuger.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LogAnalyzer
{
    #region  台区分析类
    public class NwkAnalysis
    {
        // 台区概况
        public struct NwkSummaryInfo
        {
            public string ConcAddr;         // 集中器地址
            public string PanId;                // PANID
            public string ChanelGrp;        // 工作信道组
            public string RssiThreshold;    // 场强门限
            public string ReleaseDate;      // 发布日期
            public string MainNodeVer;      // 主模块版本
            public string SubNodeVer;           // 子模块版本
            public string DocumentCnt;      // 档案总数
            public string IsBuildNwkFinished;  // 组网完成标志
            public string BuildNwkStartTime;   // 组网启动时间
            public string BuildNwkUsedTime;    // 组网花费时间
            public string BuildNwkSuccessRate; // 组网成功率
            public string BuildNwkSuccessCnt;  // 组网成功个数
            public string BuildNwkFailureCnt;  // 组网失败个数
            public string DiscovedNodeCnt;      // 发现数量
            public string NamedNodeCnt;         // 点名数量
            public string ConfigedNodeCnt;      // 配置数量
            public string MaintainedNodeCnt;    // 维护数量
            public string MainNodeNeighborCnt;  // 中心节点邻居数量
            public string BroadcastMaintainFlag;    // 广播维护开关
            public string BroadcastMaintainTime;    // 广播维护时间

            public TreeNode GetSummaryInfoTree()
            {
                string strTmp;
                TreeNode tNodeSummary = new TreeNode("台区概况");

                strTmp = "集中器地址：" + this.ConcAddr;
                tNodeSummary.Nodes.Add(strTmp);
                strTmp = "工作信道组：" + this.ChanelGrp;
                tNodeSummary.Nodes.Add(strTmp);
                strTmp = "场强门限  ：" + this.RssiThreshold;
                tNodeSummary.Nodes.Add(strTmp);
                strTmp = "发布日期  ：" + this.ReleaseDate;
                tNodeSummary.Nodes.Add(strTmp);
                strTmp = "主模块版本：" + this.MainNodeVer;
                tNodeSummary.Nodes.Add(strTmp);
                strTmp = "子模块版本：" + this.SubNodeVer;
                tNodeSummary.Nodes.Add(strTmp);
                strTmp = "台区档案总数：" + this.DocumentCnt;
                tNodeSummary.Nodes.Add(strTmp);
                strTmp = "组网完成标志：" + this.IsBuildNwkFinished;
                tNodeSummary.Nodes.Add(strTmp);
                strTmp = "组网花费时间：" + this.BuildNwkUsedTime;
                tNodeSummary.Nodes.Add(strTmp);
                strTmp = "组网成功率  ：" + this.BuildNwkSuccessRate;
                tNodeSummary.Nodes.Add(strTmp);
                strTmp = "组网成功个数：" + this.BuildNwkSuccessCnt;
                tNodeSummary.Nodes.Add(strTmp);
                strTmp = "组网失败个数：" + this.BuildNwkFailureCnt;
                tNodeSummary.Nodes.Add(strTmp);
#if true
                tNodeSummary.Nodes.Add("");     // 插入一空行
                strTmp = "发现节点个数：" + this.DiscovedNodeCnt;
                tNodeSummary.Nodes.Add(strTmp);
                strTmp = "点名节点个数：" + this.NamedNodeCnt;
                tNodeSummary.Nodes.Add(strTmp);
                strTmp = "配置节点个数：" + this.ConfigedNodeCnt;
                tNodeSummary.Nodes.Add(strTmp);
                strTmp = "维护节点个数：" + this.MaintainedNodeCnt;
                tNodeSummary.Nodes.Add(strTmp);
                strTmp = "中心邻居个数：" + this.MainNodeNeighborCnt;
                tNodeSummary.Nodes.Add(strTmp);
                strTmp = "广播维护开关：" + this.BroadcastMaintainFlag;
                tNodeSummary.Nodes.Add(strTmp);
                strTmp = "广播维护时间：" + this.BroadcastMaintainTime;
                tNodeSummary.Nodes.Add(strTmp);
#endif
                return tNodeSummary;
            }
        }

        // 子节点档案
        public Dictionary<string, SubNodeInfo> NodeDocuments;

        // 网络概况
        public NwkSummaryInfo SummaryInfo;

        // 网络节点情况
        public List<string> MainNodeNeighbors;      // 中心节点邻居 (总数，本台区个数)
        public List<string> LocalNwkNodes;          // 已发现本台区节点数
        public List<SubNodeInfo>[] NwkNodes;    // 网络分布情况： 1-7级为已入网，0级为未入网 

        // 原因分析
        public List<SubNodeInfo> OfflineNodes;          // 入网失败节点
        public List<SubNodeInfo> LowSuccessRateNodes;   // 通信成功率低节点
        public List<SubNodeInfo> AloneNodes;            // 全网孤立节点

        public NwkAnalysis()
        {
            NodeDocuments = new Dictionary<string, SubNodeInfo>();
            SummaryInfo = new NwkSummaryInfo();
            MainNodeNeighbors = new List<string>();
            LocalNwkNodes = new List<string>();
            OfflineNodes = new List<SubNodeInfo>();
            LowSuccessRateNodes = new List<SubNodeInfo>();
            AloneNodes = new List<SubNodeInfo>();
            NwkNodes = new List<SubNodeInfo>[8]
            {
                new List<SubNodeInfo>(), new List<SubNodeInfo>(), new List<SubNodeInfo>(), new List<SubNodeInfo>(),
                new List<SubNodeInfo>(), new List<SubNodeInfo>(), new List<SubNodeInfo>(), new List<SubNodeInfo>()
            };
        }

        public void Clear()
        {
            NodeDocuments.Clear();
            MainNodeNeighbors.Clear();
            LocalNwkNodes.Clear();
            OfflineNodes.Clear();
            LowSuccessRateNodes.Clear();
            AloneNodes.Clear();
            foreach (List<SubNodeInfo> list in NwkNodes)
            {
                list.Clear();
            }
        }

        // 生成分析结果树视图
        public TreeNode GetNwkAnalysisTree()
        {
            TreeNode root = new TreeNode("组网信息");
            string strTmp = "";
            TreeNode tNode, tNodeInfo;

            // root--台区概况
            TreeNode tNodeSummary = new TreeNode("台区概况");
            foreach (TreeNode node in SummaryInfo.GetSummaryInfoTree().Nodes)
            {
                tNodeSummary.Nodes.Add(node);
            }
            tNodeSummary.Expand();
            root.Nodes.Add(tNodeSummary);

            // root--网络分布情况
            TreeNode tNodeNetStruct = new TreeNode("网络分布");
            {
                // -- 中心节点邻居数
                strTmp = "中心节点邻居" + " (" + MainNodeNeighbors.Count + ")";
                tNode = new TreeNode(strTmp);
                {
                    for (int i = 0; i < MainNodeNeighbors.Count; i++)
                    {
                        strTmp = "节点" + (i + 1) + "：" + MainNodeNeighbors[i];
                        tNode.Nodes.Add(strTmp);
                    }
                }
                tNodeNetStruct.Nodes.Add(tNode);

                // -- 已发现节点数
                strTmp = "已发现节点数" + " (" + LocalNwkNodes.Count + ")";
                tNode = new TreeNode(strTmp);
                {
                    for (int i = 0; i < LocalNwkNodes.Count; i++)
                    {
                        strTmp = "节点" + (i + 1) + "：" + LocalNwkNodes[i];
                        tNode.Nodes.Add(strTmp);
                    }
                }
                tNodeNetStruct.Nodes.Add(tNode);

                // --  1->7层节点信息
                for (int i = 1; i < NwkNodes.Length; i++)
                {
                    if (NwkNodes[i].Count == 0)
                    {
                        continue;
                    }

                    strTmp = "在网【" + i + "级】节点数 (" + NwkNodes[i].Count + ")";
                    tNode = new TreeNode(strTmp);
                    {
                        for (int k = 0; k < NwkNodes[i].Count; k++)
                        {
                            tNodeInfo = NwkNodes[i][k].GetTree();
                            tNodeInfo.Text = "节点" + (k + 1) + "：" + tNodeInfo.Text;
                            tNode.Nodes.Add(tNodeInfo);
                        }
                    }
                    tNodeNetStruct.Nodes.Add(tNode);
                }
            }
            tNodeNetStruct.Expand();
            root.Nodes.Add(tNodeNetStruct);

            // root--问题节点分析
            TreeNode tNodeAnalysis = new TreeNode("问题节点分析");
            {
                // --  组网失败节点
                strTmp = "组网失败节点数 (" + OfflineNodes.Count + ")";
                tNode = new TreeNode(strTmp);
                {
                    for (int k = 0; k < OfflineNodes.Count; k++)
                    {
                        tNodeInfo = OfflineNodes[k].GetTree();
                        tNodeInfo.Text = "节点" + (k + 1) + "：" + tNodeInfo.Text + " " + OfflineNodes[k].BadReason;
                        tNode.Nodes.Add(tNodeInfo);
                    }
                }
                tNodeAnalysis.Nodes.Add(tNode);

                // --  通信成功率低的节点
                strTmp = "通信成功率低的节点数 (" + LowSuccessRateNodes.Count + ")";
                tNode = new TreeNode(strTmp);
                {
                    for (int k = 0; k < LowSuccessRateNodes.Count; k++)
                    {
                        tNodeInfo = LowSuccessRateNodes[k].GetTree();
                        tNodeInfo.Text = "节点" + (k + 1) + "：" + tNodeInfo.Text + " " + LowSuccessRateNodes[k].BadReason;
                        tNode.Nodes.Add(tNodeInfo);
                    }
                }
                tNodeAnalysis.Nodes.Add(tNode);

                // --  全网孤立节点
                strTmp = "全网孤立节点数 (" + AloneNodes.Count + ")";
                tNode = new TreeNode(strTmp);
                {
                    for (int k = 0; k < AloneNodes.Count; k++)
                    {
                        tNodeInfo = AloneNodes[k].GetTree();
                        tNodeInfo.Text = "节点" + (k + 1) + "：" + tNodeInfo.Text;
                        tNode.Nodes.Add(tNodeInfo);
                    }
                }
                tNodeAnalysis.Nodes.Add(tNode);

            }
            tNodeAnalysis.Expand();
            root.Nodes.Add(tNodeAnalysis);

#if true
            // root--台区档案
            TreeNode tNodeDoc = new TreeNode("台区档案" + " (" + NodeDocuments.Count + ")");
            {
                for (int i = 0; i < NodeDocuments.Count; i++)
                {
                    tNodeInfo = NodeDocuments.ElementAt(i).Value.GetTree();
                    tNodeInfo.Text = "序号[" + (i) + "]：" + tNodeInfo.Text;
                    tNodeDoc.Nodes.Add(tNodeInfo);
                }
            }
            tNodeDoc.Expand();
            root.Nodes.Add(tNodeDoc);
#endif

            return root;
        }

        // 保存分析结果
        public void SaveNwkAnalysisInfo(string strFileName)
        {
            string strText = "", strTmp = "";
            SubNodeInfo subNode;

            StreamWriter sw = new StreamWriter(strFileName, false, Encoding.UTF8);

            sw.WriteLine();
            sw.WriteLine("【台区概况】\r\n");
            foreach (TreeNode node in SummaryInfo.GetSummaryInfoTree().Nodes)
            {
                strText = "\t" + node.Text;
                sw.WriteLine(strText);
            }
            sw.WriteLine("\r\n");

            sw.WriteLine("【问题分析】\r\n");

            strText = "\t" + "组网失败节点：" + OfflineNodes.Count + "\r\n"
                    + "\t" + "------------------------------------------------------------------" + "\r\n"
                    + "\t" + "序号\t" + "地址\t\t" + "节点类型\t" + "邻居数\t" + "路径数\t" + "原因分析";
            sw.WriteLine(strText);
            for (int i = 0; i < OfflineNodes.Count; i++)
            {
                subNode = OfflineNodes[i];
                strText = "\t" + (i + 1) + "\t" + subNode.LongAddr + "\t" + subNode.ProtoType + "\t"
                        + subNode.NeighborTbl.Count + "\t\t" + subNode.RouteTbl.Count + "\t\t" + subNode.BadReason;
                sw.WriteLine(strText);
            }
            sw.WriteLine("\r\n");

            strText = "\t" + "通信成功率低节点：" + LowSuccessRateNodes.Count + "\r\n"
                    + "\t" + "------------------------------------------------------------------" + "\r\n"
                    + "\t" + "序号\t" + "地址\t\t" + "节点类型\t" + "通信成功率\t" + "邻居数\t" + "路径数\t" + "原因分析";
            sw.WriteLine(strText);
            for (int i = 0; i < LowSuccessRateNodes.Count; i++)
            {
                subNode = LowSuccessRateNodes[i];
                strText = "\t" + (i + 1) + "\t" + subNode.LongAddr + "\t" + subNode.ProtoType + "\t" + subNode.DocInfo.ReadAmeterSuccessRate + "\t\t"
                        + subNode.NeighborTbl.Count + "\t\t" + subNode.RouteTbl.Count + "\t\t" + subNode.BadReason;
                sw.WriteLine(strText);
            }
            sw.WriteLine("\r\n");

            strText = "\t" + "全网孤立节点：" + AloneNodes.Count + "\r\n"
                    + "\t" + "------------------------------------------------------------------" + "\r\n"
                    + "\t" + "序号\t" + "地址\t\t" + "节点类型\t" + "邻居数\t" + "路径数\t" + "邻居列表\t";
            sw.WriteLine(strText);
            for (int i = 0; i < AloneNodes.Count; i++)
            {
                subNode = AloneNodes[i];
                strText = "\t" + (i + 1) + "\t" + subNode.LongAddr + "\t" + subNode.ProtoType + "\t"
                        + subNode.NeighborTbl.Count + "\t\t" + subNode.RouteTbl.Count + "\t\t";
                for (int k = 0; k < subNode.treeNeighbors.Nodes.Count; k++)
                {
                    strText += subNode.treeNeighbors.Nodes[k].Text + "\t";
                }
                sw.WriteLine(strText);
            }
            sw.WriteLine("\r\n");

            sw.WriteLine("【网络分布】\r\n");

            strText = "\t" + "中心节点邻居：" + MainNodeNeighbors.Count + "\r\n"
                    + "\t" + "------------------------------------------------------------------" + "\r\n"
                    + "\t" + "序号\t" + "地址\t";
            sw.WriteLine(strText);
            for (int i = 0; i < MainNodeNeighbors.Count; i++)
            {
                strText = "\t" + (i + 1) + "\t" + MainNodeNeighbors[i];
                sw.WriteLine(strText);
            }
            sw.WriteLine("\r\n");

            strText = "\t" + "已发现节点数：" + LocalNwkNodes.Count + "\r\n"
                    + "\t" + "------------------------------------------------------------------" + "\r\n"
                    + "\t" + "序号\t" + "地址\t";
            sw.WriteLine(strText);
            for (int i = 0; i < LocalNwkNodes.Count; i++)
            {
                strText = "\t" + (i + 1) + "\t" + LocalNwkNodes[i];
                sw.WriteLine(strText);
            }
            sw.WriteLine("\r\n");

            for (int i = 1; i < NwkNodes.Length; i++)
            {
                if (NwkNodes[i].Count == 0)
                {
                    continue;
                }

                strText = "\t" + "在网【" + i + "级】节点：" + NwkNodes[i].Count + "\r\n"
                    + "\t" + "------------------------------------------------------------------" + "\r\n"
                    + "\t" + "序号\t" + "地址\t\t" + "节点类型\t" + "邻居数\t" + "路径数\t" + "邻居列表\t";
                sw.WriteLine(strText);
                for (int k = 0; k < NwkNodes[i].Count; k++)
                {
                    subNode = NwkNodes[i][k];
                    strText = "\t" + (k + 1) + "\t" + subNode.LongAddr + "\t" + subNode.ProtoType + "\t"
                            + subNode.NeighborTbl.Count + "\t\t" + subNode.RouteTbl.Count + "\t\t";
                    for (int kk = 0; kk < subNode.treeNeighbors.Nodes.Count; kk++)
                    {
                        strText += subNode.treeNeighbors.Nodes[kk].Text + "\t";
                    }
                    sw.WriteLine(strText);
                }
                sw.WriteLine("\r\n");

                strTmp += (i) + "级--" + NwkNodes[i].Count + "  ";
            }
            strTmp += " 失败--" + OfflineNodes.Count;

            sw.WriteLine("【台区档案】\r\n");

            strText = "\t" + "台区档案：" + NodeDocuments.Count + " ( " + strTmp + " )" + "\r\n"
                    + "\t" + "------------------------------------------------------------------" + "\r\n"
                    + "\t" + "序号\t" + "地址\t\t" + "节点类型\t" + "邻居数\t" + "路径数\t";
            sw.WriteLine(strText);
            for (int i = 0; i < NodeDocuments.Count; i++)
            {
                subNode = NodeDocuments.ElementAt(i).Value;
                strText = "\t" + (i + 1) + "\t" + subNode.LongAddr + "\t" + subNode.ProtoType + "\t"
                        + subNode.NeighborTbl.Count + "\t\t" + subNode.RouteTbl.Count + "\t\t";
                sw.WriteLine(strText);
            }
            sw.WriteLine("\r\n");

            sw.Close();
        }

    }
    #endregion
}
