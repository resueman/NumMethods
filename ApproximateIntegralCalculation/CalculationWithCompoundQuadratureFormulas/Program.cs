using Common;
using System;
using System.Collections.Generic;

namespace CalculationWithCompoundQuadratureFormulas
{
    public class Program
    {
        private readonly bool run_4_2;
        private double sumW; // SUM(f_j(y_j + h /2)), j from 0 to m - 1
        private double sumQ; // SUM(f_j(y_j)), j from 1 to m -1
        private double sumZ; // f_0 + f_m

        public Func<double, double> WeightFunction { get; private set; }

        public Function IntegrableFunction { get; private set; }

        public Program(bool run_4_2)
        {
            this.run_4_2 = run_4_2;

            WeightFunction = x => 1;

            static double func(double x) => Math.Pow(Math.E, 3 * x);
            IntegrableFunction = new Function(
                "e^(3 * x)",
                func,
                y => Math.Pow(Math.E, 3 * y) / 3,
                m1_x => 3 * func(m1_x),
                m2_x => 9 * func(m2_x),
                m4_x => 81 * func(m4_x));
        }

        public void Start()
        {
            var integrableFunction = IntegrableFunction;

            while (true)
            {
                if (run_4_2)
                {
                    Console.WriteLine("Приближенное вычисление интеграла по составным квадратурным формулам\n");
                    Console.WriteLine($"Интегрируемая функция: {integrableFunction.StringRepresentation}\n");
                    Run_4_2(integrableFunction);
                }
                else
                {
                    Console.WriteLine("Приближенное вычисление интеграла по составным квадратурным формулам и формуле Рунге\n");
                    Console.WriteLine($"Интегрируемая функция: {integrableFunction.StringRepresentation}\n");
                    Run_4_3(integrableFunction);
                }
            }
        }

        public void Run_4_2(Function integrableFunction)
        {
            var segment = new Segment();
            var m = ReadM();

            var expected = integrableFunction.CountIntegral(segment);
            Console.WriteLine($"Точное значение интеграла: {expected}\n");

            var compoundQuadratureFormulas = CreateCompoundQuadratureFormulas(integrableFunction, segment, m);
            foreach (var compoundQuadratureFormula in compoundQuadratureFormulas)
            {
                var actual = compoundQuadratureFormula.CalculateIntegral();
                var absoluteActualError = Math.Abs(actual - expected);
                Console.WriteLine(compoundQuadratureFormula.Name);
                Console.WriteLine($"Полученное значение: {actual}");
                Console.WriteLine($"|J - J(h)| = {absoluteActualError}");
                Console.WriteLine($"|R_m(f)|  <= {compoundQuadratureFormula.CalculateTheoreticalError()}");
                Console.WriteLine();
            }
            Console.WriteLine("-----------------------------------------------\n");
        }

