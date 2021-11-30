using HighestAlgebraicDegreeOfAccuracyQuadratureFormulas.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HighestAlgebraicDegreeOfAccuracyQuadratureFormulas
{
    public class GaussQuadratureFormula
    {
        public GaussQuadratureFormula(int n, List<double> roots, List<Func<double, double>> LejandrePolynomialsToN)
        {
            if (LejandrePolynomialsToN == null || LejandrePolynomialsToN.Count < n + 1)
            {
                throw new Exception();
            }

            roots = roots.OrderBy(r => r).ToList();

            NodeCoefficientPairs = new List<(double x_k, double A_k)>();
            foreach (var x_k in roots)
            {
                var P_n_minus_1 = LejandrePolynomialsToN[n - 1].Invoke(x_k);
                var A_k = 2 * (1 - Math.Pow(x_k, 2)) / (Math.Pow(n, 2) * Math.Pow(P_n_minus_1, 2));
                NodeCoefficientPairs.Add((x_k, A_k));
            }
            N = n;
            AlgebraicDegreeOfAccuracy = 2 * n - 1;
        }

        public List<(double x_k, double A_k)> NodeCoefficientPairs { get; private set; }

        public int AlgebraicDegreeOfAccuracy { get; private set; }

        public int N { get; private set; }

        public double CheckSum => NodeCoefficientPairs.Sum(p => p.A_k);

        public double CalculateIntegral(Function function)
        {
            var value = 0.0;
            foreach (var (x_k, A_k) in NodeCoefficientPairs)
            {
                value += A_k * function.Func(x_k);
            }
            return value;
        }
    }
}
