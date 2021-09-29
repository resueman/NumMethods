using System;

namespace NumericalDifferentiation
{
    class ProgramFindingDerivatives
    {
        private int maxNodeNumber; // m // 0-based
        private double startPoint; // a
        private double stepLength; // h
        private Func<double, double> function = x => Math.Pow(Math.E, 4.5 * x);
        private Func<double, double> derivative1 = x => 4.5 * Math.Pow(Math.E, 4.5 * x);
        private Func<double, double> derivative2 = x => 20.25 * Math.Pow(Math.E, 4.5 * x);

        public void Start()
        {
            Console.WriteLine("ЗАДАЧА О НАХОЖДЕНИИ ПРОИЗВОДНЫХ ТАБЛИЧНО-ЗАДДАННОЙ ФУНКЦИИ ПО ФОРМУЛАМ ЧИСЛЕННОГО ДИФФЕРЕНЦИРОВАНИЯ");
            Console.WriteLine("Вариант: 2");

            while (true)
            {
                var useUserParameters = WouldEnterParameters();
                if (!useUserParameters)
                {
                    return;
                }

                ReadMaxNodeNumber();
                ReadStartPoint();
                ReadStepLength();

                var table = new double[maxNodeNumber + 1, 6];
                for (var i = 0; i <= maxNodeNumber; ++i)
                {
                    var x = startPoint + i * stepLength;
                    table[i, 0] = x;
                    table[i, 1] = function(x);
                    table[i, 2] = i == 0
                        ? (-3 * function(x) + 4 * function(x + stepLength) - function(x + 2 * stepLength)) / (2 * stepLength)
                        : i == maxNodeNumber
                            ? (3 * function(x) - 4 * function(x + stepLength) + function(x + 2 * stepLength)) / (2 * stepLength)
                            : (function(x + stepLength) - function(x - stepLength)) / (2 * stepLength);
                    
                    table[i, 3] = Math.Abs(derivative1(x) - table[i, 2]);

                    if (i != 0 && i != maxNodeNumber)
                    {
                        table[i, 4] = (function(x + stepLength) - 2 * function(x) + function(x - stepLength)) / Math.Pow(stepLength, 2);
                        table[i, 5] = Math.Abs(derivative2(x) - table[i, 4]);
                    }
                }
                PrintTable(table);
            }
        }

        private void PrintTable(double[,] table)
        {
            Console.WriteLine(string.Format("|{0,25}|{1,25}|{2,25}|{3,25}|{4,25}|{5,25}|", "xi", "f(x_i)", "f'(x_i)чд", "|f'(xi)т - f'(xi)чд|", "f''(xi)чд", "|f''(xi)т - f''(xi)чд|"));
            for (var i = 0; i <= maxNodeNumber; i++)
            {
                Console.WriteLine(string.Format("|{0,25}|{1,25}|{2,25}|{3,25}|{4,25}|{5,25}|", table[i, 0], table[i, 1], table[i, 2], table[i, 3], table[i, 4], table[i, 5]));
            }
            Console.WriteLine();
        }

        private static bool WouldEnterParameters()
        {
            Console.WriteLine("Хотите ли вы ввести параметры? Введите 'Да' или 'Нет'");
            var userChoice = Console.ReadLine();
            while (userChoice != "Да" && userChoice != "Нет")
            {
                Console.WriteLine("Непонятно :)");
                Console.WriteLine("Хотите ли вы ввести параметры? Введите 'Да' или 'Нет'");
                userChoice = Console.ReadLine();
            }
            return userChoice == "Да";
        }

        private void ReadMaxNodeNumber()
        {
            do
            {
                Console.Write("Введите максимальный номер узла M: ");
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
                Console.WriteLine();
                Console.WriteLine(errorMessage + ", попробуйте ввести M еще раз\n");
            } while (true);
        }

        private void ReadStartPoint()
        {
            do
            {
                Console.Write("Введите значение стартовой точки: ");
                var isADouble = double.TryParse(Console.ReadLine(), out startPoint);
                var errorMessage = !isADouble ? "Значение стартовой точки должно быть вещественным или целым числом" : "";

                if (string.IsNullOrEmpty(errorMessage))
                {
                    break;
                }
                Console.WriteLine();
                Console.WriteLine(errorMessage + ", попробуйте ввести значение стартовой точки еще раз\n");
            } while (true);
        }

        private void ReadStepLength()
        {
            do
            {
                Console.Write("Введите длину шага: ");
                var isADouble = double.TryParse(Console.ReadLine(), out stepLength);
                var errorMessage = !isADouble 
                    ? "Значение длины шага должно быть вещественным или целым числом"
                    : stepLength <= 0 
                        ? "Значение длины шага должно быть болььше 0" 
                        : "";

                if (string.IsNullOrEmpty(errorMessage))
                {
                    break;
                }
                Console.WriteLine();
                Console.WriteLine(errorMessage + ", попробуйте ввести длину шага еще раз\n");
            } while (true);
        }
    }
}
