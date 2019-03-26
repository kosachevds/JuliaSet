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
    }
}