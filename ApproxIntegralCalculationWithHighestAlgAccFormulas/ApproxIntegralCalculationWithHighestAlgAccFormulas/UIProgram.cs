using ApproxIntegralCalculationWithHighestAlgAccFormulas.GaussTypeQF;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApproxIntegralCalculationWithHighestAlgAccFormulas
{
    class UIProgram
    {
        private Function f;
        private Function p;
        private Function integrableFunction;

        public UIProgram()
        {
            f = new Function(
                "sin(x)", 
                x => Math.Sin(x));

            p = new Function(
                "x ^ 0.25", 
                x => Math.Sqrt(Math.Sqrt(x)));
            
            integrableFunction = new Function(
                "1 * sin(x) * x ^ (1 / 4)",
                x => p.Func(x) * f.Func(x));
        }

        public void Start()
        {
            Console.WriteLine("Приближенное вычисление интегралов с помощью КФ НАСТ\n");
            while (true)
            {
                var segment = new Segment();
                var N = ReadNodeNumber();
                var partitionNumbers = ReadSeveralNubersOfSegmentPartition();

                /// COMPOUND GAUSS QUADRATURE FORMULA
                RunCompoundGaussQuadratureFormulaIntegralCalculation(segment, N, partitionNumbers);

                /// BUILDING GAUSS TYPE FORMULA
                RunGaussTypeQuadratureFormulaIntegralCalculation(segment, N);
                Console.WriteLine("\n\n");
            }
        }

        private void RunCompoundGaussQuadratureFormulaIntegralCalculation(Segment segment, int N, List<int> partitionNumbers)
        {
            Console.WriteLine("Вычисление интеграла с помощью составной КФ Гаусса");
            var cgqf = new CompoundGaussQF(N);
            cgqf.GaussQuadratureFormula.PrintNodeCoefficientsPairs();

            var integralValues = new List<(int m, double value)>();
            foreach (var m in partitionNumbers)
            {
                var integral = cgqf.CalculateIntegral(m, segment, integrableFunction);
                integralValues.Add((m, integral));
            }
            PrintIntegralValues(integralValues, segment, N);
        }

        private void RunGaussTypeQuadratureFormulaIntegralCalculation(Segment segment, int N)
        {
            Console.WriteLine("Вычисление интеграла с помощью КФ Гауссового типа");
            var integral = GaussTypeQuadratureFormulaBuilder.CalculateIntegral(segment, N, integrableFunction);
            Console.WriteLine($"value: {integral}");
        }

        private List<int> ReadSeveralNubersOfSegmentPartition()
        {
            Console.Write("Введите через пробел натуральные m_1, m_2, ..., m_n -- варианты числа разбиений: ");
            var partitionNumbers = Console.ReadLine()
                .Trim().Replace("  ", " ").Split(' ')
                .Select(s => int.Parse(s))
                .ToList();

            Console.WriteLine();

            return partitionNumbers;
        }

        public int ReadNodeNumber()
        {
            var N = 0;
            do
            {
                Console.Write("Введите N -- число узлов участвующих в построении КФ Гаусса: ");
                var isAnInteger = int.TryParse(Console.ReadLine(), out var n);
                var errorMessage = !isAnInteger
                    ? "N должно быть натуральным числом"
                    : n <= 0
                        ? "N должно быть больше нуля"
                        : "";

                if (string.IsNullOrEmpty(errorMessage))
                {
                    N = n;
                    break;
                }
                Console.WriteLine(errorMessage + ", попробуйте ввести N еще раз\n");
            } while (true);
            Console.WriteLine();

            return N;
        }

        private void PrintIntegralValues(List<(int m, double value)> integralValues, Segment s, int n)
        {
            Console.WriteLine($"Integral: {integrableFunction.StringRepresentation}\nSegment: [{s.Left}; {s.Right}]\nN = {n}\n");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine(string.Format("|{0,13}|{1,25}|", "m    ", "Value          "));
            Console.WriteLine("------------------------------------------");
            foreach (var (m, value) in integralValues)
            {
                Console.WriteLine(string.Format("|{0,13}|{1,25}|", $"{m}    ", string.Format("{0:F20}  ", value)));
                Console.WriteLine("------------------------------------------");
            }
            Console.WriteLine("\n\n");
        }
    }
}
