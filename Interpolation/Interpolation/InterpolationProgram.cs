using System;
using System.Collections.Generic;
using System.Linq;

namespace Interpolation
{
    class InterpolationProgram
    {
        private Func<double, double> function = x => Math.Log(1 + x, Math.E);
        private Segment segment = new Segment(0, 1);
        private int maxNodeNumber = 15; // m
        private int polynomialDegree = 7; // n
        private double x = 0.35;

        private Dictionary<double, double> tableWithPredefinedValues;
        private Dictionary<double, double> nearestSortedNodesValuesTable;

        private LagrangePolynomial polynomialOfLagrange;
        private double valueOfLagrangeInX;
        private double actualInaccuracyOfLagrange;

        private NewtonsPolynomial polynomialOfNewtons;
        private double valueOfNewtonsInX;
        private double actualInaccuracyOfNewtons;

        public InterpolationProgram(Func<double, double> function, Segment segment, int maxInterpolationNodeNumber, 
            int polynomialDegree, double x)
        {
            this.function = function;
            this.segment = segment;
            this.maxNodeNumber = maxInterpolationNodeNumber;
            this.polynomialDegree = polynomialDegree;
            this.x = x;
        }

        public InterpolationProgram()
        {
        }

        public void Start()
        {
            Console.WriteLine("ЗАДАЧА АЛГЕБРАИЧЕСКОГО ИНТЕРПОЛИРОВАНИЯ");
            Console.WriteLine("Вариант: 2 --- ln(1 + x)");

            while (true)
            {
                var useUserParameters = WouldEnterParameters();
                if (useUserParameters)
                {
                    ReadSegmentBorders();
                    ReadMaxInterpolationNodeNumber();
                }

                tableWithPredefinedValues = BuildATableWithPredefinedValues();
                Console.WriteLine("Исходная таблица значений функции:");
                PrintTable(tableWithPredefinedValues);

                if (useUserParameters)
                {
                    ReadX();
                }

                if (useUserParameters)
                {
                    ReadPolynomialDegree();
                }

                nearestSortedNodesValuesTable = GetNearestSortedNodesValuesTable(tableWithPredefinedValues, x, polynomialDegree);
                
                polynomialOfLagrange = new LagrangePolynomial(nearestSortedNodesValuesTable, function);                
                valueOfLagrangeInX = polynomialOfLagrange.GetValue(x);
                actualInaccuracyOfLagrange = polynomialOfLagrange.GetActualInaccuracy(x);

                polynomialOfNewtons = new NewtonsPolynomial(nearestSortedNodesValuesTable, function);
                valueOfNewtonsInX = polynomialOfNewtons.GetValue(x);
                actualInaccuracyOfNewtons = polynomialOfNewtons.GetActualInaccuracy(x);

                PrintResults();
            } 
        }

        private static bool WouldEnterParameters()
        {
            Console.WriteLine("Хотите ли вы ввести параметры? Введите 'Да' или 'Нет'");
            var userChoice = Console.ReadLine().ToLower();
            while (userChoice != "да" && userChoice != "нет")
            {
                Console.WriteLine("Непонятно :)");
                Console.WriteLine("Хотите ли вы ввести параметры? Введите 'Да' или 'Нет'");
                userChoice = Console.ReadLine();
            }
            return userChoice == "да";
        }

        private void ReadSegmentBorders()
        {
            Console.WriteLine("Введите границы отрезка, на котором будут вычеслены точные значение функции:");
            var border1 = 0.0;
            var border2 = 0.0; 
            for (var i = 1; i < 3; ++i)
            {
                do
                {
                    Console.Write($"Граница {i}: ");
                    var input = Console.ReadLine();
                    var isADouble = double.TryParse(input, out var doubleBorder); 
                    var isAnInteger = int.TryParse(input, out var intBorder);
                    var errorMessage = !isADouble && !isAnInteger ? "Граница должна быть вещественным или целым числом" : "";
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        if (i == 1)
                        {
                            border1 = isADouble ? doubleBorder : isAnInteger ? intBorder : border1;
                        }
                        else
                        {
                            border2 = isADouble ? doubleBorder : isAnInteger ? intBorder : border2;
                        }
                        break;
                    }
                    Console.WriteLine(errorMessage + $", попробуйте ввести границу {i} еще раз\n");
                } while (true);
            }
            segment = new Segment(border1, border2);
            Console.WriteLine();
        }

