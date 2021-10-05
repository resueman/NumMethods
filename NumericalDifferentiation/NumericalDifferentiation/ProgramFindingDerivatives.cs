using System;

namespace NumericalDifferentiation
{
    class ProgramFindingDerivatives
    {
        private int maxNodeNumber = 10; // m // 0-based
        private double startPoint = 0.5; // a
        private double stepLength = 0.1; // h > 0
        private Func<double, double> function = x => Math.Pow(Math.E, 4.5 * x);
        private Func<double, double> derivative1 = x => 4.5 * Math.Pow(Math.E, 4.5 * x);
        private Func<double, double> derivative2 = x => 20.25 * Math.Pow(Math.E, 4.5 * x);

        public void Start()
        {
            Console.WriteLine("ЗАДАЧА О НАХОЖДЕНИИ ПРОИЗВОДНЫХ ТАБЛИЧНО-ЗАДДАННОЙ ФУНКЦИИ ПО ФОРМУЛАМ ЧИСЛЕННОГО ДИФФЕРЕНЦИРОВАНИЯ");
            Console.WriteLine("Вариант: 2 --- e^(4.5 * x)");

            while (true)
            {
                var useUserParameters = WouldEnterParameters();
                if (useUserParameters)
                {
                    ReadMaxNodeNumber();
                    ReadStartPoint();
                    ReadStepLength();
                }

                var table = new double[maxNodeNumber + 1, 6];
                for (var i = 0; i <= maxNodeNumber; ++i)
                {
                    var x = startPoint + i * stepLength;
                    table[i, 0] = x;
                    table[i, 1] = function(x);
                }

                for (var i = 0; i <= maxNodeNumber; ++i)
                {
                    table[i, 2] = i == 0
                        ? (-3 * table[i, 1] + 4 * table[i + 1, 1] - table[i + 2, 1]) / (2 * stepLength)
                        : i == maxNodeNumber
                            ? (3 * table[i, 1] - 4 * table[i - 1, 1] + table[i - 2, 1]) / (2 * stepLength)
                            : (table[i + 1, 1] - table[i - 1, 1]) / (2 * stepLength);

                    var x = startPoint + i * stepLength;
                    table[i, 3] = Math.Abs(derivative1(x) - table[i, 2]);

                    if (i != 0 && i != maxNodeNumber)
                    {
                        table[i, 4] = (table[i + 1, 1] - 2 * table[i, 1] + table[i - 1, 1]) / Math.Pow(stepLength, 2);
                        table[i, 5] = Math.Abs(derivative2(x) - table[i, 4]);
                    }
                }
                PrintTable(table);
            }
        }

        private void PrintTable(double[,] table)
        {
            Console.WriteLine(string.Format("|{0,40}|{1,40}|{2,40}|{3,40}|", "xi", "f(x_i)", "f'(x_i)чд", "|f'(xi)т - f'(xi)чд|"));
            for (var i = 0; i <= maxNodeNumber; i++)
            {
                Console.WriteLine(string.Format("|{0,40}|{1,40}|{2,40}|{3,40}|", table[i, 0], table[i, 1], table[i, 2], table[i, 3]));
            }
            Console.WriteLine();

            Console.WriteLine(string.Format("|{0,40}|{1,40}|{2,40}|{3,40}|", "xi", "f(x_i)", "f''(xi)чд", "|f''(xi)т - f''(xi)чд|"));
            for (var i = 0; i <= maxNodeNumber; i++)
            {
                Console.WriteLine(string.Format("|{0,40}|{1,40}|{2,40}|{3,40}|", table[i, 0], table[i, 1], table[i, 4], table[i, 5]));
            }
            Console.WriteLine();
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
            Console.WriteLine();
            return userChoice == "да";
        }

        private void ReadMaxNodeNumber()
        {
            do
            {
                Console.Write("Введите максимальный номер узла M начиная с 0: ");
                var isAnInteger = int.TryParse(Console.ReadLine(), out maxNodeNumber);
                var errorMessage = !isAnInteger
                    ? "M должно быть целым числом"
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
            Console.WriteLine();
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
            Console.WriteLine();
        }

        private void ReadStepLength()
        {
            double stepLengthBase;
            double stepLengthDegree;
            do
            {
                Console.WriteLine("Ввод длины шага h");
                Console.Write("Введите основание h: ");
                var isADouble = double.TryParse(Console.ReadLine(), out stepLengthBase);
                var errorMessage = !isADouble 
                    ? "Значение основания длины шага должно быть вещественным или целым числом"
                    : stepLengthBase <= 0 
                        ? "Значение основания длины шага должно быть больше 0" 
                        : "";

                if (string.IsNullOrEmpty(errorMessage))
                {
                    break;
                }
                Console.WriteLine();
                Console.WriteLine(errorMessage + ", попробуйте ввести основание длины шага еще раз\n");
            } while (true);

            do
            {
                Console.Write("Введите степень h: ");
                var isADouble = double.TryParse(Console.ReadLine(), out stepLengthDegree);
                var errorMessage = !isADouble
                    ? "Значение степени длины шага должно быть вещественным или целым числом"
                    : "";

                if (string.IsNullOrEmpty(errorMessage))
                {
                    break;
                }
                Console.WriteLine();
                Console.WriteLine(errorMessage + ", попробуйте ввести степень длины шага еще раз\n");
            } while (true);

            stepLength = Math.Pow(stepLengthBase, stepLengthDegree);
        }
    }
}
