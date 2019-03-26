using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Threading;

namespace JuliaSet
{
    class JuliaSet
    {
        private const int MaxThreadCount = 12;

        public Complex CValue { get; }

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

            var threads = new List<Thread>(MaxThreadCount);
            for (int threadId = 0; threadId < MaxThreadCount; ++threadId) {
                var localThreadId = threadId;
                threads.Add(new Thread(() => {
                    for (int i = 0; i < width; ++i) {
                        if (i % MaxThreadCount != localThreadId) {
                            continue;
                        }
                        var xCoordinate = width - i - 1;
                        var real = realMin + i * realStep;
                        for (int j = 0; j < height; ++j) {
                            var imag = realMin + j * imapStep;
                            var zij = new Complex(real, imag);
                            var count = CountIterations(ref zij, maxIteration, rValue);
                            var ratioZR = Complex.Abs(zij) / rValue;
                            lock (bitmap)
                            {
                                bitmap.SetPixel(xCoordinate, j, GetColor(count, maxIteration, ratioZR));
                            }
                        }
                    }
                }));
            }
            threads.ForEach(x => x.Start());
            threads.ForEach(x => x.Join());
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