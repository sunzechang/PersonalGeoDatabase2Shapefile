using DotSpatial.Controls;
using GeoAPI.Geometries;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using NetTopologySuite.Geometries;
using NG = NetTopologySuite.Geometries;
using DotSpatial.Data;
using DotSpatial.Projections;
using SZC.IO.GeoMDB;

namespace CRSDsys
{
    public partial class GeoMdb2ShapefileForm : Form
    {
        ReadPersonalGeoDB pGeoMDBReader;
        public GeoMdb2ShapefileForm()
        {
            InitializeComponent();
        }

        private void btn_Cacnel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btn_SelectMDB_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "个人文件地理数据库(*.mdb)|*.mdb";
            if (openDlg.ShowDialog() != DialogResult.OK) return;
            tbx_mdb.Text = openDlg.FileName;
            tbx_Dir.Text = Path.Combine(Path.GetDirectoryName(openDlg.FileName), Path.GetFileNameWithoutExtension(openDlg.FileName) + "_toShp");

            dataGridView1.Rows.Clear();
            pGeoMDBReader = new ReadPersonalGeoDB();
            bool isOk = pGeoMDBReader.ReadGeoMDB(tbx_mdb.Text);
            if (isOk)
            {
                FeatureClass[] pFeaClasses = pGeoMDBReader.GetAllFeatureClasses();
                foreach (FeatureClass feaClass in pFeaClasses)
                {
                    int idx = dataGridView1.Rows.Add();
                    DataGridViewRow dr = dataGridView1.Rows[idx];
                    dr.Cells["layername"].Value = feaClass.Name;
                    //dr.Cells["datasetName"].Value = feaClass.DatasetName;
                    dr.Cells["GeomType"].Value = Enum.GetName(typeof(EsriGeometryType), feaClass.GeometryType);
                    dr.Tag = feaClass;
                }
                //获取所有要素
            }
            else
            {
                MessageBox.Show("数据库打开失败。");
                return;
            }
        }

