using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIE.Geometry;
using PIE.Controls;
using PIE.AxControls;
using PIE.ControlsUI;
using PIE.SystemUI;
// using PIE.AdapterUI;
using PIE.Plugin;
using PIE.CommonAlgo;
using PIE.SystemAlgo;
using PIE.Carto;
using PIE.DataSource;
using PIE.Framework;
namespace PIESDKUser
{
    class zuida : BaseCommand
   
        {
            /// <summary>
            /// 监督分类参数

            /// </summary>
            private PIE.CommonAlgo.SupervisedClassification_Exchange_Info m_DataInfo = null;


            /// <summary>
            /// 单击方法
            /// </summary>
            public override void OnClick()
            {
                int selBandNums = 3;//选择分类数据的波段
                m_DataInfo = new PIE.CommonAlgo.SupervisedClassification_Exchange_Info();
                m_DataInfo.InputFilePath = @"D:\data\China1\World.tif";
                m_DataInfo.OutputFilePath = @"D:\data\temp\WorldDisClassify.img";
                m_DataInfo.CStart = 0;//行列起止值
                m_DataInfo.CEnd = 2046;
                m_DataInfo.RStart = 0;
                m_DataInfo.REnd = 1022;
                m_DataInfo.SelBandNums = selBandNums;//波段的个数
                m_DataInfo.ClassifierType = 1;//分类类型 0 代表最大似然;1代表距离分类

                //多光谱波段的的集合
                List<int> listBandIndex = new List<int>();
                for (int i = 0; i < selBandNums; i++)
                {
                    listBandIndex.Add(i + 1);
                }
                m_DataInfo.SelBandIndexs = listBandIndex;

                #region ROIStatistics ROI统计算法
                ROIStatistics_Exchange_Info roiDataInfo = new PIE.CommonAlgo.ROIStatistics_Exchange_Info();
                roiDataInfo.ClassifierType = m_DataInfo.ClassifierType;
                roiDataInfo.currentFileName = m_DataInfo.InputFilePath;
                roiDataInfo.selBandIndex = m_DataInfo.SelBandIndexs.ToArray();
                roiDataInfo.selBandNums = m_DataInfo.SelBandNums;

                IList<ILayer> listLayer = m_HookHelper.ActiveView.FocusMap.GetAllLayer();
                foreach (ILayer item in listLayer)
                {
                    if (item.Name == "roi_layer")//得到利用分类工具生成的roi文件
                    {
                        roiDataInfo.pROILayer = item as IGraphicsLayer;
                    }
                    //创建并执行roi统计算法
                    ISystemAlgo algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.ROIStatisticsAlgo");
                    algo.Params = roiDataInfo;
                    bool result = AlgoFactory.Instance().ExecuteAlgo(algo);//同步执行roi统计算法
                #endregion
                    #region DistanceClassify算法
                    //执行完成后得到roi统计的参数会发生变化
                    roiDataInfo = (ROIStatistics_Exchange_Info)algo.Params;
                    int roiNum = roiDataInfo.vecROIName.Count;

                    m_DataInfo.ROINums = roiNum; //ROI个数
                    m_DataInfo.ListRoiNames = roiDataInfo.vecROIName;//roi名称列表
                    m_DataInfo.ListRoiColors = roiDataInfo.vecROIColor;//roi颜色集合
                    m_DataInfo.ROIMean = new List<double>();
                    m_DataInfo.ROICof = new List<double>();
                    for (int i = 0; i < roiNum; i++)//roi均值集合
                    {
                        for (int j = 0; j < selBandNums; j++)
                        {
                            m_DataInfo.ROIMean.Add(roiDataInfo.vecMean[i * selBandNums + j]);
                        }
                        for (int k = 0; k < selBandNums * selBandNums; k++)
                        {
                            m_DataInfo.ROICof.Add(roiDataInfo.vecCof[i * selBandNums * selBandNums + k]);
                        }
                    }
                    ISystemAlgo distanceAlgo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.MLClassificationAlgo");//最大似然法就将DistanceClassificationAlgo替换为MLClassificationAlgo
                    distanceAlgo.Params = m_DataInfo;
                    ISystemAlgoEvents systemEvents = distanceAlgo as ISystemAlgoEvents;
                    systemEvents.OnExecuteCompleted += systemEvents_OnExecuteCompleted;
                    result = AlgoFactory.Instance().ExecuteAlgo(distanceAlgo);
                    #endregion
                }
            }

            /// <summary>
            /// 算法执行完成事件
            /// </summary>
            /// <param name="algo"></param>
            void systemEvents_OnExecuteCompleted(ISystemAlgo algo)
            {
                ISystemAlgoEvents systemEvents = algo as ISystemAlgoEvents;
                systemEvents.OnExecuteCompleted -= systemEvents_OnExecuteCompleted;
                ILayer layer = LayerFactory.CreateDefaultLayer(m_DataInfo.OutputFilePath);
                if (layer == null)
                {
                    System.Windows.Forms.MessageBox.Show("分类后图层为空");
                    return;
                }
                m_HookHelper.ActiveView.FocusMap.AddLayer(layer);
                m_HookHelper.ActiveView.PartialRefresh(ViewDrawPhaseType.ViewAll);
            }
        }
    }
