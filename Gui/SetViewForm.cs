using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JuliaSet
{
    public partial class SetViewForm : Form
    {
        private Bitmap image;
        private readonly SaveFileDialog dialog;

        public SetViewForm()
        {
            InitializeComponent();
            this.image = null;
            this.dialog = CreateBmpFileDialog();
        }

        public SetViewForm(Bitmap image)
            : this()
        {
            this.image = image;
            this.ShowImage();
        }

        private void ShowImage()
        {
            this.pbSet.Image = this.image;
        }

        private void SaveImage(object sender, EventArgs e)
        {
            var filename = this.SelectFilenameWithDialog();
            if (!String.IsNullOrEmpty(filename))
            {
                this.image.Save(filename);
            }
        }

        private string SelectFilenameWithDialog()
        {
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                return dialog.FileName;
            }
            return String.Empty;
        }

        private static SaveFileDialog CreateBmpFileDialog()
        {
            return new SaveFileDialog
            {
                Filter = "Bitmap Images | *.bmp",
                RestoreDirectory = true,
            };
        }
        private void SetViewFormClosed(object sender, FormClosedEventArgs e)
        {
            this.dialog.Dispose();
        }
    }
}
