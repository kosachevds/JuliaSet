using System;
using System.Diagnostics;

namespace JuliaSet
{
    class Program
    {
        static void Main(string[] args)
        {
            var juliaSet = new JuliaSet(new System.Numerics.Complex(-0.8, 0.156));
            var sw = new Stopwatch();

            sw.Start();
            juliaSet.Create("juliaSet.bmp", 100, 5000, 5000);
            sw.Stop();

            Console.WriteLine("Elapsed: {0}, sec", sw.ElapsedMilliseconds / 1000.0);

        }
    }
}