        private void ReadMaxInterpolationNodeNumber()
        {
            do
            {
                Console.Write("Введите максимальный 0-based номер узла интерполирования: ");
                var isAnInteger = int.TryParse(Console.ReadLine(), out maxNodeNumber);
                var errorMessage = !isAnInteger 
                    ? "M должно быть вещественным числом" 
                    : maxNodeNumber < 0 
                        ? "М должно быть больше нуля"
                        : "";

                if (string.IsNullOrEmpty(errorMessage))
                {
                    break;
                }
                Console.WriteLine(errorMessage + ", попробуйте ввести M еще раз\n");
            } while (true);
        }

        private void ReadPolynomialDegree()
        {
            do
            {
                Console.Write($"Введите степень N (N <= {maxNodeNumber}) интерполяционного многочлена: ");
                var isAnInteger = int.TryParse(Console.ReadLine(), out polynomialDegree);
                var errorMessage = !isAnInteger
                    ? "N должно быть целым"
                        : polynomialDegree < 1
                            ? "N дожно быть больше 0"
                            : polynomialDegree > maxNodeNumber
                                ? $"N не должно превосходить {maxNodeNumber}"
                                : "";

                if (string.IsNullOrEmpty(errorMessage))
                {
                    break;
                }
                Console.WriteLine(errorMessage + ", попробуйте ввести N еще раз\n");
            } while (true);
        }

        private void ReadX()
        {
            do
            {
                Console.WriteLine("Введите значение точки интерполирования X, значение в которой будем искать");
                var isADouble = double.TryParse(Console.ReadLine(), out x);
                var errorMessage = !isADouble ? "X должна быть вещественным числом" : "";

                if (string.IsNullOrEmpty(errorMessage))
                {
                    break;
                }
                Console.WriteLine(errorMessage + ", попробуйте ввести X еще раз\n");
            } while (true);
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

        private static Dictionary<double, double> GetNearestSortedNodesValuesTable(Dictionary<double, double> table, double x, int polynomialDegree)
            => table
                .OrderBy(x_i => Math.Abs(x - x_i.Key))
                .Take(polynomialDegree + 1)
                .ToDictionary(p => p.Key, p => p.Value);

        private void PrintResults()
        {
            Console.WriteLine("");
            Console.WriteLine($"Число значений в таблице: {maxNodeNumber + 1}");
            
            Console.WriteLine("Исходная таблица значений функции:");
            PrintTable(tableWithPredefinedValues);
            
            Console.WriteLine($"Точка интерполирования {x}");
            
            Console.WriteLine($"Степень многочлена n: {polynomialDegree}");
            
            Console.WriteLine("Отсортированная таблица из ближайших к Х узлов и их значений, по которым строятся многочлены Лагранжа и Ньютона");
            PrintTable(nearestSortedNodesValuesTable);
            
            Console.WriteLine($"Значение интерполяционнго многочлена в форме Лагранжа в Х: {valueOfLagrangeInX}");
            Console.WriteLine($"Абсолютная фактическая погрешность для формы Лагранжа: {actualInaccuracyOfLagrange}");
            Console.WriteLine($"Значение интерполяционнго многочлена в форме Ньютона в Х: {valueOfNewtonsInX}");
            Console.WriteLine($"Абсолютная фактическая погрешность для формы Ньютона: {actualInaccuracyOfNewtons}");
            Console.WriteLine();
        }

        private static void PrintTable(Dictionary<double, double> table)
        {
            Console.WriteLine("    x          f(x)      ");
            foreach (var key in table.Keys.OrderBy(k => k))
            {
                Console.WriteLine($"{key.ToFormattedString(8)}  {table[key].ToFormattedString(8)}");
            }
            Console.WriteLine();
        }
    }
}
