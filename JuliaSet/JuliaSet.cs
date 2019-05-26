using System;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;

namespace JuliaSet
{
    public class JuliaSet
    {
        private const int MaxTaskCount = 12;

        private Complex CValue { get; }

        public JuliaSet(Complex cValue)
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

            int width = bitmap.Width;
            int height = bitmap.Height;

            var tasks = new Task[MaxTaskCount];
            var pixelsPerTask = width / MaxTaskCount;
            for (int taskId = 0; taskId < MaxTaskCount; ++taskId) {
                var firstColumn = taskId * pixelsPerTask;
                var lastColumn = firstColumn + pixelsPerTask;
                tasks[taskId] = Task.Run(() => {
                    for (int i = firstColumn; i < lastColumn; ++i) {
                        var xCoordinate = width - i - 1;
                        var real = realMin + i * realStep;
                        for (int j = 0; j < height; ++j) {
                            var imag = realMin + j * imapStep;
                            var zij = new Complex(real, imag);
                            var count = CountIterations(ref zij, maxIteration, rValue);
                            var ratioZR = Complex.Abs(zij) / rValue;
                            var color = GetColor(count, maxIteration, ratioZR);
                            lock (bitmap)
                            {
                                bitmap.SetPixel(xCoordinate, j, color);
                            }
                        }
                    }
                });
            }
            Task.WaitAll(tasks);
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
                Convert.ToByte(255 * Math.Min(1, ratioZR))
            );
        }
    }
}