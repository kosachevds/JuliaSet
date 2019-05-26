using System;
using System.ComponentModel;
using System.Numerics;
using System.Windows.Forms;

namespace Gui
{
    public partial class MainForm : Form
    {
        private const string BitmapFilename = "temp.bmp";
        private readonly BackgroundWorker setCreator;

        public MainForm()
        {
            InitializeComponent();
            this.setCreator = new BackgroundWorker();
            this.setCreator.DoWork += CreateJuliaSet;
            this.setCreator.RunWorkerCompleted += AfterSetCreation;
        }

        private void AfterSetCreation(object sender, RunWorkerCompletedEventArgs e)
        {
            this.btnCreate.Enabled = true;
        }

        private void CreateJuliaSet(object sender, DoWorkEventArgs e)
        {
            var parameters = (JuliaSetParameters) e.Argument;
            var juliaSet = new JuliaSet.JuliaSet(parameters.CValue);
            juliaSet.Create(MainForm.BitmapFilename, parameters.MaxIteration, parameters.Width, parameters.Height);
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            this.btnCreate.Enabled = false;
            var parameters = this.GetParameters();
            this.setCreator.RunWorkerAsync(parameters);
        }

        private JuliaSetParameters GetParameters()
        {
            return new JuliaSetParameters
            {
                CValue = this.GetComplexParameter(),
                Height = this.GetHeight(),
                Width = this.GetWidth(),
                MaxIteration = GetMaxIteration()
            };
        }

        private Complex GetComplexParameter()
        {
            return new Complex(
                Double.Parse(this.tbReal.Text),
                Double.Parse(this.tbImag.Text)
                );
        }

        private int GetHeight()
        {
            return ParseSizeControl(tbHeight);
        }

        private int GetWidth()
        {
            return ParseSizeControl(tbWidth);
        }

        private int GetMaxIteration()
        {
            return (int) ParseUIntControl(this.tbMaxIteration);
        }

        private static int ParseSizeControl(Control control)
        {
            return (int) ParseUIntControl(control);
        }

        private static uint ParseUIntControl(Control control)
        {
            return UInt32.Parse(control.Text);
        }



    }
}
