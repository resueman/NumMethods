using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApproxIntegralCalculationWithHighestAlgAccFormulas
{
    class CompoundGaussQF
    {
        private List<Func<double, double>> LejandrePolynomials;

        public GaussQuadratureFormula GaussQuadratureFormula { get; private set; }

        public int NodeCount { get; private set; }

        public CompoundGaussQF(int N)
        {
            LejandrePolynomials = LejandrePolynomialsBuilder.BuildFrom1ToN(N);
            var lejandrePolynomial = LejandrePolynomials[N];
            var rootFinder = new RootFinder(lejandrePolynomial, new Segment(-1, 1), Math.Pow(10, -14), 10000);
            var lejandreRoots = rootFinder.FindRoots()
                .Select(ra => ra.Root)
                .OrderBy(r => r)
                .ToList();

            GaussQuadratureFormula = new GaussQuadratureFormula(N, lejandreRoots, LejandrePolynomials);
            NodeCount = N;
        }

        public double CalculateIntegral(int m, Segment segment, Function function)
        {
            var h = Math.Abs(segment.Right - segment.Left) / m;
            var sum = 0.0;
            for (var j = 0; j < m; ++j)
            {
                var left = segment.Left + j * h;
                var right = segment.Left + (j + 1) * h;
                var q = (right - left) / 2;

                List<(double y_k, double B_k)> linearized = GaussQuadratureFormula.NodeCoefficientPairs
                    .Select(p => (left + (p.x_k + 1) * q, p.A_k * q))
                    .ToList();
                
                sum += linearized.Sum(p => function.Func(p.y_k) * p.B_k);
            }

            return sum;
        }
    }
}
