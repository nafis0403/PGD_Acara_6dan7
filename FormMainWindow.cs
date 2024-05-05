using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using AxMapWinGIS;
using MapWinGIS;
using System.Threading.Tasks;
using MWLite.Symbology.LegendControl;

namespace SIG_MapWinGIS_Nafis
{
    public partial class FormMainWindow : Form
    {
        public static string strAppPath = "Application.StartupPath";
        public static string strFilePath = Directory.GetParent(Directory.GetParent(strAppPath).ToString()).ToString() + "\\Resources";
        public int handleBatasKec;
        public int handleSaranaPendidikan;
        public FormPopUp FormPopUpObject = null;
        public Addpoint formAddPointObject = null;
        private FormBuffer formBufferObject = null;

        public FormBuffer FormBufferObject { get => formBufferObject; set => formBufferObject = value; }

        public FormMainWindow()
        {
            InitializeComponent();
            strAppPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            strFilePath = Path.Combine(strAppPath, "Resources");
            legend1.Map = (MapWinGIS.Map)axMap1.GetOcx();
            axMap1.SendMouseMove = true;
            axMap1.SendMouseDown = true;
            axMap1.SendMouseUp = true;
            axMap1.MapCursor = tkCursor.crsrMapDefault;
            KryptonRibbonGroupButton_Identify.PerformClick();
            FormPopUpObject = new FormPopUp(this);
            formAddPointObject = new Addpoint(this);
            formBufferObject = new FormBuffer(this);
            legend1.Map = (Map)axMap1.GetOcx();
        }

