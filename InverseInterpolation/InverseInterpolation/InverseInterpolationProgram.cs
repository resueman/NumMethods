using InverseInterpolation.Interpolation;
using InverseInterpolation.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InverseInterpolation
{
    class InverseInterpolationProgram
    {
        private Func<double, double> function = x => Math.Log(1 + x, Math.E);
        private Segment segment = new(-0.5, 3);
        private int maxNodeNumber = 20; // m
        private int polynomialDegree = 10; // n
        private double targetFunctionValue = 1.3862961;

        private double precision = Math.Pow(10, -12); 
        private int separationStepCount = 1000; 
        private Dictionary<double, double> tableWithPredefinedValues;

        public InverseInterpolationProgram(Func<double, double> function, Segment segment, int maxInterpolationNodeNumber,
            int polynomialDegree, double targetFunctionValue)
        {
            this.function = function;
            this.segment = segment;
            this.maxNodeNumber = maxInterpolationNodeNumber;
            this.polynomialDegree = polynomialDegree;
            this.targetFunctionValue = targetFunctionValue;
        }

        public InverseInterpolationProgram()
        { }

        public void Start()
        {
            Console.WriteLine("ЗАДАЧА ОБРАТНОГО ИНТЕРПОЛИРОВАНИЯ");
            Console.WriteLine("Вариант: 2 --- ln(1 + x)");

            var ui = new UserInteractionInterface();
            while (true)
            {
                var userChoice = ui.ProcessUserInput();
                switch (userChoice)
                {
                    case UserChoice.UseDefaultParams:
                        break;
                    case UserChoice.EnterAllParams:
                        ui.ReadSegmentBorders(out segment);
                        ui.ReadMaxInterpolationNodeNumber(out maxNodeNumber);
                        break;
                }

                Console.WriteLine("----------------------------------------------");
                Console.WriteLine("СПОСОБ: Поиск корней уравнения P_n(x) - F = 0.");
                Console.WriteLine("----------------------------------------------\n");

                tableWithPredefinedValues = userChoice != UserChoice.EnterSomeParams 
                    ? BuildATableWithPredefinedValues() 
                    : tableWithPredefinedValues;
                Console.WriteLine("Исходная таблица значений функции:");
                PrintTable(tableWithPredefinedValues.OrderBy(p => p.Key).ToList());

                if (userChoice != UserChoice.UseDefaultParams)
                {
                    ui.ReadPoint(out targetFunctionValue);
                    ui.ReadPolynomialDegree(maxNodeNumber, out polynomialDegree);
                    ui.ReadPrecisionDegree(out precision);
                }

                var nearestSortedNodesValuesTable = GetNearestSortedNodesValuesTable(tableWithPredefinedValues, targetFunctionValue, polynomialDegree);
                Console.WriteLine("Таблица выбранных для интерполирования узлов:");
                PrintTable(nearestSortedNodesValuesTable);

                var polynomial = new LagrangePolynomial(nearestSortedNodesValuesTable);
                double polynomialMinusTarget(double x) => polynomial.GetValue(x) - targetFunctionValue;
                var rootFinder = new RootFinder.SecantMethodRootFinder(polynomialMinusTarget, segment, precision, separationStepCount);
                var rootSearchingMethodResult = rootFinder.FindRoots()
                    .Select(r => new InverseInterpolationResult(r.FinalApproximationToTheRoot, r.AbsoluteDiscrepancyValue));

                Console.WriteLine("---------------------------------------------------------");
                Console.WriteLine("СПОСОБ: Алгебраическое интерполирование обратной функции.");
                Console.WriteLine("---------------------------------------------------------\n");
                
                var inverseTable = InverseTable(tableWithPredefinedValues);
                Console.WriteLine("Перевернутая таблица исходных значений функции:");
                PrintTable(inverseTable.OrderBy(p => p.Key).ToList());

                var nearestSortedNodesValuesInverseTable = GetNearestSortedNodesValuesTable(inverseTable, targetFunctionValue, polynomialDegree);
                Console.WriteLine("Таблица выбранных для интерполирования узлов:");
                PrintTable(nearestSortedNodesValuesInverseTable);

                var polynomialForInverseMethod = new LagrangePolynomial(nearestSortedNodesValuesInverseTable);
                var valueOfLagrangeInF = polynomialForInverseMethod.GetValue(targetFunctionValue);
                var inverseFunctionInterpolationMethodResult = new InverseInterpolationResult(valueOfLagrangeInF, Math.Abs(function(valueOfLagrangeInF) - targetFunctionValue));

                PrintAnalytics(rootSearchingMethodResult, inverseFunctionInterpolationMethodResult);
            }
        }

        private Dictionary<double, double> BuildATableWithPredefinedValues()
        {
            var table = new Dictionary<double, double>();
            for (var i = 0; i <= maxNodeNumber; i++)
            {
                var node = segment.Left + i * (segment.Right - segment.Left) / maxNodeNumber;
                table.Add(node, function(node));
            }
            return table;
        }

        private static Dictionary<double, double> InverseTable(Dictionary<double, double> table)
            => table.ToDictionary(p => p.Value, p => p.Key);

        private static List<KeyValuePair<double, double>> GetNearestSortedNodesValuesTable(Dictionary<double, double> table, double key, int polynomialDegree)
        {
            var list = table.ToList()
                  .OrderBy(x_i => Math.Abs(key - x_i.Key))
                  .Take(polynomialDegree + 1)
                  .ToList();

            list.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            return list;
        }

        private static void PrintTable(List<KeyValuePair<double, double>> table)
        {
            Console.WriteLine("     x          f(x)      ");
            foreach (var pair in table)
            {
                Console.WriteLine($"{pair.Key.ToFormattedString(8)}  {pair.Value.ToFormattedString(9)}");
            }
            Console.WriteLine();
        }

        private void PrintAnalytics(IEnumerable<InverseInterpolationResult> rootSearchingMethodResult, InverseInterpolationResult inverseFunctionInterpolationMethodResult)
        {
            Console.WriteLine("СПОСОБ 1: Алгебраическое интерполирование обратной функции");
            Console.WriteLine($"Значение 'X'  : {inverseFunctionInterpolationMethodResult.ArgumentValue}  |  Модуль невязки: {inverseFunctionInterpolationMethodResult.AbcoluteDisrepancyValue}");
            Console.WriteLine();
            var val = rootSearchingMethodResult.FirstOrDefault();
            if (val == null)
            {
                Console.WriteLine("не удалось найти корни методом секущих, попробуйте изменить параметры");
                return;
            }
            Console.WriteLine("СПОСОБ 2: Поиск корней уравнения P_n(x) - F = 0");
            Console.WriteLine($"Значение 'X'  : {rootSearchingMethodResult.First().ArgumentValue}  |  Модуль невязки: {rootSearchingMethodResult.First().AbcoluteDisrepancyValue}");
            Console.WriteLine();
        }
    }
}
