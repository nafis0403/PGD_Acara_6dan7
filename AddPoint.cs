using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MapWinGIS;
using AxMapWinGIS;

namespace SIG_MapWinGIS_Nafis
{
    public partial class AddPoint : Form
    {
        FormMainWindow FormMainWindowObject;
        public AddPoint(FormMainWindow FormMainWindowInitialized)
        {
            InitializeComponent();
            FormMainWindowObject = FormMainWindowInitialized;
        }

        private void AddPoint_Load(object sender, EventArgs e)
        {

        }
        private void Addpoint_Load(object sender, EventArgs e)
        {
            rdoTitik_Keyboard.Checked = true;
            txtTitikX.Text = "";
            txtTitikY.Text = "";
            cboJenisPendidikan.Text = "";
            txtNamaSekolah.Text = "";
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Image = null;
        }

        private void rdoTitik_Keyboard_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoTitik_Cursor.Checked == true)
            {
                FormMainWindowObject.KryptonRibbonGroupButton_NormalMode.PerformClick();
                FormMainWindowObject.axMap1.MapCursor = tkCursor.crsrCross;
                txtTitikX.Enabled = false;
                txtTitikY.Enabled = false;
            }
            else
            {
                FormMainWindowObject.axMap1.MapCursor = tkCursor.crsrMapDefault;
                FormMainWindowObject.KryptonRibbonGroupButton_NormalMode.PerformClick();
                txtTitikX.Enabled = true;
                txtTitikY.Enabled = true;
            }
        }

        private void axMap1_MouseDownEvent(object sender, _DMapEvents_MouseDownEvent e)
        {

        }

        private void cmdBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Browse Photo";
            ofd.InitialDirectory = @"C:\";
            ofd.Filter = "JPG (*.jpg)|*.jpg|JPEG (*.jpeg)|*.jpeg|PNG (*.png)|*.png|All files (*.*)|*.*";
            ofd.FilterIndex = 1;

            if ((ofd.ShowDialog() == DialogResult.OK))
            {
                string fileName = @Path.GetFileName(ofd.FileName);
                string sourcePath = @Path.GetDirectoryName(ofd.FileName);
                string targetPath = @Path.Combine(FormMainWindow.strFilePath, "Database/Non-Spatial/Foto");
                string sourceFile = @Path.Combine(targetPath, fileName);
                string destFile = @Path.Combine(targetPath, fileName);
                File.Copy(sourceFile, destFile, true);
                pictureBox1.Text = fileName;
                pictureBox1.ImageLocation = destFile;
            }
            else
            {
                MessageBox.Show("Foto Belum dipilih !!", "Report", MessageBoxButtons.OK);
            }
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            Shapefile sf = FormMainWindowObject.axMap1.get_Shapefile(FormMainWindowObject.handleSaranaPendidikan);
            //bool result = sf.CreateNewWithShapeID("", ShpfileType.SHP_POINT);

            var myPoint = new MapWinGIS.Point();
            myPoint.x = Convert.ToDouble(txtTitikX.Text);
            myPoint.y = Convert.ToDouble(txtTitikY.Text);

            //MessageBox.Show(sf.ShapeFileType.ToString());
            Shape myShape = new Shape();
            myShape.Create(ShpfileType.SHP_POINT);
            int myPointIndex = 0;
            myShape.InsertPoint(myPoint, ref myPointIndex);
            int myShapeIndex = 0;

            sf.StartEditingShapes();
            if (sf.EditInsertShape(myShape, ref myShapeIndex))
            {
                sf.Save();
                sf.StartEditingTable();
                sf.EditCellValue(sf.Table.get_FieldIndexByName("Fasilitas Ibadah"),
                    myShapeIndex, cboJenisPendidikan.Text);
                sf.EditCellValue(sf.Table.get_FieldIndexByName("Agama"),
                    myShapeIndex, txtNamaSekolah.Text);
                sf.EditCellValue(sf.Table.get_FieldIndexByName("Foto"),
                 myShapeIndex, pictureBox1.Text);
                sf.Save();
                sf.StartEditingTable();
            }
            else
            {
                MessageBox.Show(sf.ErrorMsg[sf.LastErrorCode].ToString());
            }
            sf.Save();
            sf.StopEditingShapes();

            FormMainWindowObject.axMap1.RemoveLayer(FormMainWindowObject.handleSaranaPendidikan);
            FormMainWindowObject.axMap1.Redraw();
            FormMainWindowObject.axMap1.Redraw2(tkRedrawType.RedrawAll);

            FormMainWindowObject.axMap1.Refresh();
            FormMainWindowObject.legend1.Layers.Remove(FormMainWindowObject.handleSaranaPendidikan);
            FormMainWindowObject.legend1.RedrawLegendAndMap();
            FormMainWindowObject.legend1.Refresh();
            FormMainWindowObject.Refresh();

            FormMainWindowObject.loadSaranaPendidikan();
            FormMainWindowObject.axMap1.Redraw();
            FormMainWindowObject.axMap1.Redraw2(tkRedrawType.RedrawAll);

            FormMainWindowObject.axMap1.Refresh();
            FormMainWindowObject.legend1.RedrawLegendAndMap();
            FormMainWindowObject.legend1.Refresh();
            FormMainWindowObject.Refresh();

            this.Hide();
            MessageBox.Show("Data Berhasil Disimpan");
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Addpoint_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            this.Hide();
            rdoTitik_Keyboard.Checked = true;
            txtTitikX.Text = "";
            txtTitikY.Text = "";
            cboJenisPendidikan.Text = "";
            txtNamaSekolah.Text = "";
            pictureBox1.Text = "";
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Image = null;
            FormMainWindowObject.axMap1.MapCursor = tkCursor.crsrMapDefault;
            FormMainWindowObject.KryptonRibbonGroupButton_NormalMode.PerformClick();
        }
    }
}
