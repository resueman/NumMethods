using Common;
using System;
using System.Collections.Generic;

namespace CalculationWithSimpleQuadratureFormulas
{
    public class Program
    {
        // SIMPLE QUADRATURE FORMULAS
        public SimpleQuadratureFormula LeftRectangle { get; private set; }

        public SimpleQuadratureFormula RightRectangle { get; private set; }

        public SimpleQuadratureFormula MiddleRectangle { get; private set; }

        public SimpleQuadratureFormula Trapezium { get; private set; }

        public SimpleQuadratureFormula Simpson { get; private set; }

        public SimpleQuadratureFormula ThreeEighths { get; private set; }

        // INTEGRABLE FUNCTIONS
        public Function IntegrableFunction { get; private set; }

        public Function ZeroDegreePolynomial { get; private set; }

        public Function FirstDegreePolynomial { get; private set; }

        public Function SecondDegreePolynomial { get; private set; }

        public Function ThirdDegreePolynomial { get; private set; }

        public Program()
        {
            IntegrableFunction = new Function(
                "2 * x * sin(3 * x) + e^(-3 * x)",
                x => 2 * x * Math.Sin(3 * x) + Math.Pow(Math.E, -3 * x),
                y => (2 / 9) * (Math.Sin(3 * y) - 3 * y * Math.Cos(3 * y)) - Math.Pow(Math.E, -3 * y) / 3);

            ZeroDegreePolynomial = new Function(
                "31",
                x => 31,
                y => 31 * y);

            FirstDegreePolynomial = new Function(
                "8 * x + 39",
                x => 8 * x + 39,
                y => 4 * Math.Pow(y, 2) + 39 * y);

            SecondDegreePolynomial = new Function(
                "27 * x ^ 2 - 8 * x + 33",
                x => 27 * Math.Pow(x, 2) - 8 * x + 33,
                y => 9 * Math.Pow(y, 3) - 4 * Math.Pow(y, 2) + 33 * y);

            ThirdDegreePolynomial = new Function(
                "16 * x ^ 3 - 9 * x ^ 2 + 42 * x - 99",
                x => 16 * Math.Pow(x, 3) - 9 * Math.Pow(x, 2) + 42 * x - 99,
                y => 4 * Math.Pow(y, 4) - 3 * Math.Pow(y, 3) + 21 * Math.Pow(y, 2) - 99 * y);

            LeftRectangle = new SimpleQuadratureFormula((f, s) => (s.Right - s.Left) * f(s.Left), "КФ левого прямоугольника");
            RightRectangle = new SimpleQuadratureFormula((f, s) => (s.Right - s.Left) * f(s.Right), "КФ правого прямоугольника");
            MiddleRectangle = new SimpleQuadratureFormula((f, s) => (s.Right - s.Left) * f((s.Left + s.Right) / 2), "КФ среднего прямоугольника");
            Trapezium = new SimpleQuadratureFormula((f, s) => (s.Right - s.Left) / 2 * (f(s.Left) + f(s.Right)), "КФ трапеции");
            Simpson = new SimpleQuadratureFormula((f, s) => (s.Right - s.Left) / 6  * (f(s.Left) + 4 * f((s.Left + s.Right) / 2) + f(s.Right)), "КФ Симпсона");
            Func<Func<double, double>, Segment, double> func = (f, s) => (s.Right - s.Left) * ((f(s.Left) + 3 * f((s.Right - s.Left) / 3 + s.Left) + 3 * f(2 * (s.Right - s.Left) / 3 + s.Left) + f(s.Right)) / 8);
            ThreeEighths = new SimpleQuadratureFormula(func, "КФ 3/8");
        }

        public void Start()
        {
            Console.WriteLine("Приближенное вычисление интеграла по простейшим квадратурным формуллам\n");
            var simpleQuadratureFormulas = new List<SimpleQuadratureFormula>
                { LeftRectangle, RightRectangle, MiddleRectangle, Trapezium, Simpson, ThreeEighths };

            var integrableFunction = IntegrableFunction;
            Console.WriteLine($"Интегрируемая функция: {integrableFunction.StringRepresentation}\n");

            while (true)
            {
                var segment = new Segment();
                var expected = integrableFunction.CountIntegral(segment);
                Console.WriteLine($"Точное значение интеграла: {expected}\n");
                foreach (var simpleQuadratureFormula in simpleQuadratureFormulas)
                {
                    var (actual, absoluteActualError) = simpleQuadratureFormula.CalculateIntegral(integrableFunction, segment);
                    Console.WriteLine(simpleQuadratureFormula.Name);
                    Console.WriteLine($"Полученное значение: {actual}");
                    Console.WriteLine($"|J_e - J_a| = {absoluteActualError}");
                    Console.WriteLine();
                }
                Console.WriteLine("-----------------------------------------------\n");
            }
        }
    }
}