        private void legend1_Load(object sender, EventArgs e)
        {
            var utils = new Utils();

            //==============================
            //ADD LAYER BATAS ADMIN
            //==============================
            string shpBatasKec = Path.Combine(strFilePath, "Database/Spatial/Kecamatan_jogja.shp");
            MapWinGIS.Shapefile sfBatasKec = new MapWinGIS.Shapefile();
            sfBatasKec.Open(shpBatasKec, null);
            handleBatasKec = legend1.Layers.Add(sfBatasKec, true);
            legend1.GetLayer(handleBatasKec).Name = "Batas Administrasi";

            int fidDesa = sfBatasKec.Table.get_FieldIndexByName("WADMKC");
            sfBatasKec.Categories.Generate(fidDesa, tkClassificationType.ctUniqueValues, 0);
            ColorScheme schemeBatasKec = new ColorScheme();

            //sfBatasDesa.Categories.ApplyColorScheme(tkColorSchemeType.ctSchemeGraduate, schemeBatasDesa);
            schemeBatasKec.SetColors2(tkMapColor.OrangeRed, tkMapColor.LightYellow);
            sfBatasKec.Categories.ApplyColorScheme3(tkColorSchemeType.ctSchemeGraduated,
                schemeBatasKec, tkShapeElements.shElementFill, 0, Convert.ToInt32(sfBatasKec.Categories.Count / 2));

            schemeBatasKec.SetColors2(tkMapColor.ForestGreen, tkMapColor.PowderBlue);
            sfBatasKec.Categories.ApplyColorScheme3(tkColorSchemeType.ctSchemeGraduated,
                schemeBatasKec, tkShapeElements.shElementFill, (Convert.ToInt32(sfBatasKec.Categories.Count / 2) + 1),
                (sfBatasKec.Categories.Count - 1));
            axMap1.Redraw();





            //==============================
            //ADD LAYER JARINGAN JALAN
            //==============================
            string shpJalan = Path.Combine(strFilePath, "Database/Spatial/Jalan_Kota_Jogja.shp");
            MapWinGIS.Shapefile sfJalan = new MapWinGIS.Shapefile();
            sfJalan.Open(shpJalan, null);
            int handleJalan = legend1.Layers.Add(sfJalan, true);
            legend1.GetLayer(handleJalan).Name = "Jaringan Jalan";

            LinePattern patternKolektor = new LinePattern();
            patternKolektor.AddLine(utils.ColorByName(tkMapColor.Red), 4.0f, tkDashStyle.dsDashDot);
            ShapefileCategory ctKolektor = sfJalan.Categories.Add("Jalan Kolektor");
            ctKolektor.Expression = "[REMARK] = \"Jalan Kolektor\"";
            ctKolektor.DrawingOptions.LinePattern = patternKolektor;
            ctKolektor.DrawingOptions.UseLinePattern = true;

            LinePattern patternLokal = new LinePattern();
            patternLokal.AddLine(utils.ColorByName(tkMapColor.Red), 3.0f, tkDashStyle.dsDashDot);
            ShapefileCategory ctLokal = sfJalan.Categories.Add("Jalan Lokal");
            ctLokal.Expression = "[REMARK] = \"Jalan Lokal\"";
            ctLokal.DrawingOptions.LinePattern = patternLokal;
            ctLokal.DrawingOptions.UseLinePattern = true;

            LinePattern patternLain = new LinePattern();
            patternLain.AddLine(utils.ColorByName(tkMapColor.Red), 2.0f, tkDashStyle.dsDashDot);
            ShapefileCategory ctLain = sfJalan.Categories.Add("Jalan Lain");
            ctLain.Expression = "[REMARK] = \"Jalan Lain\"";
            ctLain.DrawingOptions.LinePattern = patternLain;
            ctLain.DrawingOptions.UseLinePattern = true;

            sfJalan.DefaultDrawingOptions.Visible = false; // hide all the unclassified points
            sfJalan.Categories.ApplyExpressions();
            axMap1.Redraw();


            //==============================
            //ADD LAYER Sarana Pendidikan
            //==============================
            loadSaranaPendidikan();
            {
                string shpSaranaPendidikan = Path.Combine(strFilePath, "Database/Spatial/Sekolah_Jogja.shp");
                MapWinGIS.Shapefile sfSaranaPendidikan = new MapWinGIS.Shapefile();
                sfSaranaPendidikan.Open(shpSaranaPendidikan, null);
                {
                    MapWinGIS.Image imgSD = new MapWinGIS.Image();
                    imgSD.Open(Path.Combine(strFilePath, ""),
                        ImageType.USE_FILE_EXTENSION, true, null);
                    ShapefileCategory ctSD = sfSaranaPendidikan.Categories.Add("SD");
                    ctSD.Expression = "[REMARK] = \"SD\"";
                    ctSD.DrawingOptions.PointType = tkPointSymbolType.ptSymbolPicture;
                    ctSD.DrawingOptions.Picture = imgSD;
                    ctSD.DrawingOptions.PictureScaleX = 0.25;
                    ctSD.DrawingOptions.PictureScaleY = 0.25;

                    MapWinGIS.Image imgSMP = new MapWinGIS.Image();
                    imgSMP.Open(Path.Combine(strFilePath, ""),
                        ImageType.USE_FILE_EXTENSION, true, null);
                    ShapefileCategory ctSMP = sfSaranaPendidikan.Categories.Add("SMP");
                    ctSMP.Expression = "[REMARK] = \"SMP\"";
                    ctSMP.DrawingOptions.PointType = tkPointSymbolType.ptSymbolPicture;
                    ctSMP.DrawingOptions.Picture = imgSMP;
                    ctSMP.DrawingOptions.PictureScaleX = 0.25;
                    ctSMP.DrawingOptions.PictureScaleY = 0.25;

                    MapWinGIS.Image imgSMA = new MapWinGIS.Image();
                    imgSMA.Open(Path.Combine(strFilePath, ""),
                        ImageType.USE_FILE_EXTENSION, true, null);
                    ShapefileCategory ctSMA = sfSaranaPendidikan.Categories.Add("SMA");
                    ctSMA.Expression = "[REMARK] = \"SMA \"";
                    ctSMA.DrawingOptions.PointType = tkPointSymbolType.ptSymbolPicture;
                    ctSMA.DrawingOptions.Picture = imgSMA;
                    ctSMA.DrawingOptions.PictureScaleX = 0.25;
                    ctSMA.DrawingOptions.PictureScaleY = 0.25;

                    sfSaranaPendidikan.DefaultDrawingOptions.Visible = false; // hide all the unclasified points
                    sfSaranaPendidikan.Categories.ApplyExpressions();
                    axMap1.Redraw();
                }

            }
        }

        public void LoadSaranaPendidikan()
        {
            throw new NotImplementedException();
        }

        //==============================
        //BASEMAP
        //==============================
        private void Basemap_None_CheckedChanged_1(object sender, EventArgs e)
        {
            axMap1.TileProvider = MapWinGIS.tkTileProvider.ProviderNone;
            axMap1.Redraw();
            axMap1.Refresh();
        }

