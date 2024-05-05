using MapWinGIS;
using System;
using System.Windows.Forms;

namespace SIG_MapWinGIS_Nafis
{
    public partial class FormBuffer : Form
    {
        FormMainWindow FormMainWindowObject;
        private bool bufferprocess;

        public FormMainWindow FormMainWindowInitialized { get; }

        public FormBuffer(FormMainWindow formMainWindow)
        {
            InitializeComponent();
            FormMainWindowObject = FormMainWindowInitialized;
        }

        private void Buffer_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < FormMainWindowObject.legend1.Layers.Count; i++)
            {
                if (!cboInput.Items.Contains(FormMainWindowObject.legend1.Layers[i].FileName))
                {
                    cboInput.Items.Contains(FormMainWindowObject.legend1.Layers[i].FileName);
                }
            }
        }

        private void cmdBrowse_Click(object sender, EventArgs e)
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
                strfileshp = ofd.FileName;
                if (!cboInput.Items.Contains(strfileshp))
                {
                    cboInput.Items.Add(strfileshp);
                }
                cboInput.Text = strfileshp;
            }
            else
            {
                MessageBox.Show("Shapefilenya dimaskukkan dulu", "Report",
                    MessageBoxButtons.OKCancel);
            }
        }

        private void cmdBrowseOutput_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            string stroutputshp = "Shapefile belum dimasukkan";

            sfd.Title = "Browse Shapefile";
            sfd.InitialDirectory = @"D:\";
            sfd.Filter = "Shapefile (*.shp)|*.shp|All files (*.*)|*.*";
            sfd.FilterIndex = 1;
            sfd.RestoreDirectory = true;

            if ((sfd.ShowDialog() == DialogResult.OK))
            {
                stroutputshp = sfd.FileName;
                txtOutput.Text = stroutputshp;
            }
            else
            {
                MessageBox.Show("Shapefilenya dimaskukkan dulu", "Report",
                    MessageBoxButtons.OKCancel);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string inputshapefile = Convert.ToString(cboInput.Text);
            double inputdistance = Convert.ToDouble(txtDistance.Text);
            int inputsegment = Convert.ToInt16(txtSegments.Text);
            bool inputselected = Convert.ToBoolean(cboSelesctedOnly.Text);
            bool inputmerge = Convert.ToBoolean(cboMerge.Text);
            bool inputoverwrite = Convert.ToBoolean(cbxOverwrite.Checked);
            string outputshapefile = Convert.ToString(txtOutput.Text);

            Shapefile sf = new Shapefile();
            sf.Open(inputshapefile);

            Utils utils = new Utils();
            utils.ConvertDistance(tkUnitsOfMeasure.umMeters, tkUnitsOfMeasure.umDecimalDegrees, ref inputdistance);

            //var utils = new Utils();

            if (bufferprocess == true)
            {
                this.Hide();
                MessageBox.Show("Sudah Berhasil", "Report", MessageBoxButtons.OK);

                MapWinGIS.Shapefile sfSaranaIbadah = new MapWinGIS.Shapefile();
                sfSaranaIbadah.Open(outputshapefile, null);
                int handleBufferResult = FormMainWindowObject.legend1.Layers.Add(sfSaranaIbadah, true);
                FormMainWindowObject.legend1.GetLayer(handleBufferResult).Name = System.IO.Path.GetFileName(outputshapefile);
                sfSaranaIbadah.Identifiable = true;
            }
            else
            {
                this.Hide();
                MessageBox.Show("Masih Gagal", "Report",
                MessageBoxButtons.OK);
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void formBuffer_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

    }
}
