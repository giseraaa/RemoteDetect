using PIE.AxControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PIE.Geometry;
using PIE.Controls;
using PIE.ControlsUI;
using PIE.SystemUI;
// using PIE.AdapterUI;
using PIE.Plugin;
using PIE.CommonAlgo;
using PIE.SystemAlgo;
using PIE.Carto;
using PIE.DataSource;
using PIE.Framework;
using PIESDKUser;
//using PIE.AxControls;
namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private MapControl mapCtrl;
        private TOCControl tocCtrl;
        private ILayer m_Layer = null;
        public Form1()
        {
            InitializeComponent();

            //1.添加toccontrol
            tocCtrl = new TOCControl();
            tocCtrl.Dock = DockStyle.Fill;
            splitContainer1.Panel1.Controls.Add(tocCtrl);
            tocCtrl.MouseClick += tocCtrl_MouseClick;
            //2添加Mapcontrol
            mapCtrl = new PIE.AxControls.MapControl();
            mapCtrl.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(mapCtrl);
            mapCtrl.MouseMove += mapCtrl_MouseMove;
            tocCtrl.SetBuddyControl(mapCtrl);//图层树控件跟地图绑定

            IMap map = mapCtrl.FocusMap;
            ////添加地图文档
            //var pmd = @"F:\PIESDK\Data\基础矢量\行政区划\面\省级区域.pmd";
            //mapCtrl.LoadPmdFile(pmd);

            //mapCtrl.PartialRefresh(PIE.Carto.ViewDrawPhaseType.ViewAll);
            //4显示基本地图信息
            labMapName.Text = mapCtrl.FocusMap.Name;
            labLayerCount.Text = "" + mapCtrl.FocusMap.LayerCount + "";
            //labCoor.Text = ""+mapCtrl.FocusMap.SpatialReference.ExportToPrettyWkt()+"";

        }

       private void tocCtrl_MouseClick(object sender, MouseEventArgs e)
        {
           if (e.Button == MouseButtons.Right)//右键
           {  
               IMap pMap = null;
               m_Layer = null;
               PIETOCNodeType nodeType = PIETOCNodeType.Null;
               object unk = null;       
               object data = null;
               this.tocCtrl.HitTest(e.X, e.Y, ref nodeType, ref pMap, ref m_Layer, ref unk, ref data);
               //判断点击的节点是哪个类型，弹出指定的右键菜单
               if (nodeType == PIETOCNodeType.Map)
               {
                   //控制菜单项的显示隐藏
                   this.删除图层ToolStripMenuItem.Visible = false;//显示加载数据菜单选项
                   this.属性ToolStripMenuItem1.Visible = true;
                   this.添加数据ToolStripMenuItem.Visible = true;
                   this.缩放至图层ToolStripMenuItem.Visible = true;
                   //this.ToolStripMenuItem_DeleteLayer.Visible = false;//不显示删除图层菜单选项
               }
               else
              {
                   //控制菜单项的显示隐藏
                  //this.删除图层ToolStripMenuItem.Visible = false;//不显示加载数据
                  this.删除图层ToolStripMenuItem.Visible = true;//显示删除图层菜单选项
                  this.属性ToolStripMenuItem1.Visible = true;
                  this.添加数据ToolStripMenuItem.Visible = true;
                  this.缩放至图层ToolStripMenuItem.Visible = true;
               }  
               this.contextMenuStrip1.Show(this.tocCtrl, new System.Drawing.Point(e.X, e.Y)); //右键菜单显示
           }
        }


        void mapCtrl_MouseMove(object sender, MouseEventArgs e)
        {
            var mapPnt = mapCtrl.ToMapPoint(e.X, e.Y);
            var b = mapPnt.X.ToString();
            labLayerCount.Text = b;
        }
        //绘制点
        private void btnDrawShape_Click(object sender, EventArgs e)
        {
            var pnt = new PIE.Geometry.Point();
            pnt.PutCoords(106, 32);
            PIE.Display.ISymbol sym = new PIE.Display.SimpleMarkerSymbol();
            mapCtrl.DrawShape(pnt as IGeometry, sym);
            mapCtrl.CenterAt(pnt);
            mapCtrl.PartialRefresh(PIE.Carto.ViewDrawPhaseType.ViewAll);
           
            ;
        }

        private void toolAddDataCoomand_Click(object sender, EventArgs e)
        {
            //实例化一个命令对象
            ICommand cmd = new VectorCommand();
            //创建命令对象，绑定参数
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
        }
        private void toolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
        {
            ;
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            var tool = new MapZoomInTool();
            var cmd = tool as ICommand;
            cmd.OnCreate(mapCtrl);
            mapCtrl.CurrentTool = tool;
        }

        private void bntZoomout_Click(object sender, EventArgs e)
        {
            var tool = new MapZoomOutTool();
            var cmd = tool as ICommand;
            cmd.OnCreate(mapCtrl);
            mapCtrl.CurrentTool = tool;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            var tool = new PanTool();
            var cmd = tool as ICommand;
            cmd.OnCreate(mapCtrl);
            mapCtrl.CurrentTool = tool;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            var cmd = new FullExtentCommand();
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
        }

        private void btnDseCommand_Click(object sender, EventArgs e)
        {
            //1.调用功能插件中的窗体
            FrmPIECalibration frmPIECalibration = new FrmPIECalibration();
            frmPIECalibration.FindForm();
            //var exchangeIno = frm.ExChangeData;
            if (frmPIECalibration.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            //辐射定标算法实现
            //1.创建算法对象
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.CalibrationAlgo");
            //PIE.CommonAlgo.CalibrationAlgo
            algo.Name = "CalibrationAlgo";
            algo.Description = "辐射定标算法";

            //2.设置算法参数
            algo.Params = frmPIECalibration.ExChangeData;

            //2输入算法参数
            //var info = new DataPreCali_Exchange_Info();
            //3注册算法事件
            var algoEvent = algo as ISystemAlgoEvents;
            algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
            algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
            //4.执行算法
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }
        //算法完成事件
        void algoEvent_OnExecuteCompleted(ISystemAlgo algo)
        {
            progressBar.Value = 0;
            labProMsg.Text = "";
            var eCode = -1;
            var eMsg = "";
            algo.GetErrorInfo(ref eCode, ref eMsg);
            if (eCode != 0)
            {
                MessageBox.Show(algo.Name + "实行失败！");
                return;
            }
            else
            {
                var info = algo.Params as DataPreCali_Exchange_Info;
                if (info == null) return;
                var outFile = info.OutputFilePath;
                var layer = LayerFactory.CreateDefaultLayer(outFile);
                mapCtrl.FocusMap.AddLayer(layer);
                mapCtrl.ActiveView.PartialRefresh(ViewDrawPhaseType.ViewAll);
            }


        }
        //算法进度事件
        int algoEvent_OnProgressChanged(double complete, string msg, ISystemAlgo algo)
        {
            progressBar.Value = Convert.ToInt32(complete);
            //算法进度消息
            labProMsg.Text = msg;
            return 1;
        }


        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            FrmISODataClassification frmISODataClassification = new FrmISODataClassification();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", " PIE.CommonAlgo.ISODataClassificationAlgo");
            algo.Name = "ISODataClassificationAlgo";
            algo.Description = "非监督分类";
            algo.Params = frmISODataClassification.ExChangeData;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

            var ofd = new OpenFileDialog();
            ofd.Filter = "ShapeFile|*.shp";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var file = ofd.FileName;
                var layer = PIE.Carto.LayerFactory.CreateDefaultLayer(file);
                if (layer == null)
                    return;
                mapCtrl.AddLayer(layer, 0);
            }
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }




        string open_file()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Title = "请选择文件";
            dialog.Filter = "所有文件(*.*)|*.*";
            string file_name = null;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                file_name = dialog.FileName;
            }
            if (file_name == null)
            {
                MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                DialogResult dr = MessageBox.Show("确定要退出吗?", "退出选择", messButton);
                if (dr == DialogResult.OK)//如果点击“确定”按钮
                {
                    ;
                }
                else { open_file(); }
            }
            return file_name;
        }
        string save_file()
        {
            SaveFileDialog pSaveFileDialog = new SaveFileDialog();
            pSaveFileDialog.Title = "选择存储路径";
            pSaveFileDialog.Filter = "Tif文件(*.tif)|*.tif";
            string full_name = null;
            if (pSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                full_name = pSaveFileDialog.FileName;
            }
            if (full_name == null)
            {
                MessageBox.Show("请输入保存文件名");
                open_file();
            }
            return full_name;
        }
        void show_file(string file_name)
        {
            ILayer layer = PIE.Carto.LayerFactory.CreateDefaultLayer(@file_name);
            mapCtrl.ActiveView.FocusMap.AddLayer(layer);
            mapCtrl.ActiveView.PartialRefresh(ViewDrawPhaseType.ViewAll);
        }


        private void ndvi1_Click(object sender, EventArgs e)
        {
            string open_name = open_file();

            if (open_name != null)
            {
                string save_name = save_file();
                MessageBox.Show("开始计算");
                PIE.CommonAlgo.BandOper_Exchange_Info info = new PIE.CommonAlgo.BandOper_Exchange_Info();
                //info.StrExp == "(b1-b2)/(b1+b2)";(((b1-b2)/(b1+b2))>0.8)*(((b1-b2)/(b1+b2))+ (((b1-b2)/(b1+b2))<=0.8)*0
                info.StrExp = "(((b1-b2)/(b1+b2))>0.2)*((b1-b2)/(b1+b2))+ (((b1-b2)/(b1+b2))<=0.2)*0";
                
                info.SelectFileBands = new List<int> { 5, 4 };
                info.SelectFileNames = new List<string> { @open_name, @open_name };
                info.OutputFilePath = @save_name;
                info.FileTypeCode = "GTiff";
                info.PixelDataType = 7;

                PIE.SystemAlgo.ISystemAlgo algo = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                if (algo == null) return;

                PIE.SystemAlgo.ISystemAlgoEvents algoEvents = algo as PIE.SystemAlgo.ISystemAlgoEvents;
                algo.Name = "PIE.CommonAlgo.BandOperAlgo";
                algo.Params = info;

                //进度条
                var algoEvent = algo as ISystemAlgoEvents;
                algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
                algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                show_file(open_name);
                show_file(save_name);

                //MessageBox.Show("计算完成");
                //string save_name1 = save_file();
                //MessageBox.Show("开始计算");
                //PIE.CommonAlgo.BandOper_Exchange_Info info1 = new PIE.CommonAlgo.BandOper_Exchange_Info();
                //info1.StrExp = "(b1-b2)/(b1+b2)";
                //info1.SelectFileBands = new List<int> { 5, 4 };
                //info1.SelectFileNames = new List<string> { @open_name, @open_name };
                //info.OutputFilePath = @save_name;
                //info.FileTypeCode = "GTiff";
                //info.PixelDataType = 7;

                //PIE.SystemAlgo.ISystemAlgo algo1 = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                //if (algo == null) return;

                //PIE.SystemAlgo.ISystemAlgoEvents algoEvents1 = algo1 as PIE.SystemAlgo.ISystemAlgoEvents;
                //algo1.Name = "PIE.CommonAlgo.BandOperAlgo";
                //algo1.Params = info;

                ////进度条
                //var algoEvent1 = algo as ISystemAlgoEvents;
                //algoEvent1.OnProgressChanged += algoEvent_OnProgressChanged;
                //algoEvent1.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                ////执行
                //PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                //show_file(open_name);
                //show_file(save_name);
                
                //MessageBox.Show("计算完成");
            }

        }

        private void EVI_Click_1(object sender, EventArgs e)
        {

            string open_name = open_file();

            if (open_name != null)
            {
                string save_name = save_file();
                MessageBox.Show("开始计算");
                PIE.CommonAlgo.BandOper_Exchange_Info info = new PIE.CommonAlgo.BandOper_Exchange_Info();
                info.StrExp = "2.5*(b1-b2)/(b1+6*b2-7.5*b3+1)";
                info.SelectFileBands = new List<int> { 5, 4, 2 };
                info.SelectFileNames = new List<string> { @open_name, @open_name, @open_name };
                info.OutputFilePath = @save_name;
                info.FileTypeCode = "GTiff";
                info.PixelDataType = 7;

                PIE.SystemAlgo.ISystemAlgo algo = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                if (algo == null) return;

                PIE.SystemAlgo.ISystemAlgoEvents algoEvents = algo as PIE.SystemAlgo.ISystemAlgoEvents;
                algo.Name = "PIE.CommonAlgo.BandOperAlgo";
                algo.Params = info;

                //进度条
                var algoEvent = algo as ISystemAlgoEvents;
                algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
                algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                //执行
                PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                show_file(open_name);
                show_file(save_name);

                MessageBox.Show("计算完成");
            }
        }

        private void ndwi_Click_1(object sender, EventArgs e)
        {

            string open_name = open_file();

            if (open_name != null)
            {
                string save_name = save_file();
                MessageBox.Show("开始计算");
                PIE.CommonAlgo.BandOper_Exchange_Info info = new PIE.CommonAlgo.BandOper_Exchange_Info();
                info.StrExp = "(((b1-b2)/(b1+b2))>0.5)*((b1-b2)/(b1+b2))+ (((b1-b2)/(b1+b2))<=0.5)*0";
                info.SelectFileBands = new List<int> { 3, 5 };
                info.SelectFileNames = new List<string> { @open_name, @open_name };
                info.OutputFilePath = @save_name;
                info.FileTypeCode = "GTiff";
                info.PixelDataType = 7;

                PIE.SystemAlgo.ISystemAlgo algo = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                if (algo == null) return;

                PIE.SystemAlgo.ISystemAlgoEvents algoEvents = algo as PIE.SystemAlgo.ISystemAlgoEvents;
                algo.Name = "PIE.CommonAlgo.BandOperAlgo";
                algo.Params = info;

                //进度条
                var algoEvent = algo as ISystemAlgoEvents;
                algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
                algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                //执行
                PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                show_file(open_name);
                show_file(save_name);

                MessageBox.Show("计算完成");
            }
        }

        private void mndwi_Click_1(object sender, EventArgs e)
        {

            string open_name = open_file();

            if (open_name != null)
            {
                string save_name = save_file();
                MessageBox.Show("开始计算");
                PIE.CommonAlgo.BandOper_Exchange_Info info = new PIE.CommonAlgo.BandOper_Exchange_Info();
                info.StrExp = "(b1-b2)/(b1+b2)";
                info.SelectFileBands = new List<int> { 3, 6 };
                info.SelectFileNames = new List<string> { @open_name, @open_name };
                info.OutputFilePath = @save_name;
                info.FileTypeCode = "GTiff";
                info.PixelDataType = 7;

                PIE.SystemAlgo.ISystemAlgo algo = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                if (algo == null) return;

                PIE.SystemAlgo.ISystemAlgoEvents algoEvents = algo as PIE.SystemAlgo.ISystemAlgoEvents;
                algo.Name = "PIE.CommonAlgo.BandOperAlgo";
                algo.Params = info;

                //进度条
                var algoEvent = algo as ISystemAlgoEvents;
                algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
                algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                //执行
                PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                show_file(open_name);
                show_file(save_name);

                MessageBox.Show("计算完成");
            }
        }

        private void wet_Click_1(object sender, EventArgs e)
        {
            string open_name = open_file();

            if (open_name != null)
            {
                string save_name = save_file();
                MessageBox.Show("开始计算");
                PIE.CommonAlgo.BandOper_Exchange_Info info = new PIE.CommonAlgo.BandOper_Exchange_Info();
                info.StrExp = "0.0315*b1+0.2021*b2+0.1594*b3-0.6806*b4-0.6109*b5";
                info.SelectFileBands = new List<int> { 2, 3, 5, 6, 7 };
                info.SelectFileNames = new List<string> { @open_name, @open_name, @open_name, @open_name, @open_name };
                info.OutputFilePath = @save_name;
                info.FileTypeCode = "GTiff";
                info.PixelDataType = 7;

                PIE.SystemAlgo.ISystemAlgo algo = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                if (algo == null) return;

                PIE.SystemAlgo.ISystemAlgoEvents algoEvents = algo as PIE.SystemAlgo.ISystemAlgoEvents;
                algo.Name = "PIE.CommonAlgo.BandOperAlgo";
                algo.Params = info;

                //进度条
                var algoEvent = algo as ISystemAlgoEvents;
                algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
                algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                //执行
                PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                show_file(open_name);
                show_file(save_name);

                MessageBox.Show("计算完成");
            }
        }



        private void ndbi_Click_1(object sender, EventArgs e)
        {

            string open_name = open_file();

            if (open_name != null)
            {
                string save_name = save_file();
                MessageBox.Show("开始计算");
                PIE.CommonAlgo.BandOper_Exchange_Info info = new PIE.CommonAlgo.BandOper_Exchange_Info();
                info.StrExp = "(((b1-b2)/(b1+b2))>(-0.1))*((b1-b2)/(b1+b2))+ (((b1-b2)/(b1+b2))<=(-0.1))*0";
                info.SelectFileBands = new List<int> { 6, 5 };
                info.SelectFileNames = new List<string> { @open_name, @open_name };
                info.OutputFilePath = @save_name;
                info.FileTypeCode = "GTiff";
                info.PixelDataType = 7;

                PIE.SystemAlgo.ISystemAlgo algo = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                if (algo == null) return;

                PIE.SystemAlgo.ISystemAlgoEvents algoEvents = algo as PIE.SystemAlgo.ISystemAlgoEvents;
                algo.Name = "PIE.CommonAlgo.BandOperAlgo";
                algo.Params = info;

                //进度条
                var algoEvent = algo as ISystemAlgoEvents;
                algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
                algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                //执行
                PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                show_file(open_name);
                show_file(save_name);

                MessageBox.Show("计算完成");
            }
        }

        private void 主成分正变化_Click(object sender, EventArgs e)
        {
            FrmPCA frmISODataClassification = new FrmPCA();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.TransformForwardPCAAlgo");

            algo.Name = "TransformForwardPCAAlgo";
            algo.Description = "主成分正变化";
            algo.Params = frmISODataClassification.ExChangeData;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }

        private void 主成分逆变化_Click(object sender, EventArgs e)
        {
            FrmPCAInv frmISODataClassification = new FrmPCAInv();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.TransformInversePCAAlgo");

            algo.Name = "TransformForwardPCAAlgo";
            algo.Description = "主成分逆变化";
            algo.Params = frmISODataClassification.ExChangeData;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }



        private void 天地图矢量底图_Click(object sender, EventArgs e)
        {
            ICommand cmd = new TDTTiledVecCommand();
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
        }

        private void 天地图矢量注记_Click(object sender, EventArgs e)
        {
            ICommand cmd = new TDTTiledCvaCommand();
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
        }

        private void 天地图影像底图_Click(object sender, EventArgs e)
        {
            ICommand cmd = new TDTTiledImgCommand();
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
        }

        private void 天地图影像注记_Click(object sender, EventArgs e)
        {

            ICommand cmd = new TDTTiledCiaCommand();
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
        }

        private void 神经网络聚类_Click(object sender, EventArgs e)
        {
           FrmNeuralNetworkCluster frmISODataClassification = new FrmNeuralNetworkCluster();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.NeuralNetworkClusterAlgo");
            
            algo.Name = "AtmosphericCorrectionAlgo";
            algo.Params = frmISODataClassification.ExChangeData;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }

        private void 矢量底图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand cmd = new TDTTiledVecCommand();
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
        }

        private void 矢量注记ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand cmd = new TDTTiledCvaCommand();
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
        }

        private void 影像底图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand cmd = new TDTTiledImgCommand();
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
        }

        private void 影像注记ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand cmd = new TDTTiledCiaCommand();
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
        }

        private void 添加矢量数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //添加数据
            var ofd = new OpenFileDialog();
            ofd.Filter = "ShapeFile|*.shp";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var file = ofd.FileName;
                var layer = PIE.Carto.LayerFactory.CreateDefaultLayer(file);
                if (layer == null)
                    return;
                mapCtrl.AddLayer(layer, 0);

            }
        }

        private void 添加栅格数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand cmd = new RasterCommand();
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
        }

        private void 删除数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapCtrl.DeleteLayer(0);
            mapCtrl.PartialRefresh(PIE.Carto.ViewDrawPhaseType.ViewAll);
        }

        private void 清除数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapCtrl.ClearLayers();
            mapCtrl.PartialRefresh(PIE.Carto.ViewDrawPhaseType.ViewAll);
        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {

        }

        private void 添加科学数据集ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand cmd = new ScientificDatasetCommand();
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
        }

        private void 辐射定标ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1.调用功能插件中的窗体
            FrmPIECalibration frmPIECalibration = new FrmPIECalibration();
            frmPIECalibration.FindForm();
            //var exchangeIno = frm.ExChangeData;
            if (frmPIECalibration.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            //辐射定标算法实现
            //1.创建算法对象
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.CalibrationAlgo");
            //PIE.CommonAlgo.CalibrationAlgo
            algo.Name = "CalibrationAlgo";
            algo.Description = "辐射定标算法";

            //2.设置算法参数
            algo.Params = frmPIECalibration.ExChangeData;

            //2输入算法参数
            //var info = new DataPreCali_Exchange_Info();
            //3注册算法事件
            var algoEvent = algo as ISystemAlgoEvents;
            algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
            algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
            //4.执行算法
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }

        private void 大气校正ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FrmAtmosphericCorrection frmISODataClassification = new FrmAtmosphericCorrection();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", " PIE.CommonAlgo.AtmosphericCorrectionAlgo");

            algo.Name = "AtmosphericCorrectionAlgo";
            algo.Params = frmISODataClassification.ExChangeData;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }

        private void 内部平均法定标ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmCali_InnerMean frmISODataClassification = new FrmCali_InnerMean();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.Plugin.dll", " PIE.Plugin.Cali_InnerMeanCommand");
            algo.Name = "ImgClassCombineAlgo";
            algo.Description = "kmeans";
            algo.Params = frmISODataClassification.ExChangeData;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }


        private void 正射校正ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmPIEOrtho frmISODataClassification = new FrmPIEOrtho();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.Plugin.dll", " PIE.Plugin.PIEOrthoCommand");

            algo.Name = "ImgClassCombineAlgo";
            algo.Description = "kmeans";
            algo.Params = frmISODataClassification.ExChangeData;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }

        private void 影像裁剪ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmImageFastMosaic frmISODataClassification = new FrmImageFastMosaic();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.ImageMosaicParamAlgo");
            algo.Name = "TransformForwardPCAAlgo";
            algo.Description = "影像裁剪";
            algo.Params = frmISODataClassification.ExChangeData;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }

        private void 快速拼接ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmImageFastMosaic frmISODataClassification = new FrmImageFastMosaic();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.ImageMosaicParamAlgo");
            algo.Name = "TransformForwardPCAAlgo";
            algo.Description = "快速拼接";
            algo.Params = frmISODataClassification.ExChangeData;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }

        private void iSODATA分类ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmISODataClassification frmISODataClassification = new FrmISODataClassification();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", " PIE.CommonAlgo.ISODataClassificationAlgo");
            algo.Name = "ISODataClassificationAlgo";
            algo.Description = "非监督分类";
            algo.Params = frmISODataClassification.ExChangeData;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }

        private void kMeans分类ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //PIE.Plugin.ImagePreProcess.dll
            FrmKmeansClassification frmISODataClassification = new FrmKmeansClassification();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", " PIE.CommonAlgo.KmeansClassificationAlgo");

            algo.Name = "KmeansClassificationAlgo";
            algo.Description = "kmeans";
            algo.Params = frmISODataClassification.ExChangeData;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }

        private void 最大似自然发呢类ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand cmd = new zuida();
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
        }

        private void 距离分类ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand cmd = new juli();
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
        }

        private void 常用滤波ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmImgProFiltCommon frmISODataClassification = new FrmImgProFiltCommon();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", " PIE.CommonAlgo.ImgProFiltCommonAlgo");
            algo.Name = "KmeansClassificationAlgo";
            algo.Description = "kmeans";
            algo.Params = frmISODataClassification.ExChangeData;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }

        private void 微分锐化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmImgProFiltDiffSharp frmISODataClassification = new FrmImgProFiltDiffSharp();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", " PIE.CommonAlgo.ImgProFiltDiffSharpAlgo");
            
            algo.Name = "KmeansClassificationAlgo";
            algo.Description = "kmeans";
            algo.Params = frmISODataClassification.ExChangeData;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }

        private void 定向滤波ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmImgProFiltCommon frmISODataClassification = new FrmImgProFiltCommon();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", " PIE.CommonAlgo.ImgProFiltCommonAlgo");
            algo.Name = "KmeansClassificationAlgo";
            algo.Description = "kmeans";
            algo.Params = frmISODataClassification.ExChangeData;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }

        private void 频率域滤波ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmImgProFiltFrequency frmISODataClassification = new FrmImgProFiltFrequency();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", " PIE.CommonAlgo.ImgProFiltFrequencyAlgo");
            
            algo.Name = "KmeansClassificationAlgo";
            algo.Description = "kmeans";
            algo.Params = frmISODataClassification.ExChangeData;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }

        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            FrmImgClassCombine frmISODataClassification = new FrmImgClassCombine();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", " PIE.CommonAlgo.ImgClassCombineAlgo");

            algo.Name = "ImgClassCombineAlgo";
            algo.Description = "kmeans";
            algo.Params = frmISODataClassification.ExChangeData;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }

        private void 矢量转栅格ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmRasterToVector frmISODataClassification = new FrmRasterToVector();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", " PIE.CommonAlgo.RasterToVectorAlgo");

            algo.Name = "RasterToVectorAlgo";
            algo.Description = "kmeans";
            algo.Params = frmISODataClassification.ExChangeData;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }

        private void 栅格转矢量ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmVectorToRaster frmISODataClassification = new FrmVectorToRaster();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", " PIE.CommonAlgo.VectorToRasterAlgo");
            algo.Name = "RasterToVectorAlgo";
            algo.Description = "kmeans";
            algo.Params = frmISODataClassification.ExChangeData;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }

        private void 波段合成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IMap map = mapCtrl.FocusMap;
            FrmBandCombination frmISODataClassification = new FrmBandCombination(map);
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", " PIE.CommonAlgo.BandCombinationAlgo");
            
            algo.Name = "RasterToVectorAlgo";
            algo.Description = "kmeans";
            algo.Params = frmISODataClassification.m_AlgoParam;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }

        private void toolStripButton3_Click_1(object sender, EventArgs e)
        {
            string open_name = open_file();

            if (open_name != null)
            {
                    string save_name = save_file();
                    MessageBox.Show("开始计算");
                    PIE.CommonAlgo.BandOper_Exchange_Info info = new PIE.CommonAlgo.BandOper_Exchange_Info();
                    info.StrExp = "((((b1-b2)/(b1+b2))>=(-0.03))AND(((b1-b2)/(b1+b2))<=(-0.015)))*((b1-b2)/(b1+b2)) + ((((b1-b2)/(b1+b2))<=(-0.03))OR(((b1-b2)/(b1+b2))>=(-0.015)))*0";
                    info.SelectFileBands = new List<int> {2,3 };
                    info.SelectFileNames = new List<string> { @open_name, @open_name };
                    info.OutputFilePath = @save_name;
                    info.FileTypeCode = "GTiff";
                    info.PixelDataType = 7;

                    PIE.SystemAlgo.ISystemAlgo algo = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                    if (algo == null) return;

                    PIE.SystemAlgo.ISystemAlgoEvents algoEvents = algo as PIE.SystemAlgo.ISystemAlgoEvents;
                    algo.Name = "PIE.CommonAlgo.BandOperAlgo";
                    algo.Params = info;

                    //进度条
                    var algoEvent = algo as ISystemAlgoEvents;
                    algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
                    algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                    //执行
                    PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                    show_file(open_name);
                    show_file(save_name);

                    MessageBox.Show("计算完成");
                }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            string open_name = open_file();

            if (open_name != null)
            {
                    string save_name = save_file();
                    MessageBox.Show("开始计算");
                    PIE.CommonAlgo.BandOper_Exchange_Info info = new PIE.CommonAlgo.BandOper_Exchange_Info();
                    info.StrExp = "(b1-b2)/(b1+b2)";
                    info.SelectFileBands = new List<int> {4,6 };
                    info.SelectFileNames = new List<string> { @open_name, @open_name };
                    info.OutputFilePath = @save_name;
                    info.FileTypeCode = "GTiff";
                    info.PixelDataType = 7;

                    PIE.SystemAlgo.ISystemAlgo algo = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                    if (algo == null) return;

                    PIE.SystemAlgo.ISystemAlgoEvents algoEvents = algo as PIE.SystemAlgo.ISystemAlgoEvents;
                    algo.Name = "PIE.CommonAlgo.BandOperAlgo";
                    algo.Params = info;

                    //进度条
                    var algoEvent = algo as ISystemAlgoEvents;
                    algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
                    algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                    //执行
                    PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                    show_file(open_name);
                    show_file(save_name);

                    MessageBox.Show("计算完成");
                }
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            string open_name = open_file();

            if (open_name != null)
            {
                string save_name = save_file();
                MessageBox.Show("开始计算");
                PIE.CommonAlgo.BandOper_Exchange_Info info = new PIE.CommonAlgo.BandOper_Exchange_Info();
                info.StrExp = "(b1+b2+b3)/3";
                info.SelectFileBands = new List<int> { 3,4,5 };
                info.SelectFileNames = new List<string> { @open_name, @open_name };
                info.OutputFilePath = @save_name;
                info.FileTypeCode = "GTiff";
                info.PixelDataType = 7;

                PIE.SystemAlgo.ISystemAlgo algo = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                if (algo == null) return;

                PIE.SystemAlgo.ISystemAlgoEvents algoEvents = algo as PIE.SystemAlgo.ISystemAlgoEvents;
                algo.Name = "PIE.CommonAlgo.BandOperAlgo";
                algo.Params = info;

                //进度条
                var algoEvent = algo as ISystemAlgoEvents;
                algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
                algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                //执行
                PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                show_file(open_name);
                show_file(save_name);

                MessageBox.Show("计算完成");
            }
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            
            ICommand cmd = new AddPopulationDataCommand();
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripDropDownButton6_Click(object sender, EventArgs e)
        {

        }

        private void 植被盖度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string open_name = open_file();

            if (open_name != null)
            {
                string save_name = save_file();
                MessageBox.Show("开始计算");
                PIE.CommonAlgo.BandOper_Exchange_Info info = new PIE.CommonAlgo.BandOper_Exchange_Info();
                //info.StrExp == "(b1-b2)/(b1+b2)";(((b1-b2)/(b1+b2))>0.8)*(((b1-b2)/(b1+b2))+ (((b1-b2)/(b1+b2))<=0.8)*0
                info.StrExp = "(b1-b2)/(b1+b2)";

                info.SelectFileBands = new List<int> { 5, 4 };
                info.SelectFileNames = new List<string> { @open_name, @open_name };
                info.OutputFilePath = @save_name;
                info.FileTypeCode = "GTiff";
                info.PixelDataType = 7;

                PIE.SystemAlgo.ISystemAlgo algo = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                if (algo == null) return;

                PIE.SystemAlgo.ISystemAlgoEvents algoEvents = algo as PIE.SystemAlgo.ISystemAlgoEvents;
                algo.Name = "PIE.CommonAlgo.BandOperAlgo";
                algo.Params = info;

                //进度条
                var algoEvent = algo as ISystemAlgoEvents;
                algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
                algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                show_file(open_name);
                show_file(save_name);

                //MessageBox.Show("计算完成");
                //string save_name1 = save_file();
                //MessageBox.Show("开始计算");
                //PIE.CommonAlgo.BandOper_Exchange_Info info1 = new PIE.CommonAlgo.BandOper_Exchange_Info();
                //info1.StrExp = "(b1-b2)/(b1+b2)";
                //info1.SelectFileBands = new List<int> { 5, 4 };
                //info1.SelectFileNames = new List<string> { @open_name, @open_name };
                //info.OutputFilePath = @save_name;
                //info.FileTypeCode = "GTiff";
                //info.PixelDataType = 7;

                //PIE.SystemAlgo.ISystemAlgo algo1 = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                //if (algo == null) return;

                //PIE.SystemAlgo.ISystemAlgoEvents algoEvents1 = algo1 as PIE.SystemAlgo.ISystemAlgoEvents;
                //algo1.Name = "PIE.CommonAlgo.BandOperAlgo";
                //algo1.Params = info;

                ////进度条
                //var algoEvent1 = algo as ISystemAlgoEvents;
                //algoEvent1.OnProgressChanged += algoEvent_OnProgressChanged;
                //algoEvent1.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                ////执行
                //PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                //show_file(open_name);
                //show_file(save_name);

                //MessageBox.Show("计算完成");
            }
        }

        private void eVIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string open_name = open_file();

            if (open_name != null)
            {
                string save_name = save_file();
                MessageBox.Show("开始计算");
                PIE.CommonAlgo.BandOper_Exchange_Info info = new PIE.CommonAlgo.BandOper_Exchange_Info();
                info.StrExp = "2.5*(b1-b2)/(b1+6*b2-7.5*b3+1)";
                info.SelectFileBands = new List<int> { 5, 4, 2 };
                info.SelectFileNames = new List<string> { @open_name, @open_name, @open_name };
                info.OutputFilePath = @save_name;
                info.FileTypeCode = "GTiff";
                info.PixelDataType = 7;

                PIE.SystemAlgo.ISystemAlgo algo = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                if (algo == null) return;

                PIE.SystemAlgo.ISystemAlgoEvents algoEvents = algo as PIE.SystemAlgo.ISystemAlgoEvents;
                algo.Name = "PIE.CommonAlgo.BandOperAlgo";
                algo.Params = info;

                //进度条
                var algoEvent = algo as ISystemAlgoEvents;
                algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
                algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                //执行
                PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                show_file(open_name);
                show_file(save_name);

                MessageBox.Show("计算完成");
            }
        }

        private void 建筑景观聚类ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string open_name = open_file();

            if (open_name != null)
            {
                string save_name = save_file();
                MessageBox.Show("开始计算");
                PIE.CommonAlgo.BandOper_Exchange_Info info = new PIE.CommonAlgo.BandOper_Exchange_Info();
                info.StrExp = "(((b1-b2)/(b1+b2))>(-0.03))*((b1-b2)/(b1+b2))+ (((b1-b2)/(b1+b2))<=(-0.03))*0";
                info.SelectFileBands = new List<int> { 6, 5 };
                info.SelectFileNames = new List<string> { @open_name, @open_name };
                info.OutputFilePath = @save_name;
                info.FileTypeCode = "GTiff";
                info.PixelDataType = 7;

                PIE.SystemAlgo.ISystemAlgo algo = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                if (algo == null) return;

                PIE.SystemAlgo.ISystemAlgoEvents algoEvents = algo as PIE.SystemAlgo.ISystemAlgoEvents;
                algo.Name = "PIE.CommonAlgo.BandOperAlgo";
                algo.Params = info;

                //进度条
                var algoEvent = algo as ISystemAlgoEvents;
                algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
                algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                //执行
                PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                show_file(open_name);
                show_file(save_name);

                MessageBox.Show("计算完成");
            }
        }

        private void 水体提取ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string open_name = open_file();

            if (open_name != null)
            {
                string save_name = save_file();
                MessageBox.Show("开始计算");
                PIE.CommonAlgo.BandOper_Exchange_Info info = new PIE.CommonAlgo.BandOper_Exchange_Info();
                info.StrExp = "(((b1-b2)/(b1+b2))>0.5)*((b1-b2)/(b1+b2))+ (((b1-b2)/(b1+b2))<=0.5)*0";
                info.SelectFileBands = new List<int> { 3, 5 };
                info.SelectFileNames = new List<string> { @open_name, @open_name };
                info.OutputFilePath = @save_name;
                info.FileTypeCode = "GTiff";
                info.PixelDataType = 7;

                PIE.SystemAlgo.ISystemAlgo algo = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                if (algo == null) return;

                PIE.SystemAlgo.ISystemAlgoEvents algoEvents = algo as PIE.SystemAlgo.ISystemAlgoEvents;
                algo.Name = "PIE.CommonAlgo.BandOperAlgo";
                algo.Params = info;

                //进度条
                var algoEvent = algo as ISystemAlgoEvents;
                algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
                algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                //执行
                PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                show_file(open_name);
                show_file(save_name);

                MessageBox.Show("计算完成");
            }
        }

        private void 湿度监测ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string open_name = open_file();

            if (open_name != null)
            {
                string save_name = save_file();
                MessageBox.Show("开始计算");
                PIE.CommonAlgo.BandOper_Exchange_Info info = new PIE.CommonAlgo.BandOper_Exchange_Info();
                info.StrExp = "0.0315*b1+0.2021*b2+0.1594*b3-0.6806*b4-0.6109*b5";
                info.SelectFileBands = new List<int> { 2, 3, 5, 6, 7 };
                info.SelectFileNames = new List<string> { @open_name, @open_name, @open_name, @open_name, @open_name };
                info.OutputFilePath = @save_name;
                info.FileTypeCode = "GTiff";
                info.PixelDataType = 7;

                PIE.SystemAlgo.ISystemAlgo algo = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                if (algo == null) return;

                PIE.SystemAlgo.ISystemAlgoEvents algoEvents = algo as PIE.SystemAlgo.ISystemAlgoEvents;
                algo.Name = "PIE.CommonAlgo.BandOperAlgo";
                algo.Params = info;

                //进度条
                var algoEvent = algo as ISystemAlgoEvents;
                algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
                algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                //执行
                PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                show_file(open_name);
                show_file(save_name);

                MessageBox.Show("计算完成");
            }
        }

        private void 黑臭水体监测ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string open_name = open_file();

            if (open_name != null)
            {
                string save_name = save_file();
                MessageBox.Show("开始计算");
                PIE.CommonAlgo.BandOper_Exchange_Info info = new PIE.CommonAlgo.BandOper_Exchange_Info();
                info.StrExp = "((((b1-b2)/(b1+b2))>=(-0.03))AND(((b1-b2)/(b1+b2))<=(-0.015)))*((b1-b2)/(b1+b2)) + ((((b1-b2)/(b1+b2))<=(-0.03))OR(((b1-b2)/(b1+b2))>=(-0.015)))*0";
                info.SelectFileBands = new List<int> { 2, 3 };
                info.SelectFileNames = new List<string> { @open_name, @open_name };
                info.OutputFilePath = @save_name;
                info.FileTypeCode = "GTiff";
                info.PixelDataType = 7;

                PIE.SystemAlgo.ISystemAlgo algo = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                if (algo == null) return;

                PIE.SystemAlgo.ISystemAlgoEvents algoEvents = algo as PIE.SystemAlgo.ISystemAlgoEvents;
                algo.Name = "PIE.CommonAlgo.BandOperAlgo";
                algo.Params = info;

                //进度条
                var algoEvent = algo as ISystemAlgoEvents;
                algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
                algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                //执行
                PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                show_file(open_name);
                show_file(save_name);

                MessageBox.Show("计算完成");
            }
        }

        private void 土壤盐度指数ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string open_name = open_file();

            if (open_name != null)
            {
                string save_name = save_file();
                MessageBox.Show("开始计算");
                PIE.CommonAlgo.BandOper_Exchange_Info info = new PIE.CommonAlgo.BandOper_Exchange_Info();
                info.StrExp = "(b1-b2)/(b1+b2)";
                info.SelectFileBands = new List<int> { 4, 5 };
                info.SelectFileNames = new List<string> { @open_name, @open_name };
                info.OutputFilePath = @save_name;
                info.FileTypeCode = "GTiff";
                info.PixelDataType = 7;

                PIE.SystemAlgo.ISystemAlgo algo = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                if (algo == null) return;

                PIE.SystemAlgo.ISystemAlgoEvents algoEvents = algo as PIE.SystemAlgo.ISystemAlgoEvents;
                algo.Name = "PIE.CommonAlgo.BandOperAlgo";
                algo.Params = info;

                //进度条
                var algoEvent = algo as ISystemAlgoEvents;
                algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
                algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                //执行
                PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                show_file(open_name);
                show_file(save_name);

                MessageBox.Show("计算完成");
            }
        }

        private void 土壤强度指数ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string open_name = open_file();

            if (open_name != null)
            {
                string save_name = save_file();
                MessageBox.Show("开始计算");
                PIE.CommonAlgo.BandOper_Exchange_Info info = new PIE.CommonAlgo.BandOper_Exchange_Info();
                info.StrExp = "(b1+b2+b3)/3";
                info.SelectFileBands = new List<int> { 3, 4, 5 };
                info.SelectFileNames = new List<string> { @open_name, @open_name };
                info.OutputFilePath = @save_name;
                info.FileTypeCode = "GTiff";
                info.PixelDataType = 7;

                PIE.SystemAlgo.ISystemAlgo algo = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                if (algo == null) return;

                PIE.SystemAlgo.ISystemAlgoEvents algoEvents = algo as PIE.SystemAlgo.ISystemAlgoEvents;
                algo.Name = "PIE.CommonAlgo.BandOperAlgo";
                algo.Params = info;

                //进度条
                var algoEvent = algo as ISystemAlgoEvents;
                algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
                algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                //执行
                PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                show_file(open_name);
                show_file(save_name);

                MessageBox.Show("计算完成");
            }
        }

        private void 火点监测ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pM25监测ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //FrmPCA frmISODataClassification = new FrmPCA();
            //if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            //var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.TransformForwardPCAAlgo");

            //algo.Name = "TransformForwardPCAAlgo";
            //algo.Description = "主成分正变化";
            //algo.Params = frmISODataClassification.ExChangeData;
            //AlgoFactory.Instance().AsynExecuteAlgo(algo);
            FrmKrigingInterpolation frmISODataClassification = new FrmKrigingInterpolation();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.KrigingInterpolationAlgo");

            algo.Name = "TransformForwardPCAAlgo";
            algo.Description = "主成分正变化";
            algo.Params = frmISODataClassification.m_ExchangeInfo;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
            var algoEvent = algo as ISystemAlgoEvents;
            algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
        }

        private void toolStripDropDownButton8_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton3_Click_2(object sender, EventArgs e)
        {
            var tool = new PanTool();
            var cmd = tool as ICommand;
            cmd.OnCreate(mapCtrl);
            mapCtrl.CurrentTool = tool;
        }

        private void 属性ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            IMap map = mapCtrl.FocusMap;
            ILayer layer = map.GetLayer(0);
            //2.实例化属性查看窗口对象
            PIE.AxControls.LayerPropertyDialog dlg = new PIE.AxControls.LayerPropertyDialog();
            PIETOCNodeTag pieTOCNodeTag = new PIETOCNodeTag();
            pieTOCNodeTag.Map = map;
            pieTOCNodeTag.Layer = layer;
            ILayer layer1 = pieTOCNodeTag.Layer;
            dlg.Initial(map, layer);

            dlg.ShowDialog();
        }

        private void splitContainer1_Panel1_MouseClick(object sender, MouseEventArgs e)
        {
            var cmd = new LayerPropertyCommand();
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
        }

        private void 属性ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var cmd = new LayerPropertyCommand();
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
        }

        private void splitContainer1_Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            var cmd = new LayerPropertyCommand();
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
        }

        private void axTOCControl1_OnMouseDown(object sender, ESRI.ArcGIS.Controls.ITOCControlEvents_OnMouseDownEvent e)
        {
           
        }

        private void pMToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FrmPCA frmISODataClassification = new FrmPCA();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.KrigingInterpolationAlgo");
            
            algo.Name = "TransformForwardPCAAlgo";
            algo.Description = "主成分正变化";
            algo.Params = frmISODataClassification.ExChangeData;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }

        private void 绿度ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string open_name = open_file();

            if (open_name != null)
            {
                string save_name = save_file();
                MessageBox.Show("开始计算");
                PIE.CommonAlgo.BandOper_Exchange_Info info = new PIE.CommonAlgo.BandOper_Exchange_Info();
                info.StrExp = "(b1-b2)/(b1+b2)";
                info.SelectFileBands = new List<int> { 4, 5 };
                info.SelectFileNames = new List<string> { @open_name, @open_name };
                info.OutputFilePath = @save_name;
                info.FileTypeCode = "GTiff";
                info.PixelDataType = 7;

                PIE.SystemAlgo.ISystemAlgo algo = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                if (algo == null) return;

                PIE.SystemAlgo.ISystemAlgoEvents algoEvents = algo as PIE.SystemAlgo.ISystemAlgoEvents;
                algo.Name = "PIE.CommonAlgo.BandOperAlgo";
                algo.Params = info;

                //进度条
                var algoEvent = algo as ISystemAlgoEvents;
                algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
                algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                //执行
                PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                show_file(open_name);
                show_file(save_name);

                MessageBox.Show("计算完成");
            }
        }

        private void 湿度ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string open_name = open_file();

            if (open_name != null)
            {
                string save_name = save_file();
                MessageBox.Show("开始计算");
                PIE.CommonAlgo.BandOper_Exchange_Info info = new PIE.CommonAlgo.BandOper_Exchange_Info();
                info.StrExp = "0.0315*b1+0.2021*b2+0.1594*b3-0.6806*b4-0.6109*b5";
                info.SelectFileBands = new List<int> { 2, 3, 5, 6, 7 };
                info.SelectFileNames = new List<string> { @open_name, @open_name, @open_name, @open_name, @open_name };
                info.OutputFilePath = @save_name;
                info.FileTypeCode = "GTiff";
                info.PixelDataType = 7;

                PIE.SystemAlgo.ISystemAlgo algo = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                if (algo == null) return;

                PIE.SystemAlgo.ISystemAlgoEvents algoEvents = algo as PIE.SystemAlgo.ISystemAlgoEvents;
                algo.Name = "PIE.CommonAlgo.BandOperAlgo";
                algo.Params = info;

                //进度条
                var algoEvent = algo as ISystemAlgoEvents;
                algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
                algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                //执行
                PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                show_file(open_name);
                show_file(save_name);

                MessageBox.Show("计算完成");
            }
        }

        private void 干度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string open_name = open_file();

            if (open_name != null)
            {
                string save_name = save_file();
                MessageBox.Show("开始计算");
                PIE.CommonAlgo.BandOper_Exchange_Info info = new PIE.CommonAlgo.BandOper_Exchange_Info();
                info.StrExp = "(((2*b6/(b5+b4)-(b4/(b3+b4)+b2/(b2+b5))))/((2*b6/(b5+b4)+(b4/(b3+b4)+b2/(b2+b5))))+((b5-b3)-(b1+b4))/((b5+b3)+(b1+b4)))/2";
                info.SelectFileBands = new List<int> { 2, 3,4 ,5, 6, 7 };
                info.SelectFileNames = new List<string> { @open_name, @open_name, @open_name, @open_name, @open_name, @open_name };
                info.OutputFilePath = @save_name;
                info.FileTypeCode = "GTiff";
                info.PixelDataType = 7;

                PIE.SystemAlgo.ISystemAlgo algo = PIE.SystemAlgo.AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.BandOperAlgo");
                if (algo == null) return;

                PIE.SystemAlgo.ISystemAlgoEvents algoEvents = algo as PIE.SystemAlgo.ISystemAlgoEvents;
                algo.Name = "PIE.CommonAlgo.BandOperAlgo";
                algo.Params = info;

                //进度条
                var algoEvent = algo as ISystemAlgoEvents;
                algoEvent.OnProgressChanged += algoEvent_OnProgressChanged;
                algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;
                //执行
                PIE.SystemAlgo.AlgoFactory.Instance().ExecuteAlgo(algo);

                show_file(open_name);
                show_file(save_name);

                MessageBox.Show("计算完成");
            }
        }

        private void 热度ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void rSEIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmPCA frmISODataClassification = new FrmPCA();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.TransformForwardPCAAlgo");

            algo.Name = "TransformForwardPCAAlgo";
            algo.Description = "主成分正变化";
            algo.Params = frmISODataClassification.ExChangeData;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }

        private void 波段合成ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            IMap map = mapCtrl.FocusMap;
            FrmBandCombination frmISODataClassification = new FrmBandCombination(map);
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", " PIE.CommonAlgo.BandCombinationAlgo");

            algo.Name = "RasterToVectorAlgo";
            algo.Description = "kmeans";
            algo.Params = frmISODataClassification.m_AlgoParam;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
        }

        private void 核密度分析ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmKrigingInterpolation frmISODataClassification = new FrmKrigingInterpolation();
            if (frmISODataClassification.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var algo = AlgoFactory.Instance().CreateAlgo("PIE.CommonAlgo.dll", "PIE.CommonAlgo.KrigingInterpolationAlgo");
            
            algo.Name = "TransformForwardPCAAlgo";
            algo.Description = "主成分正变化";
            algo.Params = frmISODataClassification.m_ExchangeInfo;
            AlgoFactory.Instance().AsynExecuteAlgo(algo);
            var algoEvent = algo as ISystemAlgoEvents;
            algoEvent.OnExecuteCompleted += algoEvent_OnExecuteCompleted;

        }

        private void 热力图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            #region 1参数设置   
            string strFileName = @"F:\\PIESDK\\PIE-SDK博客材料\\PIE示例数据\\矢量数据\\Shape\\Shape\\省会城市.shp";
            IFeatureDataset fDataset = PIE.DataSource.DatasetFactory.OpenFeatureDataset(strFileName);
            if (fDataset.GetGeomType() != GeometryType.GeometryPoint) return;
            HotMapContruct_Exchange_Info info = new HotMapContruct_Exchange_Info();
            info.InputFeatureDataset = fDataset;
            //具体分析
            info.BUseWeightFiled = true;
            info.WeightFeildName = "GDP";//根据数据的权重字段进行设置
            info.Radius = 20;
            info.BCreateFeatureDataset = false;
            info.DeviceWidth = 2000;
            info.DefualtWeightValue = 50;
            info.OutRasterType = "GTIFF";
            info.DeviceWidth = 1000;
            info.CellSize = fDataset.GetExtent().GetWidth() / info.CellSize;
            info.OutRasterFilePath = @"D:\\temp\\省会城市热力图测试.tiff";
            #endregion

            //2、算法执行
            PIE.CommonAlgo.HotMapContructAlgo alog = new HotMapContructAlgo();
            alog.Params = info;
            alog.Execute();
 
            //3、结果显示
            ILayer layer = LayerFactory.CreateDefaultLayer(info.OutRasterFilePath);

            if (layer == null)

            {

                MessageBox.Show("执行失败");

                return;
             }
            mapCtrl.FocusMap.AddLayer(layer);
            mapCtrl.ActiveView.PartialRefresh(ViewDrawPhaseType.ViewAll);

            (fDataset as IDisposable).Dispose();//释放内存

            fDataset = null;
        }

        private void toolStripButton5_Click_1(object sender, EventArgs e)
        {
            ICommand cmd = new NewProjectCommand();
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
            
        }

        private void toolStripButton7_Click_1(object sender, EventArgs e)
        {
            ICommand cmd = new SaveProjectCommand();
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
        }

        private void 删除图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_Layer == null) return;
           mapCtrl.FocusMap.DeleteLayer(m_Layer);
           mapCtrl.ActiveView.PartialRefresh(ViewDrawPhaseType.ViewAll);
        }

        private void toolStripDropDownButton2_Click(object sender, EventArgs e)
        {

        }

        private void 添加数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //添加数据
            var ofd = new OpenFileDialog();
            ofd.Filter = "Shape Files|*.shp;*.000|Raster Files|*.tif;*.tiff;*.dat;*.bmp;*.img;*.jpg|HDF Files|*.hdf;*.h5|NC Files|*.nc";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var file = ofd.FileName;
                var layer = PIE.Carto.LayerFactory.CreateDefaultLayer(file);
                if (layer == null)
                    return;
                mapCtrl.AddLayer(layer, 0);

            }
        }

        private void 缩放至图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cmd = new ZoomToLayerCommand();
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
            
        }

        private void toolStripButton1_Click_2(object sender, EventArgs e)
        {
            var tool = new MapZoomInTool();
            var cmd = tool as ICommand;
            cmd.OnCreate(mapCtrl);
            mapCtrl.CurrentTool = tool;
        }

        private void toolStripButton2_Click_2(object sender, EventArgs e)
        {
            var tool = new MapZoomOutTool();
            var cmd = tool as ICommand;
            cmd.OnCreate(mapCtrl);
            mapCtrl.CurrentTool = tool;
        }

        private void toolStripButton4_Click_1(object sender, EventArgs e)
        {
            var cmd = new FullExtentCommand();
            cmd.OnCreate(mapCtrl);
            cmd.OnClick();
        }

        private void toolStripDropDownButton10_Click(object sender, EventArgs e)
        {

        }


        }



    }