        private void Basemap_OpenStreetMap_CheckedChanged(object sender, EventArgs e)
        {
            axMap1.TileProvider = MapWinGIS.tkTileProvider.OpenStreetMap;
            axMap1.Redraw();
            axMap1.Refresh();
        }

        private void Basemap_OpenCycleMap_CheckedChanged(object sender, EventArgs e)
        {
            axMap1.TileProvider = MapWinGIS.tkTileProvider.OpenCycleMap;
            axMap1.Redraw();
            axMap1.Refresh();
        }

        private void Basemap_OpenTransportMap_CheckedChanged(object sender, EventArgs e)
        {
            axMap1.TileProvider = MapWinGIS.tkTileProvider.OpenTransportMap;
            axMap1.Redraw();
            axMap1.Refresh();
        }

        private void Basemap_BingMaps_CheckedChanged(object sender, EventArgs e)
        {
            axMap1.TileProvider = MapWinGIS.tkTileProvider.BingMaps;
            axMap1.Redraw();
            axMap1.Refresh();
        }

        private void Basemap_BingSatellite_CheckedChanged(object sender, EventArgs e)
        {
            axMap1.TileProvider = MapWinGIS.tkTileProvider.BingSatellite;
            axMap1.Redraw();
            axMap1.Refresh();
        }

        private void Basemap_BingHybrid_CheckedChanged(object sender, EventArgs e)
        {
            axMap1.TileProvider = MapWinGIS.tkTileProvider.BingHybrid;
            axMap1.Redraw();
            axMap1.Refresh();
        }

        private void Basemap_GoogleMaps_CheckedChanged(object sender, EventArgs e)
        {
            axMap1.TileProvider = MapWinGIS.tkTileProvider.GoogleMaps;
            axMap1.Redraw();
            axMap1.Refresh();
        }

        private void Basemap_GoogleSatellite_CheckedChanged(object sender, EventArgs e)
        {
            axMap1.TileProvider = MapWinGIS.tkTileProvider.GoogleSatellite;
            axMap1.Redraw();
            axMap1.Refresh();
        }

        private void Basemap_GoogleHybrid_CheckedChanged(object sender, EventArgs e)
        {
            axMap1.TileProvider = MapWinGIS.tkTileProvider.GoogleHybrid;
            axMap1.Redraw();
            axMap1.Refresh();
        }

        private void Basemap_GoogleTerrain_CheckedChanged(object sender, EventArgs e)
        {
            axMap1.TileProvider = MapWinGIS.tkTileProvider.GoogleTerrain;
            axMap1.Redraw();
            axMap1.Refresh();
        }

        private void Basemap_HereMaps_CheckedChanged(object sender, EventArgs e)
        {
            axMap1.TileProvider = MapWinGIS.tkTileProvider.HereMaps;
            axMap1.Redraw();
            axMap1.Refresh();
        }

        private void Basemap_HereSatellite_CheckedChanged(object sender, EventArgs e)
        {
            axMap1.TileProvider = MapWinGIS.tkTileProvider.HereSatellite;
            axMap1.Redraw();
            axMap1.Refresh();
        }

        private void Basemap_HereHybrid_CheckedChanged(object sender, EventArgs e)
        {
            axMap1.TileProvider = MapWinGIS.tkTileProvider.HereHybrid;
            axMap1.Redraw();
            axMap1.Refresh();
        }

        private void Basemap_HereTerrain_CheckedChanged(object sender, EventArgs e)
        {
            axMap1.TileProvider = MapWinGIS.tkTileProvider.HereTerrain;
            axMap1.Redraw();
            axMap1.Refresh();
        }

        private void Basemap_Rosreestr_CheckedChanged(object sender, EventArgs e)
        {
            axMap1.TileProvider = MapWinGIS.tkTileProvider.Rosreestr;
            axMap1.Redraw();
            axMap1.Refresh();
        }

        private void Basemap_MapQuestAerial_CheckedChanged(object sender, EventArgs e)
        {
            axMap1.TileProvider = MapWinGIS.tkTileProvider.MapQuestAerial;
            axMap1.Redraw();
            axMap1.Refresh();
        }

