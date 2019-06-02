using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Windows.Forms;
using JuliaSet;

namespace Gui
{
    public partial class MainForm : Form
    {
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
            new SetViewForm((Bitmap)e.Result).Show();
        }

        private void CreateJuliaSet(object sender, DoWorkEventArgs e)
        {
            var parameters = (JuliaSetParameters) e.Argument;
            var juliaSet = new JuliaSet.JuliaSet(parameters.CValue);
            e.Result = juliaSet.Create(parameters.MaxIteration, parameters.Width, parameters.Height);
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            JuliaSetParameters parameters;
            try
            {
                parameters = this.GetParameters();
            }
            catch (ParsingParameterException exc)
            {
                ShowErrorMessageBox("Bad parameter format: " + exc.ParameterName);
                return;
            }
            catch (Exception exc)
            {
                ShowErrorMessageBox(exc.Message);
                return;
            }
            this.btnCreate.Enabled = false;
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
            try
            {
                return new Complex(
                    ParseDoubleWithBothDelimiters(this.tbReal.Text),
                    ParseDoubleWithBothDelimiters(this.tbImag.Text)
                );
            }
            catch (FormatException)
            {
                throw new ParsingParameterException("Complex parameter");
            }
        }

        private int GetHeight()
        {
            return ParseSizeControl(this.tbHeight);
        }

        private int GetWidth()
        {
            return ParseSizeControl(this.tbWidth);
        }

        private int GetMaxIteration()
        {
            return (int) ParseUIntControl(this.tbMaxIteration, "Max Iteration");
        }

        private static int ParseSizeControl(Control control)
        {
            return (int) ParseUIntControl(control, "Image size");
        }

        private static uint ParseUIntControl(Control control, string parameterName)
        {
            try
            {
                return UInt32.Parse(control.Text);
            }
            catch (FormatException)
            {
                throw new ParsingParameterException(parameterName);
            }
        }

        private static void ShowErrorMessageBox(string text)
        {
            MessageBox.Show(
                text,
                "Error!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
                );
        }

        private static double ParseDoubleWithBothDelimiters(string text)
        {
            text = text.Replace(',', '.');
            return Double.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
        }

    }
}