        public void Run_4_3(Function integrableFunction)
        {
            var segment = new Segment();
            var m = ReadM();

            var expected = integrableFunction.CountIntegral(segment);
            Console.WriteLine($"Точное значение интеграла: {expected}\n");

            var results = new Dictionary<string, List<(double value, double error)>>();
            var compoundQuadratureFormulas = CreateCompoundQuadratureFormulas(integrableFunction, segment, m);
            foreach (var compoundQuadratureFormula in compoundQuadratureFormulas)
            {
                var actual = compoundQuadratureFormula.CalculateIntegral();
                var absoluteActualError = Math.Abs(actual - expected);
                results.Add(compoundQuadratureFormula.Name, new List<(double value, double error)> { (actual, absoluteActualError) });
                
                //Console.WriteLine($"Полученное значение: {actual}");
                //Console.WriteLine($"|J - J(h)| = {absoluteActualError}");
                //Console.WriteLine();
            }

            // increase in l
            var l = ReadL();
            compoundQuadratureFormulas = CreateCompoundQuadratureFormulas(integrableFunction, segment, m * l);
            foreach (var compoundQuadratureFormula in compoundQuadratureFormulas)
            {
                var actual = compoundQuadratureFormula.CalculateIntegral();
                var absoluteActualError = Math.Abs(actual - expected);
                results[compoundQuadratureFormula.Name].Add((actual, absoluteActualError));

                //Console.WriteLine($"Полученное значение: {actual}");
                //Console.WriteLine($"|J - J(h)| = {absoluteActualError}");
                //Console.WriteLine();
            }

            var runges = new Dictionary<string, (double value, double error)>();
            foreach (var cqf in compoundQuadratureFormulas)
            {
                var res = results[cqf.Name];
                var J_m = res[0].value;
                var J_ml = res[1].value;
                var runge = CalculateRungesIntegral(J_m, J_ml, l, cqf.AlgebraicPrecision);
                var absoluteActualError = Math.Abs(runge - expected);
                runges.Add(cqf.Name, (runge, absoluteActualError));

                //Console.WriteLine($"Полученное значение: {runge}");
                //Console.WriteLine($"|J - J(h)| = {absoluteActualError}");
                //Console.WriteLine();
            }

            foreach (var compoundQuadratureFormula in compoundQuadratureFormulas)
            {
                var name = compoundQuadratureFormula.Name;
                var res = results[name];
                var J_m = res[0];
                var J_ml = res[1];
                var J_r = runges[name];

                name = name.Contains("прямоугольников") ? name.Replace("прямоугольников", "прямоуг-ов  ") : name.Insert(name.Length, "       ");
                Console.WriteLine("--------------------------------------------------------------------------------------------------------");
                Console.WriteLine(string.Format("{0,25}|{1,25}|{2,25}|{3,25}|", name, "J(h)          ", "J(h/l)         ", "J_runge         "));
                Console.WriteLine("--------------------------------------------------------------------------------------------------------");
                Console.WriteLine(string.Format("{0,25}|{1,25}|{2,25}|{3,25}|", "Полученное значение:     ", J_m.value, J_ml.value, J_r.value));
                Console.WriteLine("--------------------------------------------------------------------------------------------------------");
                Console.WriteLine(string.Format("{0,25}|{1,25}|{2,25}|{3,25}|", "|J_т - J_п|:             ", J_m.error, J_ml.error, J_r.error));
                Console.WriteLine("--------------------------------------------------------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        public static double CalculateRungesIntegral(double J_m, double J_ml, int l, int d) 
            => (Math.Pow(l, d + 1) * J_ml - J_m) / (Math.Pow(l, d + 1) - 1);

        private List<CompoundQuadratureFormula> CreateCompoundQuadratureFormulas(Function f, Segment s, int subSegmentsCount)
        {
            var h = (s.Right - s.Left) / subSegmentsCount;

            var f_0 = f.Func(s.Left);
            var f_m = f.Func(s.Right);
            sumZ = f_0 + f_m;

            sumQ = f.Func(s.Left + h / 2);
            sumW = 0;
            for (var j = 1; j < subSegmentsCount; ++j)
            {
                var y_j = s.Left + j * h;
                sumQ += f.Func(y_j + (h / 2));
                sumW += f.Func(y_j);
            }

            var diff = s.Right - s.Left;
            var M1 = IntegrableFunction.M1(s.Right);
            var M2 = IntegrableFunction.M1(s.Right);
            var M4 = IntegrableFunction.M1(s.Right);

            var leftRectangles = new CompoundQuadratureFormula(
                () => h * (f_0 + sumW), 
                "СКФ левых прямоугольников", 
                () => M1 * Math.Pow(diff, 2) / (2 * subSegmentsCount),
                0);
            
            var rightRectangles = new CompoundQuadratureFormula(
                () => h * (sumW + f_m), 
                "СКФ правых прямоугольников", 
                () => M1 * Math.Pow(diff, 2) / (2 * subSegmentsCount),
                0);

            var middleRectangles = new CompoundQuadratureFormula( 
                () => h * sumQ, 
                "СКФ средних прямоугольников",
                () => M2 * Math.Pow(diff, 3) / (24 * Math.Pow(subSegmentsCount, 2)),
                1);
            
            var trapezium = new CompoundQuadratureFormula(
                () => h / 2 * (sumZ + 2 * sumW),
                "СКФ трапеций",
                () => M2 * Math.Pow(diff, 3) / (12 * Math.Pow(subSegmentsCount, 2)),
                1);
            
            var simpson = new CompoundQuadratureFormula(
                () => h / 6 * (sumZ + 2 * sumW + 4 * sumQ),
                "СКФ Симпсона",
                () => M4 * Math.Pow(diff, 5) / (2880 * Math.Pow(subSegmentsCount, 4)), 3);

            var simpleQuadratureFormulas = new List<CompoundQuadratureFormula>
            {
                leftRectangles,
                rightRectangles,
                middleRectangles,
                trapezium,
                simpson
            };

            return simpleQuadratureFormulas;
        }

        public static int ReadM()
        {
            int m;
            do
            {
                Console.Write("Введите m -- число разбиений заданого отрезка интегрирования: ");
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

            return m;
        }

        public static int ReadL()
        {
            int l;
            do
            {
                Console.Write("Введите l -- число раз, в которые увеличится количество разбиений отрезка интегрирования: ");
                var isAnInteger = int.TryParse(Console.ReadLine(), out l);
                var errorMessage = !isAnInteger
                    ? "l должно быть целым числом"
                    : l <= 0
                        ? "l должно быть больше нуля"
                        : "";

                if (string.IsNullOrEmpty(errorMessage))
                {
                    break;
                }
                Console.WriteLine(errorMessage + ", попробуйте ввести l еще раз\n");
            } while (true);
            Console.WriteLine();

            return l;
        }
    }
}
