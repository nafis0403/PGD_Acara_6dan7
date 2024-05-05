using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AxMapWinGIS;
using MapWinGIS;

namespace SIG_MapWinGIS_Nafis
{
    public partial class FormPopUp : Form
    {
        FormMainWindow FormMainWindowObject;

        public FormMainWindow FormMainWindowInitialized { get; }

        public FormPopUp(FormMainWindow formMainWindow)
        {
            InitializeComponent();
            FormMainWindowObject = FormMainWindowInitialized;
        }

        private void cmdEdit_Click(object sender, EventArgs e)
        {
            if (cmdEdit.Text == "Edit")
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox(
                    " Please enter your password...", "Password", "", -1, -1);
                if (input == "123")
                {
                    cboJenisPendidikan.Enabled = true;
                    txtNamaSekolah.ReadOnly = false;
                    cmdBrowse.Enabled = true;
                    cmdDelete.Visible = true;
                    cmdEdit.Text = "Save";
                }
                else
                {
                    cboJenisPendidikan.Enabled = false;
                    txtNamaSekolah.ReadOnly = true;
                    cmdBrowse.Enabled = false;
                    cmdDelete.Visible = false;
                    cmdEdit.Text = "Edit";
                    MessageBox.Show("Password salah!");
                }
            }

            else if (cmdEdit.Text == "Save")
            {
                Shapefile sf = FormMainWindowObject.axMap1.get_Shapefile(FormMainWindowObject.handleSaranaPendidikan);

                sf.StartEditingTable();
                sf.EditCellValue(sf.Table.get_FieldIndexByName("Jenis Pendidikan"),
                    Convert.ToInt32(txtShapeIndex.Text), cboJenisPendidikan.Text);
                sf.EditCellValue(sf.Table.get_FieldIndexByName("Nama Sekolah"),
                    Convert.ToInt32(txtShapeIndex.Text), txtNamaSekolah.Text);
                sf.EditCellValue(sf.Table.get_FieldIndexByName("foto"),
                    Convert.ToInt32(txtShapeIndex.Text), txtFoto.Text);
                sf.StopEditingTable();
                sf.Save();

                cmdEdit.Text = "Edit";
                cboJenisPendidikan.Enabled = false;
                txtNamaSekolah.ReadOnly = true;
                cmdBrowse.Enabled = false;
                FormMainWindowObject.axMap1.Redraw2(tkRedrawType.RedrawAll);
                FormMainWindowObject.axMap1.Refresh();
                this.Hide();
                MessageBox.Show("Data Sudah Tersimpan");
            }
        }

        private void cmdBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = @"D:\";
            ofd.Filter = "JPG (.jpg)|.jpg|JPEG (.jpeg)|.jpeg|PNG (.png)|.png|All files (.)|.";
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string fileName = @Path.GetFileName(ofd.FileName);
                string sourcePath = @Path.GetDirectoryName(ofd.FileName);
                string targetPath = @Path.Combine(FormMainWindow.strFilePath, "Database/Non spasial/Foto");
                string sourceFile = @Path.Combine(sourcePath, fileName);
                string destFile = @Path.Combine(targetPath, fileName);
                File.Copy(sourceFile, destFile, true);
                txtFoto.Text = fileName;
                pictureBox1.ImageLocation = destFile;
            }
            else
            {
                MessageBox.Show("SILAHKAN PILIH FOTO!!!", "Report", MessageBoxButtons.OK);
            }
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            Shapefile sf = FormMainWindowObject.axMap1.get_Shapefile(FormMainWindowObject.handleSaranaPendidikan);
            sf.StartEditingShapes();
            if (sf.EditDeleteShape(Convert.ToInt32(txtShapeIndex.Text)))
            {
                MessageBox.Show("Data Gagal Dihapus !. Error: " + sf.ErrorMsg[sf.LastErrorCode]);
            }
            else
            {
                MessageBox.Show("Data Berhasil Dihapus. Index = " + Convert.ToInt32(txtShapeIndex.Text));
                FormMainWindowObject.axMap1.Redraw2(tkRedrawType.RedrawAll);
                FormMainWindowObject.axMap1.Refresh();
            }
            sf.Save();
            sf.StopEditingShapes();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
            Shapefile sf = FormMainWindowObject.axMap1.get_Shapefile(FormMainWindowObject.handleSaranaPendidikan);
            sf.SelectNone();
        }
        private void PopUp_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            Shapefile sf = FormMainWindowObject.axMap1.get_Shapefile(FormMainWindowObject.handleSaranaPendidikan);
            sf.SelectNone();
        }

        private void FormPopUp_Load(object sender, EventArgs e)
        {

        }
        private void cboJenisPendidikan_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
