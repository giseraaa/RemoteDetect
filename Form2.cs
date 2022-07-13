//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using PIE.Geometry;
//using PIE.Controls;
//using PIE.AxControls;
//using PIE.ControlsUI;
//using PIE.SystemUI;
//using PIE.AdapterUI;
//using PIE.Plugin;
//using PIE.CommonAlgo;
//using PIE.SystemAlgo;
//using PIE.Carto;
//using PIE.DataSource;
//namespace PIESDKUser
//{
//    public partial class Form2 : Form
//    {
//        private MapControl mapCtrl;
//        public Form2(PIE.AxControls.MapControl mapCtrl)
//        {
//            InitializeComponent();
//            this.mapCtrl = mapCtrl;
//        }

//        private void Form2_Load(object sender, EventArgs e)
//        {

//            for (int i = 0; i < 10; i++)
//            {
                
//                string name = mapCtrl.GetLayer(i).Name.ToString();
//                if (name.Length > 0)
//                {
//                    this.comboBox1.Items.Add(name);
//                }
//            }
//        }
//        IRasterLayer rasterlayer = new RasterLayer();
//        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            int len = comboBox1.Items.Count;
//            for (int j = 0; j < len; j++)
//            {
//                if (mapCtrl.GetLayer(j).Name.ToString() == this.comboBox1.SelectedItem.ToString())
//                {
//                    rasterlayer = mapCtrl.GetLayer(j) as IRasterLayer;
//                    break;
//                }
//            }
//        }

//        private string full_name;
//        private void button1_Click(object sender, EventArgs e)
//        {
//            SaveFileDialog pSaveFileDialog = new SaveFileDialog();
//            pSaveFileDialog.Title = "选择存储路径";
//            pSaveFileDialog.Filter = "tif文件(*.tif)|*.tif";
//            if (pSaveFileDialog.ShowDialog() == DialogResult.OK)
//            {
//                full_name = pSaveFileDialog.FileName;
//                this.textBox1.Text = pSaveFileDialog.FileName;

//            }
//        }
//        public float[] read_data(IRasterBand band, int x_size, int y_size, PixelDataType type)
//        {
//            float[] buf = new float[x_size * y_size];
//            bool res = band.Read(0, 0, x_size, y_size, buf, x_size, y_size, type);
//            return buf;
//        }

//        private void button2_Click(object sender, EventArgs e)
//        {
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
//            // 创建新栅格
//            IRasterDataset newDataset = DatasetFactory.CreateRasterDataset(full_name, x_size, y_size, 1, type, "GTiff", null);
//            //写入result
//            newDataset.Write(0, 0, x_size, y_size, result, x_size, y_size, type, 1, bandmap);
//            // 设置空间参考
//            newDataset.SpatialReference = dataset.SpatialReference;

//        }

//    }
//}
