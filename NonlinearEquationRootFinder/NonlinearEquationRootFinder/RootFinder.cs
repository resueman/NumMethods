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
        private readonly int separationStepCount;
        private readonly double separationStepLength;

        private const double NewtonsMethodMaxIterations = 50;
        private const double modifiedNewtonsMethodMaxIterations = 50;
        private const double secantMethodMaxIterations = 50;

        public RootFinder(Func<double, double> function, 
            Func<double, double> derivative1, Func<double, double> derivative2,
            Segment segment, double precision, int separationStepCount)
        {
            this.function = function;
            this.derivative1 = derivative1;
            this.derivative2 = derivative2;
            rootsFindingSegment = segment;
            this.precision = precision;
            this.separationStepCount = separationStepCount;
            separationStepLength = (segment.Right - segment.Left) / separationStepCount;
        }

        public void FindRoots()
        {
            var equation = "2^(-x) - sin(x)";
            Console.WriteLine($"ЧИСЛЕННЫЕ МЕТОДЫ РЕШЕНИЯ НЕЛИНЕЙНЫХ УРАВНЕНИЙ\n");
            Console.WriteLine($"ИСХОДНЫЕ ПАРАМЕТРЫ:");
            Console.WriteLine($"Функция: {equation}");
            Console.WriteLine($"Отрезок поиска решений: [{rootsFindingSegment.Left}; {rootsFindingSegment.Right}]");
            Console.WriteLine($"Точность: {precision}\n");
            Console.WriteLine($"Число отрезков на которые мы делим исходный: {separationStepCount}");
            
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

            var newtonsAnalytics = ClarifyRootsUsingNewtonsMethod(segments);
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine($"РЕЗУЛЬТАТ УТОЧНЕНИЯ КОРНЕЙ МЕТОДОМ НЬЮТОНА:\n");
            Console.WriteLine("--------------------------------------------");
            PrintResults(newtonsAnalytics);

            var modifiedNewtonsAnalytics = ClarifyRootsUsingModifiedNewtonsMethod(segments);
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

        private void PrintResults(List<RootClarifierMethodAnalytics> analytics)
        {
            var counter = 1;
            foreach (var result in analytics)
            {
                Console.WriteLine($"Отрезок {counter}: [{result.Segment.Left.ToFormattedString(3)}; {result.Segment.Right.ToFormattedString(3)}]");
                Console.WriteLine($"Начальное приближение: {result.InitialApproximationToTheRoot[0].ToFormattedString()}");
                Console.WriteLine($"Число шагов N: {result.StepsCounts}");
                Console.WriteLine($"Приближенное решение x_N: {result.FinalApproximationToTheRoot.ToFormattedString()}");
                Console.WriteLine($"|x_N - x_(N-1)|: {result.LengthOfLastApproximationSegment.ToFormattedString(10)}");
                Console.WriteLine($"Абсолютная величина невязки: {result.DiscrepancyAbsoluteValue.ToFormattedString(17)}");
                Console.WriteLine();
                ++counter;
            }
        }

        private List<Segment> SeparateRoots()
        {
            var rootContainingSegments = new List<Segment>();
            for (var left = rootsFindingSegment.Left; left < rootsFindingSegment.Right; left += separationStepLength)
            {
                var right = Math.Min(left + separationStepLength, rootsFindingSegment.Right);
                if (function(left) * function(right) <= 0)
                {
                    rootContainingSegments.Add(new Segment(left, right));
                }
            }
            return rootContainingSegments;
        }

        private List<RootClarifierMethodAnalytics> ClarifyRootsUsingBisection(List<Segment> segments)
        {
            var rootAnalytics = new List<RootClarifierMethodAnalytics>();
            foreach (var segment in segments)
            {
                var l = segment.Left;
                var r = segment.Right;
                var initialApproximationToTheRoot = (l + r) / 2;
                var stepsCount = 0;
                var center = initialApproximationToTheRoot;
                while (r - l > 2 * precision)
                {
                    if (Math.Abs(function(center)) < precision)
                    {
                        break;
                    }
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
                var disrepencyAbsoluteValue = Math.Abs(function(center));
                var analytics = new RootClarifierMethodAnalytics(segment, new List<double> { initialApproximationToTheRoot }, stepsCount, center, Math.Abs(r - l), disrepencyAbsoluteValue);
                rootAnalytics.Add(analytics);
            }
            return rootAnalytics;
        }

        private List<RootClarifierMethodAnalytics> ClarifyRootsUsingNewtonsMethod(List<Segment> segments)
        {
            var rootAnalytics = new List<RootClarifierMethodAnalytics>();
            foreach (var segment in segments)
            {
                var random = new Random();
                bool success;
                do
                {
                    var x_0 = segment.Left + random.NextDouble() * (segment.Right - segment.Left);
                    success = TryClarifyRootUsingNewtonsMethod(x_0, segment, out var analytics);
                    if (success) rootAnalytics.Add(analytics);
                } while (!success);
            }
            return rootAnalytics;
        }

        private bool TryClarifyRootUsingNewtonsMethod(double x_0, Segment segment, out RootClarifierMethodAnalytics analytics)
        {
            analytics = null;          
            if (function(x_0) * derivative2(x_0) <= 0)
            {
                return false;
            }

            var prev = x_0;
            var curr = prev - function(prev) / derivative1(prev);
            var stepsCount = 0;
            while (Math.Abs(curr - prev) > precision)
            {
                if (NewtonsMethodMaxIterations < stepsCount)
                {
                    return false;
                }
                try
                {
                    prev = curr;
                    curr = prev - function(prev) / derivative1(prev);
                    ++stepsCount;
                }
                catch (DivideByZeroException)
                {
                    return false;
                }
            }
            analytics = new RootClarifierMethodAnalytics(segment, new List<double> { x_0 }, stepsCount, curr, Math.Abs(curr - prev), Math.Abs(function(curr)));
            return true;
        }

        private List<RootClarifierMethodAnalytics> ClarifyRootsUsingModifiedNewtonsMethod(List<Segment> segments)
        {
            var rootAnalytics = new List<RootClarifierMethodAnalytics>();
            foreach (var segment in segments)
            {
                var random = new Random();
                bool success;
                do
                {
                    var x_0 = segment.Left + random.NextDouble() * (segment.Right - segment.Left);
                    success = TryClarifyRootUsingModifiedNewtonsMethod(x_0, segment, out var analytics);
                    if (success) rootAnalytics.Add(analytics);
                } while (!success);
            }
            return rootAnalytics;
        }

        private bool TryClarifyRootUsingModifiedNewtonsMethod(double x_0, Segment segment, out RootClarifierMethodAnalytics analytics)
        {
            analytics = null;
            if (function(x_0) * derivative2(x_0) <= 0)
            {
                return false;
            }

            var x_0_derivative = derivative1(x_0);
            var prev = x_0;
            var curr = prev - function(prev) / x_0_derivative;
            var stepsCount = 0;
            while (Math.Abs(curr - prev) > precision)
            {
                if (modifiedNewtonsMethodMaxIterations < stepsCount)
                {
                    return false;
                }
                try
                {
                    prev = curr;
                    curr = prev - function(prev) / x_0_derivative;
                    ++stepsCount;
                }
                catch (DivideByZeroException)
                {
                    return false;
                }
            }
            analytics = new RootClarifierMethodAnalytics(segment, new List<double> { x_0 }, stepsCount, curr, Math.Abs(curr - prev), Math.Abs(function(curr)));
            return true;
        }

        private List<RootClarifierMethodAnalytics> ClarifyRootsUsingSecantMethod(List<Segment> segments)
        {
            var rootAnalytics = new List<RootClarifierMethodAnalytics>();
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

        private bool TryClarifyRootUsingSecantMethod(double x_0, double x_1, Segment segment, out RootClarifierMethodAnalytics analytics)
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
                if (secantMethodMaxIterations < stepsCount)
                {
                    return false;
                }
                try
                {
                    prev = curr;
                    curr = next;
                    next = curr - function(curr) * (curr - prev) / (function(curr) - function(prev));
                    ++stepsCount;
                }
                catch (DivideByZeroException)
                {
                    return false;
                }
            }
            analytics = new RootClarifierMethodAnalytics(segment, new List<double> { x_0, x_1 }, stepsCount, next, Math.Abs(next - curr), Math.Abs(function(next)));
            return true;
        }
    }
}
