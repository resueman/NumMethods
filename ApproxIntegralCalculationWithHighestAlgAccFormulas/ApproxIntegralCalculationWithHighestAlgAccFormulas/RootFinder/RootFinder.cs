using System;
using System.Collections.Generic;

namespace ApproxIntegralCalculationWithHighestAlgAccFormulas
{
    public class RootFinder
    {
        private readonly Func<double, double> function;

        private readonly Segment rootsFindingSegment;
        private readonly double precision;
        private readonly double separationStepLength;

        private const double secantMethodMaxIterations = 5000;

        public RootFinder(Func<double, double> function, Segment segment,
            double precision, int separationStepCount)
        {
            this.function = function;
            rootsFindingSegment = segment;
            this.precision = precision;
            separationStepLength = (segment.Right - segment.Left) / separationStepCount;
        }

        public List<RootClarifierMethodAnalytics> FindRoots()
        {
            var segments = SeparateRoots();
            return ClarifyRootsUsingSecantMethod(segments);
        }

        private List<Segment> SeparateRoots()
        {
            var rootContainingSegments = new List<Segment>();
            for (var left = rootsFindingSegment.Left; left < rootsFindingSegment.Right; left += separationStepLength)
            {
                var right = Math.Min(left + separationStepLength, rootsFindingSegment.Right);
                var f_l = function(left);
                var f_r = function(right);
                var mult = f_l * f_r;
                if (mult <= 0)
                {
                    rootContainingSegments.Add(new Segment(left, right));
                }
            }
            return rootContainingSegments;
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
            while (Math.Abs(next - curr) > precision)
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
