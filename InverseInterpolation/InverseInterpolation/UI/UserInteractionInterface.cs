using System;

namespace InverseInterpolation.UI
{
    public class UserInteractionInterface
    {
        private bool firstParameterEntering = true;

        public UserChoice ProcessUserInput()
        {
            while (true)
            {
                var message = firstParameterEntering
                    ? "Хотите ли вы ввести параметры? Введите 'Да' или 'Нет'"
                    : "'П' -- нажмите, чтобы продолжить\n'Н' -- нажмите, чтобы ввести все параметры заново";

                Console.WriteLine(message);

                var userChoice = Console.ReadLine().ToLower();
                if (userChoice != "да" && userChoice != "нет" && userChoice != "п" && userChoice != "н")
                {
                    Console.WriteLine("Непонятно :)");
                    continue;
                }

                switch (userChoice)
                {
                    case "да":
                        firstParameterEntering = false;
                        return UserChoice.EnterAllParams;
                    case "нет":
                        return UserChoice.UseDefaultParams;
                    case "п":
                        return UserChoice.EnterSomeParams;
                    case "н":
                        return UserChoice.EnterAllParams;
                    default:
                        break;
                }
            }
        }

        public void ReadSegmentBorders(out Segment segment)
        {
            Console.WriteLine("\nВведите границы отрезка, на котором будут вычеслены точные значение функции:");
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

        public void ReadMaxInterpolationNodeNumber(out int maxNodeNumber)
        {
            do
            {
                Console.Write("Введите максимальный номер узла интерполирования при счете с 0: ");
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
            Console.WriteLine();
        }

        public void ReadPolynomialDegree(int maxNodeNumber, out int polynomialDegree)
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
            Console.WriteLine();
        }

        public void ReadPrecision(out double precision)
        {
            double precisionBase;
            double precisionDegree;
            do
            {
                Console.WriteLine("Ввод точности");
                Console.Write("Введите основание точности: ");
                var isADouble = double.TryParse(Console.ReadLine(), out precisionBase);
                var errorMessage = !isADouble
                    ? "Значение основания точности должно быть вещественным или целым числом"
                    : precisionBase <= 0
                        ? "Значение основания точности должно быть больше 0"
                        : "";

                if (string.IsNullOrEmpty(errorMessage))
                {
                    break;
                }
                Console.WriteLine();
                Console.WriteLine(errorMessage + ", попробуйте ввести основание точности еще раз\n");
            } while (true);

            do
            {
                Console.Write("Введите степень ε: ");
                var isADouble = double.TryParse(Console.ReadLine(), out precisionDegree);
                var errorMessage = !isADouble
                    ? "Значение степени точности должно быть вещественным или целым числом"
                    : "";

                if (string.IsNullOrEmpty(errorMessage))
                {
                    break;
                }
                Console.WriteLine();
                Console.WriteLine(errorMessage + ", попробуйте ввести степень точности еще раз\n");
            } while (true);
            Console.WriteLine();

            precision = Math.Pow(precisionBase, precisionDegree);
        }

        public void ReadPoint(out double point)
        {
            do
            {
                Console.Write("Введите значение функции F: ");
                var isADouble = double.TryParse(Console.ReadLine(), out point);
                var errorMessage = !isADouble ? "F должно быть вещественным числом" : "";

                if (string.IsNullOrEmpty(errorMessage))
                {
                    break;
                }
                Console.WriteLine(errorMessage + ", попробуйте ввести F еще раз\n");
            } while (true);
            Console.WriteLine();
        }
    }
}
