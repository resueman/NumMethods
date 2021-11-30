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
        public Func<double, double> wightFunction = x => 1;

        public Function IntegrableFunction { get; private set; }

        public Function ZeroDegreePolynomial { get; private set; }

        public Function FirstDegreePolynomial { get; private set; }

        public Function SecondDegreePolynomial { get; private set; }

        public Function ThirdDegreePolynomial { get; private set; }

        public Program()
        {
            IntegrableFunction = new Function(
                "1 * 2 * x * sin(3 * x) + e^(-3 * x)",
                x => 2 * x * Math.Sin(3 * x) + Math.Pow(Math.E, -3 * x),
                y => (2 / 9) * (Math.Sin(3 * y) - 3 * y * Math.Cos(3 * y)) - Math.Pow(Math.E, -3 * y) / 3);

            ZeroDegreePolynomial = new Function(
                "1* 31",
                x => 31,
                y => 31 * y);

            FirstDegreePolynomial = new Function(
                "1 * 8 * x + 39",
                x => 8 * x + 39,
                y => 4 * Math.Pow(y, 2) + 39 * y);

            SecondDegreePolynomial = new Function(
                "1 * 27 * x ^ 2 - 8 * x + 33",
                x => 27 * Math.Pow(x, 2) - 8 * x + 33,
                y => 9 * Math.Pow(y, 3) - 4 * Math.Pow(y, 2) + 33 * y);

            ThirdDegreePolynomial = new Function(
                "1 * 16 * x ^ 3 - 9 * x ^ 2 + 42 * x - 99",
                x => 16 * Math.Pow(x, 3) - 9 * Math.Pow(x, 2) + 42 * x - 99,
                y => 4 * Math.Pow(y, 4) - 3 * Math.Pow(y, 3) + 21 * Math.Pow(y, 2) - 99 * y);

            LeftRectangle = new SimpleQuadratureFormula(
                (f, s) => (s.Right - s.Left) * f(s.Left),
                "КФ левого прямоугольника",
                "R_l = f`(x) / 2 * (b -a) ^ 2");
           
            RightRectangle = new SimpleQuadratureFormula(
                (f, s) => (s.Right - s.Left) * f(s.Right),
                "КФ правого прямоугольника",
                "R_r = - f`(x) * ((b - a) ^ 2) / 2");

            MiddleRectangle = new SimpleQuadratureFormula(
                (f, s) => (s.Right - s.Left) * f((s.Left + s.Right) / 2),
                "КФ среднего прямоугольника",
                "R_mid = f``(x) / 24 * (b - a) ^ 3");

            Trapezium = new SimpleQuadratureFormula(
                (f, s) => (s.Right - s.Left) / 2 * (f(s.Left) + f(s.Right)), 
                "КФ трапеции",
                "R_tr = - f``(x) / 12 * (b - a) ^ 3");
            
            Simpson = new SimpleQuadratureFormula(
                (f, s) => (s.Right - s.Left) / 6  * (f(s.Left) + 4 * f((s.Left + s.Right) / 2) + f(s.Right)), 
                "КФ Симпсона",
                "R_simp =  - f````(x) / 2880 * (b - a) ^ 5");

            Func<Func<double, double>, Segment, double> func = (f, s) => (s.Right - s.Left) * ((f(s.Left) + 3 * f((s.Right - s.Left) / 3 + s.Left) + 3 * f(2 * (s.Right - s.Left) / 3 + s.Left) + f(s.Right)) / 8);
            ThreeEighths = new SimpleQuadratureFormula(
                func, 
                "КФ 3/8",
                "");
        }

        public void Start()
        {
            Console.WriteLine("Приближенное вычисление интеграла по простейшим квадратурным формуллам\n");
            var simpleQuadratureFormulas = new List<SimpleQuadratureFormula>
                { LeftRectangle, RightRectangle, MiddleRectangle, Trapezium, Simpson, ThreeEighths };

            var integrableFunction = IntegrableFunction;
            Console.WriteLine($"Интегрируемая функция: {integrableFunction.StringRepresentation}\n");

            foreach (var qf in simpleQuadratureFormulas)
            {
                Console.WriteLine(qf.ErrorEstimation);
            }

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
