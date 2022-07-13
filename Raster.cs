//using PIE.Carto;
//using PIE.Controls;
//using PIE.AxControls;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;
//using PIE.DataSource;

//namespace WindowsFormsApplication1
//{
//    class AddDataCommand : BaseCommand
//    {
//        /// <summary>

//        /// 构造函数

//        /// </summary>
        
//        private MapControl mapCtrl = null;
//        public AddDataCommand()

//        {
//            this.Caption = "指标计算";
//            this.ToolTip = "指标计算";
//            this.Checked = true;
//            this.Enabled = false;
//            // this.Image = "";

//        }
//        /// <summary>

//        /// 创建插件对象

//        /// </summary>

//        /// <param name="hook"></param>
//        public override void OnCreate(Object hook)

//        {
//            if (hook == null) return;
//            if (!(hook is PIE.Carto.IPmdContents)) return;
//            this.Enabled = true;
//            m_Hook = hook;
//            m_HookHelper.Hook = hook;
//            mapCtrl = (MapControl)hook;
//        }


//        /// <summary>

//        /// 点击事件

//        /// </summary>
//        public float[] read_data(IRasterBand band, int x_size, int y_size, PixelDataType type)
//        {
//            float[] buf = new float[x_size * y_size];
//            bool res = band.Read(0, 0, x_size, y_size, buf, x_size, y_size, type);
//            return buf;
//        }

//        public override void OnClick()

//        {
//            IRasterLayer rasterlayer = mapCtrl.GetLayer(0) as IRasterLayer;
//            //Form pForm = new PIESDKUser.Form2((MapControl)m_HookHelper.Hook);
//            //pForm.ShowDialog();
//            IRasterDataset dataset = rasterlayer.Dataset;
//            IRasterBand band_1 = dataset.GetRasterBand(0);
//            IRasterBand band_2 = dataset.GetRasterBand(1);

//            //数据读取函数
//            int x_size = band_1.GetXSize();
//            int y_size = band_1.GetYSize();
//            PixelDataType type = PixelDataType.Float32;

//            float[] buf = read_data(band_1, x_size, y_size, type);
//            float[] buf_1 = read_data(band_2, x_size, y_size, type);

//            int[] bandmap = new int[] { 1 };
//            float[] result = new float[x_size * y_size];
//            for (int i = 0; i < buf.Length; i++)
//            {
//                // 定义中间变量
//                float temp_1 = buf[i] + buf_1[i];
//                float temp_2 = buf[i] - buf_1[i];
//                result[i] = temp_2 / temp_1;
//            }
//            OpenFileDialog openDialog = new OpenFileDialog();
    
//            openDialog.Title = "保存数据";

//            openDialog.Filter = "Raster File|*.img;*.tif;*.dat;*.tiff|Shape File|*.shp|所有文件|*.*";

//            if (openDialog.ShowDialog() != DialogResult.OK) return;

//            // 创建新栅格
//            IRasterDataset newDataset = DatasetFactory.CreateRasterDataset(openDialog.FileName, x_size, y_size, 1, type, "GTiff", null);
//            //写入result
//            newDataset.Write(0, 0, x_size, y_size, result, x_size, y_size, type, 1, bandmap);
//            // 设置空间参考
//            newDataset.SpatialReference = dataset.SpatialReference;
//            ILayer layer = newDataset as ILayer;
//            m_HookHelper.ActiveView.FocusMap.AddLayer(layer);
//            m_HookHelper.ActiveView.PartialRefresh(ViewDrawPhaseType.ViewAll);
//        }

//    }

//}