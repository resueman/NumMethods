using System;
using System.Collections.Generic;
using System.Linq;

namespace HighestAlgebraicDegreeOfAccuracyQuadratureFormulas
{
    class MellerQuadratureFormulaProgram
    {
        private Func<double, double> weightFunction;
        private Func<double, double> function;
        private Func<double, double> integrableFunction;
        private List<int> Ns; 

        public MellerQuadratureFormulaProgram()
        {
            function = x => Math.Pow(Math.E, 2 * x);
            weightFunction = x => 1 / Math.Sqrt(1 - x * x);
            integrableFunction = x => function(x) * weightFunction(x);
        }

        public void Start()
        {
            Console.WriteLine("Вычисление интеграла на [-1; 1] функции w(x) * f(x), где w(x) = 1 / sqrt(1 - x ^ 2), f(x) = e ^ (2 * x), по КФ Меллера\n");
            while (true)
            {
                ReadNs();

                foreach (var n in Ns)
                {
                    var roots = CalculateChebyshevPolynomialRoots(n);
                    var integralValue = Math.PI / n * roots.Sum(x_k => function(x_k));
                    PrintResults(n, roots, integralValue);
                }
            }
        }

        private List<double> CalculateChebyshevPolynomialRoots(int n)
        {
            var roots = new List<double>();
            for (var k = 1; k <= n; ++k)
            {
                var root = Math.Cos(Math.PI * (2.0 * k - 1) / (2.0 * n));
                roots.Add(root);
            }
            return roots;
        }

        public void ReadNs()
        {
            Console.WriteLine("Введите натуральные N1, N2, N3 через пробел: ");
            Ns = Console.ReadLine()
                .Trim().Replace("  ", " ").Split(' ')
                .Select(s => int.Parse(s))
                .ToList();
        }

        private void PrintResults(int n, List<double> roots, double integralValue)
        {
            Console.WriteLine($"N = {n}");
            PrintNodeCoefficientPairs(n, roots);/////////////////////////
            Console.WriteLine($"Значение интеграла: {integralValue}\n\n");
        }

        private void PrintNodeCoefficientPairs(int n, List<double> roots)
        {
            Console.WriteLine("------------------------------------");
            Console.WriteLine(string.Format("|{0,23}|{1,10}|", $"x_k          ", "A_k    "));
            Console.WriteLine("------------------------------------");
            foreach (var root in roots)
            {
                Console.WriteLine(string.Format("|{0,23}|{1,10}|", $"{root}  ", $"pi/{n}   "));
                Console.WriteLine("------------------------------------");
            } 
        }
    }
}
