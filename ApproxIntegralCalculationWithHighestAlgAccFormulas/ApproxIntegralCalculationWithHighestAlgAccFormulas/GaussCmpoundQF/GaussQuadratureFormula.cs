using System;
using System.Collections.Generic;
using System.Linq;

namespace ApproxIntegralCalculationWithHighestAlgAccFormulas
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

        public void PrintNodeCoefficientsPairs()
        {
            Console.WriteLine($"N = {NodeCoefficientPairs.Count}");
            Console.WriteLine("-------------------------------------------------");

            var k = 1;
            foreach (var (x_k, A_k) in NodeCoefficientPairs)
            {
                Console.WriteLine(string.Format("|{0,23}|{1,23}|", $"x_{k}          ", $"A_{k}          "));
                Console.WriteLine("-------------------------------------------------");
                Console.WriteLine(string.Format("|{0,23}|{1,23}|", x_k, A_k));
                Console.WriteLine("-------------------------------------------------");
                ++k;
            }
            Console.WriteLine($"Checksum: {CheckSum}\n\n");
        }
    }
}

//public double CalculateIntegral(Function function, Segment segment = null)
//{
//    if (segment == null)
//    {
//        return CalculateIntegralWithQuadratureFormula(function, NodeCoefficientPairs);
//    }

//    var q = (segment.Right - segment.Left) / 2;
//    var linearizedNodeCoefficientsPairs = NodeCoefficientPairs
//        .Select(p => (segment.Left + (p.x_k + 1) * q, p.A_k * q))
//        .ToList();

//    return CalculateIntegralWithQuadratureFormula(function, linearizedNodeCoefficientsPairs);
//}

//private double CalculateIntegralWithQuadratureFormula(Function function,
//    List<(double x_k, double A_k)> nodeCoefficientPairs)
//{
//    var value = 0.0;
//    foreach (var (x_k, A_k) in NodeCoefficientPairs)
//    {
//        value += A_k * function.Func(x_k);
//    }
//    return value;
//}