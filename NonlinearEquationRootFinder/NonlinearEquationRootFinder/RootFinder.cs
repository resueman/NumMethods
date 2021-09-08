using System;
using System.Collections.Generic;

namespace NonlinearEquationRootFinder
{
    class RootFinder
    {
        private readonly Func<double, double> function; 
        private readonly Func<double, double> derivative1; 
        private readonly Func<double, double> derivative2;

        private readonly Segment rootsFindingSegment;
        private readonly double precision;
        private readonly double clarificationStepLength;

        public RootFinder(Func<double, double> function, 
            Func<double, double> derivative1, Func<double, double> derivative2,
            Segment segment, double precision, int clarificationStepCount)
        {
            this.function = function;
            this.derivative1 = derivative1;
            this.derivative2 = derivative2;
            rootsFindingSegment = segment;
            this.precision = precision;
            clarificationStepLength = (segment.Right - segment.Left) / clarificationStepCount;
        }

        private void PrintResults(List<RootAnalytics> analytics)
        {
            var counter = 1;
            foreach (var result in analytics)
            {
                Console.WriteLine($"Отрезок {counter}: [{result.Segment.Left.ToFormattedString(3)}; {result.Segment.Right.ToFormattedString(3)}]");
                Console.WriteLine($"Начальное приближение: {result.InitialApproximationToTheRoot[0].ToFormattedString()}");
                Console.WriteLine($"Число шагов: {result.StepsCounts}");
                Console.WriteLine($"Приближенное решение: {result.FinalApproximationToTheRoot.ToFormattedString()}");
                Console.WriteLine($"|x_n - x_(n-1)|: {result.LengthOfLastApproximationSegment.ToFormattedString()}");
                Console.WriteLine($"Абсолютная величина невязки: {result.DiscrepancyAbsoluteValue.ToFormattedString()}");
                Console.WriteLine();
                ++counter;
            }
        }

        public void FindRoots()
        {
            var equation = "2^(-x) - sin(x)";
            Console.WriteLine($"ЧИСЛЕННЫЕ МЕТОДЫ РЕШЕНИЯ НЕЛИНЕЙНЫХ УРАВНЕНИЙ\n");
            Console.WriteLine($"ИСХОДНЫЕ ПАРАМЕТРЫ:");
            Console.WriteLine($"Функция: {equation}");
            Console.WriteLine($"Отрезок поиска решений: [{rootsFindingSegment.Left}; {rootsFindingSegment.Right}]");
            Console.WriteLine($"Точность: {precision}\n");
            
            Console.WriteLine($"РЕЗУЛЬТАТ ПРОЦЕДУРЫ ОТДЕЛЕНИЯ КОРНЕЙ:\n");
            var segments = SeparateRoots();
            Console.WriteLine($"Число промежутков перемены знака: {segments.Count}\n");
            Console.WriteLine($"Отрезки, принадлежащие [{rootsFindingSegment.Left}; {rootsFindingSegment.Right}], каждый из которых содержит единственный корень:");
            foreach (var segment in segments)
            {
                Console.WriteLine($"[{segment.Left}; {segment.Right}]");
            }
            Console.WriteLine();

            var bisectionAnalytics = ClarifyRootsUsingBisection(segments);
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine($"РЕЗУЛЬТАТ УТОЧНЕНИЯ КОРНЕЙ МЕТОДОМ БИСЕКЦИИ:\n");
            Console.WriteLine("---------------------------------------------");
            PrintResults(bisectionAnalytics);

            var newtonsAnalytics = ClarifyRootsUsingNewtonMethod(segments);
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine($"РЕЗУЛЬТАТ УТОЧНЕНИЯ КОРНЕЙ МЕТОДОМ НЬЮТОНА:\n");
            Console.WriteLine("--------------------------------------------");
            PrintResults(newtonsAnalytics);

            var modifiedNewtonsAnalytics = ClarifyRootsUsingModifiedNewtonMethod(segments);
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine($"РЕЗУЛЬТАТ УТОЧНЕНИЯ КОРНЕЙ МОДИФИЦИРОВАННЫМ МЕТОДОМ НЬЮТОНА:\n");
            Console.WriteLine("------------------------------------------------------------");
            PrintResults(modifiedNewtonsAnalytics);

            var secantMethodAnalytics = ClarifyRootsUsingSecantMethod(segments);
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine($"РЕЗУЛЬТАТ УТОЧНЕНИЯ КОРНЕЙ МЕТОДОМ СЕКУЩИХ:\n");
            Console.WriteLine("--------------------------------------------");
            PrintResults(secantMethodAnalytics);
        }

        private List<Segment> SeparateRoots()
        {
            var rootContainingSegments = new List<Segment>();
            for (var left = rootsFindingSegment.Left; left < rootsFindingSegment.Right; left += clarificationStepLength)
            {
                var right = Math.Min(left + clarificationStepLength, rootsFindingSegment.Right);
                if (function(left) * function(right) <= 0)
                {
                    rootContainingSegments.Add(new Segment(left, right));
                }
            }
            return rootContainingSegments;
        }

