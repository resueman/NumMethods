using HighestAlgebraicDegreeOfAccuracyQuadratureFormulas.Common;
using NonlinearEquationRootFinder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HighestAlgebraicDegreeOfAccuracyQuadratureFormulas
{
    public class GaussQuadratureFormulaProgram
    {
        private List<Func<double, double>> LejandrePolynomials;
        private List<GaussQuadratureFormula> GaussQuadratureFormulas;
        
        private Function function;
        private List<(int K, double value)> integralValues;
        
        public GaussQuadratureFormulaProgram()
        {
            function = new Function
                ("1 * 1 / sqrt((1 + x ^ 2) * (4 + 3 * x ^ 2))",
                x => 1 / Math.Sqrt((1 + Math.Pow(x, 2)) * (4 + 3 * Math.Pow(x, 2))),
                y => 0.402183050616033);

            GaussQuadratureFormulas = new List<GaussQuadratureFormula>();
        }

        public void Start()
        {
            Console.WriteLine($"Нахождение интеграла функции: {function.StringRepresentation} по КФ Гаусса\n");

            var n = 8;

            LejandrePolynomials = LejandrePolynomialsBuilder.BuildFrom1ToN(n);
            for (var k = 1; k <= n; ++k)
            {
                var lejandrePolynomial = LejandrePolynomials[k];
                var rootFinder = new RootFinder(lejandrePolynomial, new Segment(-1, 1), Math.Pow(10, -14), 10000);
                var lejandreRoots = rootFinder.FindRoots()
                    .Select(ra => ra.Root)
                    .OrderBy(r => r)
                    .ToList();

                var GaussQF = new GaussQuadratureFormula(k, lejandreRoots, LejandrePolynomials);
                GaussQuadratureFormulas.Add(GaussQF);
            }

            while (true)
            {
                var s = new Segment();
                var q = (s.Right - s.Left) / 2;
                integralValues = new List<(int K, double value)>();
                Console.WriteLine("          Узлы и коэффициенты КФ Гаусса                        Узлы и коэффициенты КФ, подобной КФ Гаусса    ");
                for (var k = 1; k <= n; ++k)
                {
                    var GaussQF = GaussQuadratureFormulas[k - 1];
                    var linearizedNodeCoefficientsPairs = GaussQF
                        .NodeCoefficientPairs
                        .Select(p => (s.Left + (p.x_k + 1) * q, p.A_k * q))
                        .ToList();

                    PrintNodeCoefficientsPairs(GaussQF.NodeCoefficientPairs, linearizedNodeCoefficientsPairs);

                    if (k == 4 || k == 6 || k == 7 || k == 8)
                    {
                        var integralValue = CountIntegral(linearizedNodeCoefficientsPairs);
                        integralValues.Add((k, integralValue));
                    }
                }

                PrintIntegralValues(s);
            }
        }

        private double CountIntegral(List<(double, double)> linearized)
        {
            var value = 0.0;
            foreach (var (y_k, B_k) in linearized)
            {
                value += B_k * function.Func(y_k);
            }
            return value;
        }

        public void PrintNodeCoefficientsPairs(List<(double, double)> nodeCoefficientPairs, List<(double, double)> linearizedNodeCoefficientPairs)
        {
            Console.WriteLine($"N = {nodeCoefficientPairs.Count}");
            Console.WriteLine("-------------------------------------------------          -------------------------------------------------");

            var k = 1;
            var checkSum_A_k = 0.0;
            var checkSum_B_k = 0.0;
            for (var i = 0; i < nodeCoefficientPairs.Count; ++i)
            {
                var (x_k, A_k) = nodeCoefficientPairs[i];
                var (y_k, B_k) = linearizedNodeCoefficientPairs[i];
                Console.WriteLine(string.Format("|{0,23}|{1,23}|{2,10}|{3,23}|{4,23}|", $"x_{k}          ", $"A_{k}          ", "", $"y_{k}          ", $"B_{k}          "));
                Console.WriteLine("-------------------------------------------------          -------------------------------------------------");
                Console.WriteLine(string.Format("|{0,23}|{1,23}|{2,10}|{3,23}|{4,23}|", $"{x_k}  ", $"{A_k}  ", "", $"{y_k}  ", $"{B_k}  "));
                Console.WriteLine("-------------------------------------------------          -------------------------------------------------");
                ++k;
                checkSum_A_k += A_k;
                checkSum_B_k += B_k;
            }
            Console.WriteLine(string.Format("{0,49}{1,10}{2,51}", $"Checksum: {checkSum_A_k}", "", $"Checksum: {checkSum_B_k}\n\n"));
        }

        private void PrintIntegralValues(Segment s)
        {
            Console.WriteLine($"Integral: {function.StringRepresentation}\nSegment: [{s.Left}; {s.Right}]\nValue (for [0; 1]): {0.4021830506160328}\n");
            Console.WriteLine("-------------------------------");
            Console.WriteLine(string.Format("|{0,5}|{1,23}|", "K  ", "Value          "));
            Console.WriteLine("-------------------------------");
            foreach (var (K, value) in integralValues)
            {
                Console.WriteLine(string.Format("|{0,5}|{1,23}|", $"{K}  ", $"{value}  "));
                Console.WriteLine("-------------------------------");
            }
            Console.WriteLine("\n\n");
        }
    }    
}
