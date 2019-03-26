using System;
using System.Numerics;

namespace JuliaSet
{
    class JuliaSet
    {
        public Complex CValue { get; }

        public JuliaSet(double cValue)
        {
            this.CValue = cValue;
        }

        public void Create(string filename, int maxIteration, int width, int height)
        {

        }

        private double ComputeRValue()
        {
            return (1 + Math.Sqrt(1 + 4 * Complex.Abs(this.CValue))) / 2;
        }
    }
}