        //==============================
        //CURSOR MODE, IDENTIFY, MEASURE
        //==============================
        private void KryptonRibbonGroup_NormalMode_Click(object sender, EventArgs e)
        {
            axMap1.MapCursor = tkCursor.crsrMapDefault;
            if (KryptonRibbonGroupButton_NormalMode.Checked == true)
            {
                axMap1.CursorMode = MapWinGIS.tkCursorMode.cmNone;
                KryptonRibbonGroupButton_ZoomInMode.Checked = false;
                KryptonRibbonGroupButton_ZoomOutMode.Checked = false;
                KryptonRibbonGroupButton_PanMode.Checked = false;
                KryptonRibbonGroupButton_Identify.Checked = false;
                KryptonRibbonGroupButton_MeasureLength.Checked = false;
                KryptonRibbonGroupButton_MeasureArea.Checked = false;
            }
            else
            {
                KryptonRibbonGroupButton_NormalMode.Checked = true;
                axMap1.CursorMode = MapWinGIS.tkCursorMode.cmNone;
            }
        }

        private void KryptonRibbonGroupButton_ZoomInMode_Click_1(object sender, EventArgs e)
        {
            axMap1.MapCursor = tkCursor.crsrMapDefault;
            if (KryptonRibbonGroupButton_ZoomInMode.Checked == true)
            {
                axMap1.CursorMode = MapWinGIS.tkCursorMode.cmZoomIn;
                KryptonRibbonGroupButton_NormalMode.Checked = false;
                KryptonRibbonGroupButton_ZoomOutMode.Checked = false;
                KryptonRibbonGroupButton_PanMode.Checked = false;
                KryptonRibbonGroupButton_Identify.Checked = false;
                KryptonRibbonGroupButton_MeasureLength.Checked = false;
                KryptonRibbonGroupButton_MeasureArea.Checked = false;
            }
            else
            {
                KryptonRibbonGroupButton_NormalMode.Checked = true;
                axMap1.CursorMode = MapWinGIS.tkCursorMode.cmNone;
            }
        }

        private void KryptonRibbonGroupButton_ZoomOutMode_Click_1(object sender, EventArgs e)
        {
            axMap1.MapCursor = tkCursor.crsrMapDefault;
            if (KryptonRibbonGroupButton_ZoomOutMode.Checked == true)
            {
                axMap1.CursorMode = MapWinGIS.tkCursorMode.cmZoomOut;
                KryptonRibbonGroupButton_NormalMode.Checked = false;
                KryptonRibbonGroupButton_ZoomInMode.Checked = false;
                KryptonRibbonGroupButton_PanMode.Checked = false;
                KryptonRibbonGroupButton_Identify.Checked = false;
                KryptonRibbonGroupButton_MeasureLength.Checked = false;
                KryptonRibbonGroupButton_MeasureArea.Checked = false;
            }
            else
            {
                KryptonRibbonGroupButton_NormalMode.Checked = true;
                axMap1.CursorMode = MapWinGIS.tkCursorMode.cmNone;
            }
        }

        private void KryptonRibbonGroupButton_PanMode_Click_1(object sender, EventArgs e)
        {
            axMap1.MapCursor = tkCursor.crsrMapDefault;
            if (KryptonRibbonGroupButton_PanMode.Checked == true)
            {
                axMap1.CursorMode = MapWinGIS.tkCursorMode.cmPan;
                KryptonRibbonGroupButton_NormalMode.Checked = false;
                KryptonRibbonGroupButton_ZoomInMode.Checked = false;
                KryptonRibbonGroupButton_ZoomOutMode.Checked = false;
                KryptonRibbonGroupButton_Identify.Checked = false;
                KryptonRibbonGroupButton_MeasureLength.Checked = false;
                KryptonRibbonGroupButton_MeasureArea.Checked = false;
            }
            else
            {
                KryptonRibbonGroupButton_NormalMode.Checked = true;
                axMap1.CursorMode = MapWinGIS.tkCursorMode.cmNone;
            }
        }

