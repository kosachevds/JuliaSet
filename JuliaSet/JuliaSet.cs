using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;

namespace JuliaSet
{
    public class JuliaSet
    {
        private readonly int MaxTaskCount;

        private Complex CValue { get; }

        public JuliaSet(Complex cValue)
        {
            this.CValue = cValue;
            this.MaxTaskCount = Environment.ProcessorCount;
        }

        public void Create(string filename, int maxIteration, int width, int height)
        {
            var bitmap = this.Create(maxIteration, width, height);
            bitmap.Save(filename);
        }

        public Bitmap Create(int maxIteration, int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            PlotOnBitmap(bitmap, maxIteration);
            return bitmap;
        }

        private void PlotOnBitmap(Bitmap bitmap, int maxIteration)
        {
            var rValue = this.ComputeRValue();

            var realMin = -rValue;
            var realMax = rValue;
            var realStep = (realMax - realMin) / bitmap.Width;

            var imagMin = -rValue;
            var imagMax = rValue;
            var imagStep = (imagMax - imagMin) / bitmap.Height;

            int width = bitmap.Width;
            int height = bitmap.Height;
            var tasks = new List<Task<Color[,]>>(MaxTaskCount);
            var columnsPerTask = width / MaxTaskCount;
            for (int taskId = 0; taskId < MaxTaskCount; ++taskId) {
                var firstColumn = taskId * columnsPerTask;
                var lastColumn = firstColumn + columnsPerTask;
                tasks.Add(Task.Run(() =>
                {
                    var colors = new Color[columnsPerTask, height];
                    for (int i = firstColumn; i < lastColumn; ++i) {
                        var iShifted = i - firstColumn;
                        var real = realMin + i * realStep;
                        for (int j = 0; j < height; ++j) {
                            var imag = imagMin + j * imagStep;
                            var zij = new Complex(real, imag);
                            var count = CountIterations(ref zij, maxIteration, rValue);
                            var ratioZR = Complex.Abs(zij) / rValue;
                            colors[iShifted, j] = GetColor(count, maxIteration, ratioZR);
                        }
                    }
                    return colors;
                }));
            }

            for (int taskId = 0; taskId < tasks.Count; ++taskId)
            {
                var offset = taskId * columnsPerTask;
                var colors = tasks[taskId].Result;
                for (int i = 0; i < colors.GetLength(0); ++i)
                {
                    var x = width - (i + offset) - 1;
                    for (int j = 0; j < colors.GetLength(1); ++j)
                    {
                        bitmap.SetPixel(x, j, colors[i, j]);
                    }
                }
            }
        }

        private int CountIterations(ref Complex initialZ, int maxIteration, double rValue)
        {
            var count = 0;
            var lastZ = initialZ;
            var absLastZ = Complex.Abs(lastZ);
            while (count < maxIteration && absLastZ > 0 && absLastZ <= rValue)
            {
                lastZ = lastZ * lastZ + this.CValue;
                absLastZ = Complex.Abs(lastZ);
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
                Convert.ToByte(255 * (1 - countRatio)),
                Convert.ToByte(255 * countRatio),
                Convert.ToByte(255 * Math.Min(1, ratioZR))
            );
        }
    }
}