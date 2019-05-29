using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;

namespace JuliaSet
{
    public class JuliaSet
    {
        private int MaxTaskCount { get; }
        private Complex MappingParameter { get; }
        private double Limit { get; }

        public JuliaSet(Complex mappingParameter)
        {
            this.MappingParameter = mappingParameter;
            this.MaxTaskCount = Environment.ProcessorCount;
            this.Limit = ComputeLimit(mappingParameter);
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
            var realMin = -Limit;
            var realStep = 2 * Limit / bitmap.Width;

            var imagMin = -Limit;
            var imagStep = 2 * Limit / bitmap.Height;

            int height = bitmap.Height;
            var tasks = new List<Task<Color[,]>>(MaxTaskCount);
            var columnsPerTask = bitmap.Width / MaxTaskCount;
            for (int taskId = 0; taskId < MaxTaskCount; ++taskId) {
                var firstColumn = taskId * columnsPerTask;
                var lastColumn = Math.Min(bitmap.Width, firstColumn + columnsPerTask);
                tasks.Add(Task.Run(() =>
                {
                    var colors = new Color[lastColumn - firstColumn, height];
                    for (int i = firstColumn; i < lastColumn; ++i) {
                        var iShifted = i - firstColumn;
                        var real = realMin + i * realStep;
                        for (int j = 0; j < height; ++j) {
                            var imag = imagMin + j * imagStep;
                            var zij = new Complex(real, imag);
                            var count = CountIterations(zij, maxIteration);
                            var ratioZR = Complex.Abs(zij) / Limit;
                            colors[iShifted, j] = GetColor(count, maxIteration, ratioZR);
                        }
                    }
                    return colors;
                }));
            }
            FillBitmap(tasks, bitmap, columnsPerTask);
        }

        private int CountIterations(Complex initialZ, int maxIteration)
        {
            var count = 0;
            var lastZ = initialZ;
            var absLastZ = Complex.Abs(lastZ);
            while (count < maxIteration && absLastZ > 0 && absLastZ <= this.Limit)
            {
                lastZ = lastZ * lastZ + this.MappingParameter;
                absLastZ = Complex.Abs(lastZ);
                ++count;
            }
            return count;
        }

        private static void FillBitmap(IReadOnlyList<Task<Color[,]>> tasks, Bitmap bitmap, int columnsPerTask)
        {
            for (int taskId = 0; taskId < tasks.Count; ++taskId)
            {
                var offset = taskId * columnsPerTask;
                var colors = tasks[taskId].Result;
                for (int i = 0; i < colors.GetLength(0); ++i)
                {
                    var x = bitmap.Width - (i + offset) - 1;
                    for (int j = 0; j < colors.GetLength(1); ++j)
                    {
                        bitmap.SetPixel(x, j, colors[i, j]);
                    }
                }
            }
        }

        private static double ComputeLimit(Complex parameter)
        {
            return (1 + Math.Sqrt(1 + 4 * Complex.Abs(parameter))) / 2;
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