        private void KryptonRibbonGroupButton_Identify_Click_1(object sender, EventArgs e)
        {
            axMap1.MapCursor = tkCursor.crsrMapDefault;
            if (KryptonRibbonGroupButton_Identify.Checked == true)
            {
                axMap1.MapCursor = tkCursor.crsrUpArrow;
                axMap1.CursorMode = MapWinGIS.tkCursorMode.cmNone;
                KryptonRibbonGroupButton_NormalMode.Checked = false;
                KryptonRibbonGroupButton_ZoomInMode.Checked = false;
                KryptonRibbonGroupButton_ZoomOutMode.Checked = false;
                KryptonRibbonGroupButton_PanMode.Checked = false;
                KryptonRibbonGroupButton_MeasureLength.Checked = false;
                KryptonRibbonGroupButton_MeasureArea.Checked = false;
            }
            else
            {
                KryptonRibbonGroupButton_NormalMode.Checked = true;
                axMap1.CursorMode = MapWinGIS.tkCursorMode.cmNone;
            }
        }

        private void KryptonRibbonGroupButton_Length_Click_1(object sender, EventArgs e)
        {
            axMap1.MapCursor = tkCursor.crsrMapDefault;
            if (KryptonRibbonGroupButton_MeasureLength.Checked == true)
            {
                axMap1.CursorMode = MapWinGIS.tkCursorMode.cmMeasure;
                axMap1.Measuring.MeasuringType = tkMeasuringType.MeasureDistance;
                KryptonRibbonGroupButton_NormalMode.Checked = false;
                KryptonRibbonGroupButton_ZoomInMode.Checked = false;
                KryptonRibbonGroupButton_ZoomOutMode.Checked = false;
                KryptonRibbonGroupButton_PanMode.Checked = false;
                KryptonRibbonGroupButton_Identify.Checked = false;
                KryptonRibbonGroupButton_MeasureArea.Checked = false;
            }
            else
            {
                KryptonRibbonGroupButton_NormalMode.Checked = true;
                axMap1.CursorMode = MapWinGIS.tkCursorMode.cmNone;
            }
        }

        private void KryptonRibbonGroupButton_Weight_Click(object sender, EventArgs e)
        {
            axMap1.MapCursor = tkCursor.crsrMapDefault;
            if (KryptonRibbonGroupButton_MeasureArea.Checked == true)
            {
                axMap1.CursorMode = MapWinGIS.tkCursorMode.cmMeasure;
                axMap1.Measuring.MeasuringType = tkMeasuringType.MeasureArea;
                KryptonRibbonGroupButton_NormalMode.Checked = false;
                KryptonRibbonGroupButton_ZoomInMode.Checked = false;
                KryptonRibbonGroupButton_ZoomOutMode.Checked = false;
                KryptonRibbonGroupButton_PanMode.Checked = false;
                KryptonRibbonGroupButton_Identify.Checked = false;
                KryptonRibbonGroupButton_MeasureLength.Checked = false;
            }
            else
            {
                KryptonRibbonGroupButton_NormalMode.Checked = true;
                axMap1.CursorMode = MapWinGIS.tkCursorMode.cmNone;
            }
        }


        //==============================
        //CURSOR MODE, IDENTIFY, MEASURE
        //==============================
        private void KryptonRibbonGroupButton_ZoomIn_Click_1(object sender, EventArgs e)
        {
            axMap1.ZoomIn(0.2);
        }

        private void KryptonRibbonGroupButton_ZoomOut_Click_1(object sender, EventArgs e)
        {
            axMap1.ZoomOut(0.2);
        }

        private void KryptonRibbonGroupButton_FullExtent_Click_1(object sender, EventArgs e)
        {
            axMap1.ZoomToMaxExtents();
        }

        private void KryptonRibbonGroupButton_ZoomToPrev_Click(object sender, EventArgs e)
        {
            axMap1.ZoomToPrev();
        }

        //==============================
        //DATA
        //==============================
        private void KryptonRibbonGroupButton_AddData_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            string strfileshp = "Shapefile belum dimasukkan";

            ofd.Title = "Browse Shapefile";
            ofd.InitialDirectory = @"D:\";
            ofd.Filter = "Shapefile (*.shp)|*.shp|All files (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;

