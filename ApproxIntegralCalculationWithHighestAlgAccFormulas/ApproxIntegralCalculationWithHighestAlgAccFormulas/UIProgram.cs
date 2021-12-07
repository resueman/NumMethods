using System;
using System.Collections.Generic;
using System.Linq;

namespace ApproxIntegralCalculationWithHighestAlgAccFormulas
{
    class UIProgram
    {
        private Function function;

        public UIProgram()
        {
            function = new Function
                ("1 * sin(x) * x ^ (1 / 4)",
                x => Math.Sin(x) * Math.Sqrt(Math.Sqrt(x)));
        }

        public void Start()
        {
            while (true)
            {
                var segment = new Segment();
                var N = ReadNodeNumber();
                var partitionNumbers = ReadSeveralNubersOfSegmentPartition();
                
                var cgqf = new CompoundGaussQF(N);
                cgqf.GaussQuadratureFormula.PrintNodeCoefficientsPairs();
                
                var integralValues = new List<(int m, double value)>();
                foreach (var m in partitionNumbers)
                {
                    var integral = cgqf.CalculateIntegral(m, segment, function);
                    integralValues.Add((m, integral));
                }
                PrintIntegralValues(integralValues, segment, N);
            }
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
            Console.WriteLine($"Integral: {function.StringRepresentation}\nSegment: [{s.Left}; {s.Right}]\nN = {n}\n");
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
