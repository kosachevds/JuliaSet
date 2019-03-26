using System;
using System.Drawing;
using System.Numerics;

namespace JuliaSet
{
    class JuliaSet
    {
        private const int MaxThreadCount = 12;

        public Complex CValue { get; }

        public JuliaSet(double cValue)
        {
            this.CValue = cValue;
        }

        public void Create(string filename, int maxIteration, int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            PlotOnBitmap(bitmap, maxIteration);
            bitmap.Save(filename);
        }

        private void PlotOnBitmap(Bitmap bitmap, int maxIteration)
        {
            var rValue = this.ComputeRValue();

            var realMin = -rValue;
            var realMax = rValue;
            var realStep = (realMax - realMin) / bitmap.Width;

            var imagMin = -rValue;
            var imagMax = rValue;
            var imapStep = (imagMax - imagMin) / bitmap.Height;

            for (int i = 0; i < bitmap.Width; ++i) {
                var xCoordinate = bitmap.Width - i - 1;
                var real = realMin + i * realStep;
                for (int j = 0; j < bitmap.Height; ++j) {
                    var imag = realMin + j * imapStep;
                    var zij = new Complex(real, imag);
                    var count = CountIterations(ref zij, maxIteration, rValue);
                    var ratioZR = Complex.Abs(zij) / rValue;
                    bitmap.SetPixel(xCoordinate, j, GetColor(count, maxIteration, ratioZR));
                }
            }
        }

        private int CountIterations(ref Complex initialZ, int maxIteration, double rValue)
        {
            var count = 0;
            var lastZ = initialZ;
            while (count < maxIteration && Complex.Abs(lastZ) > 0)
            {
                lastZ = lastZ * lastZ + this.CValue;
                ++count;
            }
            return count;
        }

        private double ComputeRValue()
        {
            return (1 + Math.Sqrt(1 + 4 * Complex.Abs(this.CValue))) / 2;
        }

        private static Color GetColor(int count, int maxIteration, double ratioZR)
        {
            var countRatio = (double)count / maxIteration;
            return Color.FromArgb(
                255,
                Convert.ToByte(255 * countRatio),
                Convert.ToByte(255 * (1 - countRatio)),
                Convert.ToByte(255 * Math.Max(1, ratioZR))
            );
        }
    }
}