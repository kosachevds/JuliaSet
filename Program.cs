using System;
using System.Diagnostics;

namespace JuliaSet
{
    class Program
    {
        static void Main(string[] args)
        {
            var juliaSet = new JuliaSet(new System.Numerics.Complex(-0.74543, 0.11301));
            var sw = new Stopwatch();

            Console.WriteLine("Working...");
            sw.Start();
            juliaSet.Create("juliaSet.bmp", 500, 10000, 10000);
            sw.Stop();

            Console.WriteLine("Elapsed, ms: {0}", sw.ElapsedMilliseconds);
        }
    }
}