        private List<RootAnalytics> ClarifyRootsUsingBisection(List<Segment> segments)
        {
            var rootAnalytics = new List<RootAnalytics>();
            foreach (var segment in segments)
            {
                var l = segment.Left;
                var r = segment.Right;
                var initialApproximationToTheRoot = (l + r) / 2;
                var stepsCount = 0;
                var center = initialApproximationToTheRoot;
                while (Math.Abs(r - l) > 2 * precision)
                {
                    if (function(l) * function(center) <= 0)
                    {
                        r = center;
                    }
                    else
                    {
                        l = center;
                    }
                    center = (r + l) / 2;
                    ++stepsCount;
                }
                var root = (r + l) / 2;
                var disrepencyAbsoluteValue = Math.Abs(function(root));
                var analytics = new RootAnalytics(segment, new List<double> { initialApproximationToTheRoot }, stepsCount, root, Math.Abs(r - l), disrepencyAbsoluteValue);
                rootAnalytics.Add(analytics);
            }
            return rootAnalytics;
        }

        private List<RootAnalytics> ClarifyRootsUsingNewtonMethod(List<Segment> segments)
        {
            var rootAnalytics = new List<RootAnalytics>();
            foreach (var segment in segments)
            {
                var random = new Random();
                bool success;
                do
                {
                    var x_0 = segment.Left + random.NextDouble() * (segment.Right - segment.Left);
                    success = TryClarifyRootUsingNewtonMethod(x_0, segment, out var analytics);
                    if (success) rootAnalytics.Add(analytics);
                } while (!success);
            }
            return rootAnalytics;
        }

        private bool TryClarifyRootUsingNewtonMethod(double x_0, Segment segment, out RootAnalytics analytics)
        {
            analytics = null;          
            if (function(x_0) * derivative2(x_0) <= 0)
            {
                return false;
            }

            var prev = x_0;
            var curr = prev - function(prev) / derivative1(prev);
            var stepCount = 0;
            while (Math.Abs(curr - prev) > precision)
            {
                try
                {
                    prev = curr;
                    curr = prev - function(prev) / derivative1(prev);
                    ++stepCount;
                }
                catch (DivideByZeroException)
                {
                    return false;
                }
            }
            analytics = new RootAnalytics(segment, new List<double> { x_0 }, stepCount, curr, Math.Abs(curr - prev), Math.Abs(function(curr)));
            return true;
        }

        private List<RootAnalytics> ClarifyRootsUsingModifiedNewtonMethod(List<Segment> segments)
        {
            var rootAnalytics = new List<RootAnalytics>();
            foreach (var segment in segments)
            {
                var random = new Random();
                bool success;
                do
                {
                    var x_0 = segment.Left + random.NextDouble() * (segment.Right - segment.Left);
                    success = TryClarifyRootUsingModifiedNewtonMethod(x_0, segment, out var analytics);
                    if (success) rootAnalytics.Add(analytics);
                } while (!success);
            }
            return rootAnalytics;
        }

        private bool TryClarifyRootUsingModifiedNewtonMethod(double x_0, Segment segment, out RootAnalytics analytics)
        {
            analytics = null;
            if (function(x_0) * derivative2(x_0) <= 0)
            {
                return false;
            }

            var x_0_derivative = derivative1(x_0);
            if (x_0_derivative == 0)
            {
                return false;
            }

            var prev = x_0;
            var curr = prev - function(prev) / x_0_derivative;
            var stepsCount = 0;
            while (Math.Abs(curr - prev) > precision)
            {
                prev = curr;
                curr = prev - function(prev) / x_0_derivative;
                ++stepsCount;
            }
            analytics = new RootAnalytics(segment, new List<double> { x_0 }, stepsCount, curr, Math.Abs(curr - prev), Math.Abs(function(curr)));
            return true;
        }

        private List<RootAnalytics> ClarifyRootsUsingSecantMethod(List<Segment> segments)
        {
            var rootAnalytics = new List<RootAnalytics>();
            foreach (var segment in segments)
            {
                var random = new Random();
                bool success;
                do
                {
                    var x_0 = segment.Left + random.NextDouble() * (segment.Right - segment.Left);
                    var x_1 = segment.Left + random.NextDouble() * (segment.Right - segment.Left);
                    x_0 = Math.Min(x_0, x_1);
                    x_1 = Math.Max(x_0, x_1);
                    success = TryClarifyRootUsingSecantMethod(x_0, x_1, segment, out var analytics);
                    if (success) rootAnalytics.Add(analytics);
                } while (!success);
            }
            return rootAnalytics;
        }

        private bool TryClarifyRootUsingSecantMethod(double x_0, double x_1, Segment segment, out RootAnalytics analytics)
        {
            analytics = null;
            if (x_0 == x_1)
            {
                return false;
            }
            var prev = x_0;
            var curr = x_1;
            var next = curr - function(curr) * (curr - prev) / (function(curr) - function(prev));
            var stepsCount = 0;
            while (Math.Abs(curr - prev) > precision)
            {
                if (function(curr) - function(prev) == 0)
                {
                    return false;
                }
                prev = curr;
                curr = next;
                next = curr - function(curr) * (curr - prev) / (function(curr) - function(prev));
                ++stepsCount;
            }
            analytics = new RootAnalytics(segment, new List<double> { x_0, x_1 }, stepsCount, next, Math.Abs(next - curr), Math.Abs(function(next)));
            return true;
        }
    }
}
