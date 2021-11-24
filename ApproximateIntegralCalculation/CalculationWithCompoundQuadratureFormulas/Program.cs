using Common;
using System;
using System.Collections.Generic;

namespace CalculationWithCompoundQuadratureFormulas
{
    public class Program
    {
        private int m; // amount of intervals
        private double h; // partition size
        private double sumW; // SUM(f_j(y_j + h /2)), j from 0 to m - 1
        private double sumQ; // SUM(f_j(y_j)), j from 1 to m -1
        private double sumZ; // f_0 + f_m

        public Func<double, double> WeightFunction { get; private set; }

        public Function IntegrableFunction { get; private set; }

        // COMPOUND QUADRATURE FORMULAS
        public CompoundQuadratureFormula LeftRectangles { get; private set; }

        public CompoundQuadratureFormula RightRectangles { get; private set; }

        public CompoundQuadratureFormula MiddleRectangles { get; private set; }

        public CompoundQuadratureFormula Trapezium { get; private set; }

        public CompoundQuadratureFormula Simpson { get; private set; }

        public Program()
        {
            WeightFunction = x => 1;

            IntegrableFunction = new Function(
                "e^(3 * x)",
                x => Math.Pow(Math.E, 3 * x),
                y => Math.Pow(Math.E, 3 * y) / 3);
        }

        public void Start()
        {
            Console.WriteLine("Приближенное вычисление интеграла по составным квадратурным формуллам\n");

            var integrableFunction = IntegrableFunction;
            Console.WriteLine($"Интегрируемая функция: {integrableFunction.StringRepresentation}\n");

            while (true)
            {
                var segment = new Segment();
                ReadM();
                h = (segment.Right - segment.Left) / m;

                var expected = integrableFunction.CountIntegral(segment);
                Console.WriteLine($"Точное значение интеграла: {expected}\n");

                var simpleQuadratureFormulas = CreateCompoundQuadratureFormulas(integrableFunction, segment);
                foreach (var simpleQuadratureFormula in simpleQuadratureFormulas)
                {
                    var (actual, absoluteActualError) = simpleQuadratureFormula.CalculateIntegral(integrableFunction, segment);
                    Console.WriteLine(simpleQuadratureFormula.Name);
                    Console.WriteLine($"Полученное значение: {actual}");
                    Console.WriteLine($"|J - J(h)| = {absoluteActualError}");
                    Console.WriteLine($"|R_m(f)|  <= {simpleQuadratureFormula.TheoreticalError()}");
                    Console.WriteLine();
                }
                Console.WriteLine("-----------------------------------------------\n");
            }
        }

        private List<CompoundQuadratureFormula> CreateCompoundQuadratureFormulas(Function f, Segment s)
        {
            var f_0 = f.Func(s.Left);
            var f_m = f.Func(s.Right);
            sumZ = f_0 + f_m;

            sumQ = f.Func(s.Left + h / 2);
            sumW = 0;
            for (var j = 1; j < m; ++j)
            {
                var y_j = s.Left + j * h;
                sumQ += f.Func(y_j + (h / 2));
                sumW += f.Func(y_j);
            }

            var diff = s.Right - s.Left;
            var M1 = 3* IntegrableFunction.Func(s.Right);
            var M2 = 9 * IntegrableFunction.Func(s.Right);
            var M4 = 81 * IntegrableFunction.Func(s.Right);

            LeftRectangles = new CompoundQuadratureFormula(
                () => h * (f_0 + sumW), 
                "СКФ левых прямоугольников", 
                () => M1 * Math.Pow(diff, 2) / (2 * m));
            
            RightRectangles = new CompoundQuadratureFormula(
                () => h * (sumW + f_m), 
                "СКФ правых прямоугольников", 
                () => M1 * Math.Pow(diff, 2) / (2 * m));

            MiddleRectangles = new CompoundQuadratureFormula( 
                () => h * sumQ, 
                "СКФ средних прямоугольников",
                () => M2 * Math.Pow(diff, 3) / (24 * Math.Pow(m, 2)));
            
            Trapezium = new CompoundQuadratureFormula(
                () => h / 2 * (sumZ + 2 * sumW),
                "СКФ трапеций",
                () => M2 * Math.Pow(diff, 3) / (12 * Math.Pow(m, 2)));
            
            Simpson = new CompoundQuadratureFormula(
                () => h / 6 * (sumZ + 2 * sumW + 4 * sumQ),
                "СКФ Симпсона",
                () => M4 * Math.Pow(diff, 5) / (2880 * Math.Pow(m, 4)));

            var simpleQuadratureFormulas = new List<CompoundQuadratureFormula>
            {
                LeftRectangles,
                RightRectangles,
                MiddleRectangles,
                Trapezium,
                Simpson
            };

            return simpleQuadratureFormulas;
        }

        public void ReadM()
        {
            do
            {
                Console.Write("Введите m -- число промежутков деления заданого отрезка: ");
                var isAnInteger = int.TryParse(Console.ReadLine(), out m);
                var errorMessage = !isAnInteger
                    ? "m должно быть целым числом"
                    : m <= 0
                        ? "m должно быть больше нуля"
                        : "";

                if (string.IsNullOrEmpty(errorMessage))
                {
                    break;
                }
                Console.WriteLine(errorMessage + ", попробуйте ввести m еще раз\n");
            } while (true);
            Console.WriteLine();
        }
    }
}