            if ((ofd.ShowDialog() == DialogResult.OK))
            {
                strFilePath = ofd.FileName;

                MapWinGIS.Shapefile sfAsetPendidikan = new MapWinGIS.Shapefile();
                sfAsetPendidikan.Open(strfileshp, null);
                int handleBufferResult = legend1.Layers.Add(sfAsetPendidikan, true);
                legend1.GetLayer(handleBufferResult).Name = System.IO.Path.GetFileName(strFilePath);
                sfAsetPendidikan.Identifiable = true;

                if (!formBufferObject.cboInput.Items.Contains(strfileshp))
                {
                    formBufferObject.cboInput.Items.Add(strfileshp);
                }
                formBufferObject.cboInput.Text = strfileshp;
            }
            else
            {
                MessageBox.Show(strfileshp, "Report",
                    MessageBoxButtons.OKCancel);
            }
        }

        private void KryptonRibbonGroupButton_Remove_Click(object sender, EventArgs e)
        {
            legend1.Layers.Remove(legend1.SelectedLayer);
        }



        //==============================
        //DATAGRIDVIEW EVENT
        //==============================

        private void DataGridView1_RowHeaderMouseDoubleClick(object sender, EventArgs e)
        {
            if (DataGridView1.SelectedRows.Count > 0)
            {
                Shapefile sf = axMap1.get_Shapefile(handleSaranaPendidikan);
                sf.SelectNone();

                for (int i = 0; i < DataGridView1.SelectedRows.Count; i++)
                {
                    sf.set_ShapeSelected(Convert.ToInt32(DataGridView1.SelectedRows[i].Cells["fid"].Value), true);
                }
                axMap1.ZoomToSelected(handleSaranaPendidikan);
            }
        }

        private void DataGridView1_SelectionChanged_1(object sender, EventArgs e)
        {
            if (DataGridView1.SelectedRows.Count > 0)
            {
                Shapefile sf = axMap1.get_Shapefile(handleSaranaPendidikan);
                sf.SelectNone();

                for (int i = 0; i < DataGridView1.SelectedRows.Count; i++)
                {
                    sf.set_ShapeSelected(Convert.ToInt32(DataGridView1.SelectedRows[i].Cells["fid"].Value), true);
                }
                axMap1.ZoomToSelected(handleSaranaPendidikan);
            }
        }

