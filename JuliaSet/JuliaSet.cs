using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;

namespace JuliaSet
{
    public class JuliaSet
    {
        private int ProcessorCount { get; }
        private Complex MappingParameter { get; }
        private double Limit { get; }
        private double ImagMin { get; }
        private double RealMin { get; }

        public JuliaSet(Complex mappingParameter)
        {
            this.MappingParameter = mappingParameter;
            this.ProcessorCount = Environment.ProcessorCount;
            this.Limit = ComputeLimit(mappingParameter);
            this.RealMin = -Limit;
            this.ImagMin = -Limit;
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
            var tasks = CreateTasks(bitmap.Width, bitmap.Height, maxIteration);
            FillBitmap(tasks, bitmap);
        }

        private List<Task<Color[,]>> CreateTasks(int width, int height, int maxIteration)
        {
            var realStep = 2 * Limit / width;
            var imagStep = 2 * Limit / height;
            var columnsPerTask = width / ProcessorCount;
            var tasks = new List<Task<Color[,]>>(ProcessorCount);
            for (int taskId = 0; taskId < ProcessorCount; ++taskId)
            {
                var firstColumn = taskId * columnsPerTask;
                var lastColumn = Math.Min(width, firstColumn + columnsPerTask);
                tasks.Add(Task.Run(() =>
                {
                    // TODO: functiom 'CreateTask'
                    var colors = new Color[lastColumn - firstColumn, height];
                    for (int i = firstColumn; i < lastColumn; ++i)
                    {
                        var iShifted = i - firstColumn;
                        var real = RealMin + i * realStep;
                        for (int j = 0; j < height; ++j)
                        {
                            var imag = ImagMin + j * imagStep;
                            var zij = new Complex(real, imag);
                            var count = CountIterations(zij, maxIteration);
                            colors[iShifted, j] = GetColor(count, maxIteration);
                        }
                    }
                    return colors;
                }));
            }
            return tasks;
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

        private static void FillBitmap(IReadOnlyList<Task<Color[,]>> tasks, Bitmap bitmap)
        {
            var offset = 0;
            foreach (var task in tasks)
            {
                var colors = task.Result;
                for (int i = 0; i < colors.GetLength(0); ++i)
                {
                    var x = bitmap.Width - (i + offset) - 1;
                    for (int j = 0; j < colors.GetLength(1); ++j)
                    {
                        bitmap.SetPixel(x, j, colors[i, j]);
                    }
                }
                offset += colors.GetLength(0);
            }
        }

        private static double ComputeLimit(Complex parameter)
        {
            return (1 + Math.Sqrt(1 + 4 * Complex.Abs(parameter))) / 2;
        }

        private static Color GetColor(int count, int maxIteration)
        {
            var countRatio = (double)count / maxIteration;
            return Color.FromArgb(
                255,
                Convert.ToByte(255 * (1 - countRatio)),
                Convert.ToByte(255 * countRatio),
                Convert.ToByte(255 * countRatio)
            );
        }
    }
}