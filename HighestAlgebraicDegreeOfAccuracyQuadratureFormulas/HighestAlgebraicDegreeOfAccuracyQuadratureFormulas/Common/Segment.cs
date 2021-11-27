using System;

namespace HighestAlgebraicDegreeOfAccuracyQuadratureFormulas
{
    public class Segment
    {
        public double Left { get; private set; }

        public double Right { get; private set; }

        public Segment(double left, double right)
        {
            Left = Math.Min(left, right);
            Right = Math.Max(left, right);
        }

        public Segment()
        {
            Console.WriteLine("Введите границы отрезка интегрирования:");
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
            Console.WriteLine();

            Left = Math.Min(border1, border2);
            Right = Math.Max(border1, border2);
        }
    }
}