        //==============================
        //QUERY
        //==============================
        private void KryptonRibbonGroupComboBoxQueryKecamatan_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(KryptonRibbonGroupComboBoxQueryKecamatan.Text == "")
                && !(KryptonRibbonGroupComboBoxQueryPendidikan.Text == ""))
            {
                //Shapefile sfBatasDesa.....

                Shapefile sfSaranaPendidikan = axMap1.get_Shapefile(handleSaranaPendidikan);
                sfSaranaPendidikan.SelectNone();

                string errorSaranaPendidikan = "";
                object resultSaranaPendidikan = null;
                string querySaranaPendidikan = "[REMARK] = \"" + KryptonRibbonGroupComboBoxQueryKecamatan.Text
                    + "\" AND [Nama_KP] = \"" + KryptonRibbonGroupComboBoxQueryPendidikan.Text + "\"";

                if (sfSaranaPendidikan.Table.Query(querySaranaPendidikan, ref resultSaranaPendidikan, ref errorSaranaPendidikan))
                {
                    int[] shapesSaranaPendidikan = resultSaranaPendidikan as int[];
                    if (shapesSaranaPendidikan != null)
                    {
                        for (int i = 0; i < shapesSaranaPendidikan.Length; i++)
                        {
                            sfSaranaPendidikan.set_ShapeSelected(shapesSaranaPendidikan[i], true);
                        }
                        axMap1.ZoomToSelected(handleSaranaPendidikan);
                        axMap1.ZoomIn(0.2);
                        axMap1.ZoomIn(0.2);
                        axMap1.ZoomIn(0.2);
                        axMap1.ZoomIn(0.2);
                        axMap1.ZoomIn(0.2);
                    }
                }
            }
        }
        private void KryptonRibbonGroupComboBoxQueryPendidikan_SelectedIndexChanged(object sender, EventArgs e)
        {
        
        }
        private void KryptonRibbonGroupComboBoxQueryCariNamaSekolah_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(KryptonRibbonGroupComboBoxQueryKecamatan.Text == "Cari Kecamatan..."))
            {
                KryptonRibbonGroupComboBoxQueryCariNamaSekolah.Items.Clear();
                KryptonRibbonGroupComboBoxQueryCariNamaSekolah.Text = "Cari Sarana Pendidikan...";

                ///tanpa batas desa 

                Shapefile sfSaranaPendidikan = axMap1.get_Shapefile(handleSaranaPendidikan);
                sfSaranaPendidikan.SelectNone();

                string errorSaranaPendidikan = "";
                object resultSaranaPendidikan = null;
                string querySaranaPendidikan = "[REMARK] = \"" + KryptonRibbonGroupComboBoxQueryKecamatan.Text + "\"";

                if (sfSaranaPendidikan.Table.Query(querySaranaPendidikan, ref resultSaranaPendidikan, ref errorSaranaPendidikan))
                {
                    int[] shapesSaranaPendidikan = resultSaranaPendidikan as int[];
                    if (shapesSaranaPendidikan != null)
                    {
                    if (!(shapesSaranaPendidikan.Length == 0))
                        {
                            MessageBox.Show("Pada Kecamatan " + KryptonRibbonGroupComboBoxQueryKecamatan.Text
                                + "ditemukan " + shapesSaranaPendidikan.Length.ToString()
                                + "Sarana Pendidikan Pemerintah Kota Jogja..", "Informasi Sarana Pendidikan", MessageBoxButtons.OK);
                        }
                        else
                        {
                            MessageBox.Show("Pada Kecamatan " + KryptonRibbonGroupComboBoxQueryKecamatan.Text
                                + " tidak ditemukan Sarana Pendidikan  Pemerintah Kota Jogja..",
                                "Informasi Sarana Pendidikan", MessageBoxButtons.OK);
                        }
                    }
                    else
                    {
                    MessageBox.Show("Pada Kecamatan " + KryptonRibbonGroupComboBoxQueryKecamatan.Text
                          + " tidak ditemukan Sarana Pendidikan pemerintah Kota Jogja..",
                          "Informasi Sarana Pendidikan", MessageBoxButtons.OK);
                    }
                }
            }

        }

        //==============================
        //EDIT
        //==============================
        private void KryptonRibbonGroupButton_AddPoint_Click_1(object sender, EventArgs e)
        {
            formAddPointObject.Show();
            formAddPointObject.BringToFront();
        }

        //==============================
        //ANALYST
        //==============================
        private void KryptonRibbonGroupButton_Buffer_Click(object sender, EventArgs e)
        {
            formBufferObject.Show();
            formBufferObject.BringToFront();
        }

        //==============================
        //MAP EVENT
        //==============================
        private void AxMap1_MouseEvent(object sender, _DMapEvents_MouseUpEvent e)
        {
            double projX = 0.0;
            double projY = 0.0;
            axMap1.PixelToProj(e.x, e.y, ref projX, ref projY);
            object result = null;
            Extents ext = new Extents();
            ext.SetBounds(projX, projY, 0.0, projX, projY, 0.0);
            double tolerance = 100; //meters
            Utils utils = new Utils();
            utils.ConvertDistance(tkUnitsOfMeasure.umMeters, tkUnitsOfMeasure.umDecimalDegrees, ref tolerance);

            if (KryptonRibbonGroupButton_Identify.Checked == true)
            {
                Shapefile sf = axMap1.get_Shapefile(handleSaranaPendidikan);
                sf.SelectNone();
                axMap1.Redraw2(tkRedrawType.RedrawAll);
                axMap1.Refresh();

                FormPopUpObject.Hide();
                if (sf is null)
                {
                    if (sf.SelectShapes(ext, tolerance, SelectMode.INTERSECTION, ref result))
                    {
                        int[] shapes = result as int[];
                        if (shapes.Length > 0)
                        {
                            sf.SelectNone();
                            sf.set_ShapeSelected(shapes[0], true);
                            axMap1.Redraw2(tkRedrawType.RedrawAll);
                            axMap1.Refresh();

                            FormPopUpObject.txtShapeIndex.Text = shapes[0].ToString();


                            FormPopUpObject.txtNamaSekolah.Text = sf.get_CellValue(
                                sf.Table.get_FieldIndexByName("nama sekolah"), shapes[0]).ToString();
                            FormPopUpObject.cboJenisPendidikan.Text = sf.get_CellValue(
                                sf.Table.get_FieldIndexByName("Jenis Pendidikan"), shapes[0]).ToString();
                            FormPopUpObject.pictureBox1.Text = sf.get_CellValue(
                                sf.Table.get_FieldIndexByName(""), shapes[0]).ToString();

                            FormPopUpObject.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                            FormPopUpObject.pictureBox1.ImageLocation = Path.Combine(strFilePath,
                                "" + sf.get_CellValue(
                                 sf.Table.get_FieldIndexByName("foto"), shapes[0]).ToString());

                            FormPopUpObject.Show();
                            FormPopUpObject.BringToFront();

                        }

                    }

                }

            }
            else if (axMap1.MapCursor == tkCursor.crsrCross)
            {
                formAddPointObject.txtTitikX = Convert.ToString(projX);
                formAddPointObject.txtTitikY = Convert.ToString(projY);
            }

        }

        //==============================
        //FUNCTION
        //==============================
        public void loadSaranaPendidikan()
        {
            //==============================
            //ADD LAYER SARANA Pendidikan
            //==============================
            string shpSaranaPendidikan = Path.Combine(strFilePath, "Database/Spatial/Sekolah_Jogja.shp");
            MapWinGIS.Shapefile sfSaranaPendidikan = new MapWinGIS.Shapefile();
            sfSaranaPendidikan.Open(shpSaranaPendidikan, null);

            sfSaranaPendidikan.DefaultDrawingOptions.Visible = false; //hide all the unclassified points
            sfSaranaPendidikan.Categories.ApplyExpressions();
            axMap1.Redraw();

            sfSaranaPendidikan.DefaultDrawingOptions.Visible = false; //hide all the unclassified points
            sfSaranaPendidikan.Categories.ApplyExpressions();
            axMap1.Redraw();

            sfSaranaPendidikan.DefaultDrawingOptions.Visible = false; //hide all the unclassified points
            sfSaranaPendidikan.Categories.ApplyExpressions();
            axMap1.Redraw();

            //==============================
            //LOAD ATTRIBUTE
            //==============================



            DataGridView1.Rows.Clear();
            KryptonRibbonGroupComboBoxQueryKecamatan.Items.Clear();

            for (int i = 0; i < sfSaranaPendidikan.Table.NumFields; i++)
            {
                DataGridView1.Columns.Add(sfSaranaPendidikan.Table.Field[i].Name, sfSaranaPendidikan.Table.Field[i].Name);
            }
            DataGridView1.Columns.Add("fid", "fid");

            for (int i = 0; i < sfSaranaPendidikan.Table.NumRows; i++)
            {
                string[] myAttributeRow = new string[sfSaranaPendidikan.Table.NumFields + 1];
                for (int j = 0; j < sfSaranaPendidikan.Table.NumFields; j++)
                {
                    myAttributeRow[j] = sfSaranaPendidikan.Table.CellValue[j, i].ToString();
                }
                myAttributeRow[sfSaranaPendidikan.Table.NumFields] = i.ToString();
                DataGridView1.Rows.Insert(i, myAttributeRow);

                if (!KryptonRibbonGroupComboBoxQueryKecamatan.Items.Contains(
                    sfSaranaPendidikan.Table.CellValue[sfSaranaPendidikan.FieldIndexByName["REMARK"], i].ToString()))
                {
                    KryptonRibbonGroupComboBoxQueryKecamatan.Items.Add(
                        sfSaranaPendidikan.Table.CellValue[sfSaranaPendidikan.FieldIndexByName["REMARK"], i].ToString());
                }
            }

            DataGridView1.ClearSelection();
            KryptonRibbonGroupComboBoxQueryKecamatan.Sorted = true;
        }
        private void axMap1_MouseDownEvent(object sender, _DMapEvents_MouseDownEvent e)
        {

        }

        private void TabPage_Basemap_Click(object sender, EventArgs e)
        {

        }

        private void Basemap_None_Click(object sender, EventArgs e)
        {
            axMap1.TileProvider = MapWinGIS.tkTileProvider.ProviderNone;
            axMap1.Redraw();
            axMap1.Refresh();
        }

        private void Basemap_OpenCycleMap_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void Basemap_OpenTransportMap_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void Basemap_GoogleMaps_CheckedChanged_1(object sender, EventArgs e)
        {
            axMap1.TileProvider = MapWinGIS.tkTileProvider.GoogleMaps;
            axMap1.Redraw();
            axMap1.Refresh();
        }
    }
}