        private void btn_SelectDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            if (folderDlg.ShowDialog() != DialogResult.OK) return;
            tbx_Dir.Text = folderDlg.SelectedPath;
        }

        /// <summary>
        /// 含圆形的要素处理不了
        /// </summary>
        /// <param name="pDT"></param>
        /// <param name="shpFile"></param>
        /// <param name="RowIdx"></param>
        /// <param name="endRowIdx"></param>
        /// <returns></returns>
        private void Table2ShapeFile(IFeatureSet feaSet,DataTable pDT,string ShapeColumnName)
        {
            //给要素集添加属性
            List<string> attriNames = new List<string>();
            foreach (DataColumn dataCol in pDT.Columns)
            {
                if (dataCol.ColumnName != ShapeColumnName)
                {
                    attriNames.Add(dataCol.ColumnName);
                    feaSet.DataTable.Columns.Add(dataCol.ColumnName, dataCol.DataType);
                }
            }

            //给要素集添加要素
            IGeometry pGeo = null;
            IFeature pFea = null;
            DataRow dr = null;
            for (int i= 0; i < pDT.Rows.Count; i++)
            {
                dr = pDT.Rows[i];
                switch (feaSet.FeatureType)
                {
                    case FeatureType.Point:
                        pGeo = CreatePointFromDataRow(dr, ShapeColumnName);
                        break;
                    case FeatureType.MultiPoint:
                        pGeo = CreateMultiPointFromDataRow(dr, ShapeColumnName);
                        //需要补充
                        break;
                    case FeatureType.Line:
                        pGeo = CreateLineStrFromDataRow(dr, ShapeColumnName);
                        break;
                    case FeatureType.Polygon:
                        pGeo = CreatePolygonFromDataRow(dr, ShapeColumnName);
                        break;
                }
                //处理属性
                pFea = feaSet.AddFeature(pGeo);
                
                foreach (string colName in attriNames)
                {
                    pFea.DataRow[colName] = dr[colName]; 
                }
            }
        }


        private IGeometry CreatePointFromDataRow(DataRow dr,string ShapeColumnName)
        {
            byte[] shapeBytes = dr[ShapeColumnName] as byte[];
            byte[] GeoByte = new byte[8];
            //跳过前4个字节
            Array.Copy(shapeBytes, 4, GeoByte, 0, 8);
            Coordinate pCoord = new Coordinate();
            pCoord.X = BitConverter.ToDouble(GeoByte, 0);
            Array.Copy(shapeBytes, 4+8, GeoByte, 0, 8);
            pCoord.Y = BitConverter.ToDouble(GeoByte, 0);
            return new NG.Point(pCoord);
        }

        private IGeometry CreateMultiPointFromDataRow(DataRow dr, string ShapeColumnName)
        {
            byte[] shapeBytes = dr[ShapeColumnName] as byte[];
            //跳过前4个字节；紧接着32个字节(两个点)是包围盒范围；然后4个字节是总点数
            byte[] numPtByte = new byte[4];
            Array.Copy(shapeBytes, 4 + 32, numPtByte, 0, 4);
            int numPt = BitConverter.ToInt32(numPtByte, 0);
            IPoint[] pts = new Point[numPt];
            byte[] GeoByte = new byte[8];
            for (int i = 0;  i < numPt; i++)
            {
                Array.Copy(shapeBytes, 4 + 32 +4 + i*16, GeoByte, 0, 8);
                Coordinate pCoord = new Coordinate();
                pCoord.X = BitConverter.ToDouble(GeoByte, 0);
                Array.Copy(shapeBytes, 4 + 32 + 4 + i * 16+8, GeoByte, 0, 8);
                pCoord.Y = BitConverter.ToDouble(GeoByte, 0);
                pts[i] = new Point(pCoord);
            }
            return new NG.MultiPoint(pts);
        }

        private IGeometry CreateLineStrFromDataRow(DataRow dr, string ShapeColumnName)
        {       
            byte[] shapeBytes = dr[ShapeColumnName] as byte[];
            //紧接着前两个点是几何的包围盒，也就是16*2=32个字节
            //然后是4个字节，值为1；然后是8个字节，代表有多少个点
            byte[] numPartByte = new byte[4];
            Array.Copy(shapeBytes, 4 + 32, numPartByte, 0, 4);
            int numParts = BitConverter.ToInt32(numPartByte, 0);       

            byte[] numPtByte = new byte[4];
            Array.Copy(shapeBytes, 4 + 32 + 4, numPtByte, 0, 4);
            int numPt = BitConverter.ToInt32(numPtByte, 0);

            //获取各个多段线的分割索引号起始位置
            int[] startIdx = new int[numParts];
            byte[] temp = new byte[4];
            for (int k = 0; k < numParts; k++)
            {
                Array.Copy(shapeBytes, 4 + 32 + 4 + 4 + k * 4, temp, 0, 4);
                startIdx[k] = BitConverter.ToInt32(temp, 0);
            }

            //去掉坐标点之前的字节
            byte[] shapeIgnoreHeadByte = new byte[shapeBytes.Length - 4 - 32 - 4 - 4 - 4 * numParts];
            Array.Copy(shapeBytes, 4 + 32 + 4 + 4 + 4 * numParts, shapeIgnoreHeadByte, 0, shapeIgnoreHeadByte.Length);

            ILineString[] lines = new ILineString[numParts];         
            for (int k = 0; k < startIdx.Length; k++)
            {
                int sIdx = startIdx[k];
                int endIdx = k + 1 < startIdx.Length ? startIdx[k + 1] : numPt;

                //4+32:几何类型和包围盒字节
                //紧接着的4+4是多边形个数和总点数；
                //跳过4 * numPolygon个字节
                Coordinate[] pCoords = new Coordinate[endIdx - sIdx];
                int i = 0;
                Coordinate coor;
                byte[] geo = new byte[8];
                for (int j = sIdx * 16; j < endIdx * 16; j += 16)
                {
                    Array.Copy(shapeIgnoreHeadByte, j, geo, 0, geo.Length);
                    coor = new Coordinate();
                    coor.X = BitConverter.ToDouble(geo, 0);
                    Array.Copy(shapeIgnoreHeadByte, j + 8, geo, 0, geo.Length);
                    coor.Y = BitConverter.ToDouble(geo, 0);
                    pCoords[i++] = coor;
                }
                lines[k] = new LineString(pCoords);
            }
            if(lines.Length==1)
            {
                return lines[0];
            }
            else
            {
                return new MultiLineString(lines);
            }
        }

        private IGeometry CreatePolygonFromDataRow(DataRow dr,string ShapeColumnName)
        {
            byte[] shapeBytes = dr[ShapeColumnName] as byte[];
            //紧接着前两个点是几何的包围盒，也就是16*2=32个字节
            //然后是4个字节，代表有几个部分；然后是4个字节，代表有多少个点，紧接着的每四个字节代表每个部分开始点的位置
            byte[] numSkipByte = new byte[4];
            Array.Copy(shapeBytes, 4 + 32, numSkipByte, 0, 4);
            int numParts = BitConverter.ToInt32(numSkipByte, 0);

            byte[] numPtByte = new byte[4];
            Array.Copy(shapeBytes, 4 + 32 + 4, numPtByte, 0, 4);
            int numPt = BitConverter.ToInt32(numPtByte, 0);

            //获取各个多边形的分割索引号起始点序号
            int[] startIdx = new int[numParts];
            byte[] temp = new byte[4];
            for (int k = 0; k < numParts; k++)
            {
                Array.Copy(shapeBytes, 4 + 32 + 4 + 4 + k * 4, temp, 0, 4);
                startIdx[k] = BitConverter.ToInt32(temp, 0);
            }

            //去掉坐标点之前的字节
            byte[] shapeIgnoreHeadByte = new byte[shapeBytes.Length - 4 - 32 - 4 -4 - 4*numParts];
            Array.Copy(shapeBytes, 4 + 32 + 4 + 4 + 4 * numParts, shapeIgnoreHeadByte, 0, shapeIgnoreHeadByte.Length);

            ILinearRing[] linearRings = new ILinearRing[numParts];
            for (int k = 0; k < startIdx.Length; k++)
            {
                int sIdx = startIdx[k];
                //下一个部分起点序号
                int endIdx = k + 1 < startIdx.Length ? startIdx[k + 1] : numPt;

                //4+32:几何类型和包围盒字节
                //紧接着的4+4是多边形个数和总点数；
                //跳过4 * numPolygon个字节
                Coordinate[] pCoords = new Coordinate[endIdx-sIdx];
       
                int i = 0;
                Coordinate coor;
                byte[] geo = new byte[8];
                for (int j = sIdx * 16; j < endIdx*16; j += 16)
                {
                    Array.Copy(shapeIgnoreHeadByte, j, geo, 0, geo.Length);
                    coor = new Coordinate();
                    coor.X = BitConverter.ToDouble(geo, 0);
                    Array.Copy(shapeIgnoreHeadByte, j + 8, geo, 0, geo.Length);
                    coor.Y = BitConverter.ToDouble(geo, 0);
                    pCoords[i++] = coor;
                }
                ILinearRing t_Ring = new LinearRing(pCoords);
                linearRings[k] = t_Ring;
                pCoords = null;
            }

            IGeometry result = null;
            if (linearRings.Length == 1)
            {
                result = new Polygon(linearRings[0]);
            }
            else
            {
                //通过测试初步判定，多边形孔紧跟着外壳存储
                Stack<Polygon> pStack = new Stack<Polygon>();
                pStack.Push(new Polygon(linearRings[0]));
                for (int i=1;i<linearRings.Length;i++)
                {
                    Polygon pGeo = pStack.Pop();
                    if (pGeo.Contains(linearRings[i]))
                    {
                        Polygon poly = pGeo as Polygon;
                        ILinearRing[] holes = new LinearRing[poly.Holes.Length + 1];
                        Array.Copy(poly.Holes, holes, poly.Holes.Length);
                        holes[holes.Length - 1] = linearRings[i];
                        pStack.Push(new Polygon(poly.Shell, holes));
                    }
                    else
                    {
                        pStack.Push(pGeo);//还原
                        pStack.Push(new Polygon(linearRings[i]));
                    }
                }
                if(pStack.Count>1)
                {
                    result = new MultiPolygon(pStack.ToArray());
                }
                else
                {
                    result = pStack.Pop();
                }
            }
            shapeIgnoreHeadByte = null;//释放
            shapeBytes = null;
            //return new Polygon(shell, holes.ToArray());
            return result;
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            string outputDir = tbx_Dir.Text.Trim();
            if (!Directory.Exists(outputDir))
            {
                if (string.IsNullOrWhiteSpace(outputDir))
                {
                    MessageBox.Show("请选择Shapefile文件保存文件夹");
                    return;
                }
                Directory.CreateDirectory(outputDir);
            }


            List<FeatureClass> pOutputFeatureClasses = GetCheckedFeatureClass();

            foreach (FeatureClass feaClass in pOutputFeatureClasses)
            {
                DataTable pDT = feaClass.GetDataTable();
                if (pDT == null)
                {
                    //表格不存在
                    MessageBox.Show(feaClass.Name + "表格不存在。");
                    continue;
                }
                if (pDT.Rows.Count == 0)
                {
                    //MessageCenterMgr.MessageCenter.AddInfoMsg(feaClass.Name + "表格中要素个数为0。");
                    MessageBox.Show(feaClass.Name + "表格中要素个数为0。");
                    continue;
                }

                List<IFeatureSet> feaSets = new List<IFeatureSet>();



                IFeatureSet feaSet = null;
                switch (feaClass.GeometryType)
                {
                    case EsriGeometryType.Point:
                        feaSet = new FeatureSet(FeatureType.Point);
                        break;
                    case EsriGeometryType.MultiPoint:
                        feaSet = new FeatureSet(FeatureType.MultiPoint);
                        break;
                    case EsriGeometryType.Line:
                        feaSet = new FeatureSet(FeatureType.Line);
                        break;
                    case EsriGeometryType.Polygon:
                        feaSet = new FeatureSet(FeatureType.Polygon);
                        break;
                }
                if (feaSet == null)
                {
                    MessageBox.Show(feaClass + "表格中几何类型不被支持，请联系开发者处理。");
                    continue;
                }

                Table2ShapeFile(feaSet, pDT, feaClass.GeomShapeFieldName);
                //设置坐标系并保存
                feaSet.Projection = ProjectionInfo.FromEsriString(feaClass.ProjectionString);
                feaSet.Filename = Path.Combine(tbx_Dir.Text, feaClass.Name + ".shp");
                feaSet.Save();

                MessageBox.Show(feaClass.Name + "表格向Shapefile转换成功。");
            }

            MessageBox.Show("转换完毕");

            pGeoMDBReader.CloseMDB();
            this.Close();
        }
 
        private List<FeatureClass> GetCheckedFeatureClass()
        {
            //获取需要转换的表格
            List<FeatureClass> pOutputFeatureClasses = new List<FeatureClass>();

            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                if (Convert.ToBoolean(dr.Cells["isOutput"].Value))
                {
                    pOutputFeatureClasses.Add((FeatureClass)dr.Tag);
                }
            }
            return pOutputFeatureClasses;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                if (e.RowIndex < 0) return;
                DataGridViewCheckBoxCell dgvCheck = (DataGridViewCheckBoxCell)(this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex]);
                bool isCheck = Convert.ToBoolean(dgvCheck.Value);
                dgvCheck.Value = !isCheck;
            }
        }
    }


}
