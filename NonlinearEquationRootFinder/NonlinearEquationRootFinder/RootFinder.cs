using System;
using System.Collections.Generic;

namespace NonlinearEquationRootFinder
{
    class RootFinder
    {
        private Func<double, double> function; 
        private Func<double, double> derivative1; 
        private Func<double, double> derivative2;

        private Segment rootsFindingSegment;
        private double precision;
        private double clarificationStepLength;

        public List<Segment> RootContainingSegments { get; private set; }
        public List<double> Roots { get; private set; }

        public RootFinder(Func<double, double> func, 
            Func<double, double> derivative1, Func<double, double> derivative2,
            Segment segment, double precision, int clarificationStepCount)
        {
            function = func;
            this.derivative1 = derivative1;
            this.derivative2 = derivative2;
            rootsFindingSegment = segment;
            this.precision = precision;
            clarificationStepLength = (segment.Right - segment.Left) / clarificationStepCount;
            RootContainingSegments = new List<Segment>();
            Roots = new List<double>();
        }

        public void FindRoots()
        {
            SeparateRoots();
            ClarifyRootsUsingBisection();
            ClarifyRootsUsingNewtonMethod();
            ClarifyRootsUsingModifiedNewtonMethod();
            ClarifyRootsUsingSecantMethod();
        }

        private void SeparateRoots()
        {
            for (var left = rootsFindingSegment.Left; left < rootsFindingSegment.Right; left += clarificationStepLength)
            {
                var right = Math.Min(left + clarificationStepLength, rootsFindingSegment.Right);
                if (function(left) * function(right) <= 0)
                {
                    RootContainingSegments.Add(new Segment(left, right));
                }
            }
        }

        private void ClarifyRootsUsingBisection()
        {
            var roots = new List<double>();
            foreach (var segment in RootContainingSegments)
            {
                var l = segment.Left;
                var r = segment.Right;
                
                if (Math.Abs(function(l)) <= precision)
                {
                    roots.Add(l);
                    continue;
                }

                if (Math.Abs(function(r)) <= precision)
                {
                    roots.Add(r);
                    continue;
                }

                while (r - l > 2 * precision)
                {
                    var m = (r + l) / 2;
                    if (function(l) * function(m) <= 0)
                    {
                        r = m;
                        continue;
                    }
                    l = m;
                }
                var root = (r + l) / 2;
                roots.Add(root);
            }
        }

        private void ClarifyRootsUsingNewtonMethod()
        {
            var roots = new List<double>();
            foreach (var segment in RootContainingSegments)
            {
                var random = new Random();
                bool success;
                do
                {
                    var x_0 = segment.Left + random.NextDouble() * (segment.Right - segment.Left);
                    success = TryClarifyRootUsingNewtonMethod(x_0, out var root);
                    if (success) roots.Add(root);
                } while (!success);
            }
        }

        private bool TryClarifyRootUsingNewtonMethod(double x_0, out double root)
        {
            root = rootsFindingSegment.Left - 10;            
            if (function(x_0) * derivative2(x_0) > 0)
            {
                throw new Exception("Не выполнено условие сходимости");
            }

            var prev = x_0;
            var curr = prev - function(prev) / derivative1(prev);
            while (Math.Abs(curr - prev) > precision)
            {
                try
                {
                    var tmp = curr;
                    curr = prev - function(prev) / derivative1(prev);
                    prev = tmp;
                }
                catch (DivideByZeroException)
                {
                    return false;
                }
            }
            root = curr;
            return true;
        } 

        private void ClarifyRootsUsingModifiedNewtonMethod()
        {
            var roots = new List<double>();
            foreach (var segment in RootContainingSegments)
            {
                var random = new Random();
                bool success;
                do
                {
                    var x_0 = segment.Left + random.NextDouble() * (segment.Right - segment.Left);
                    success = TryClarifyRootUsingModifiedNewtonMethod(x_0, out var root);
                    if (success) roots.Add(root);
                } while (!success);
            }
        }

        private bool TryClarifyRootUsingModifiedNewtonMethod(double x_0, out double root)
        {
            root = rootsFindingSegment.Left - 10;
            if (function(x_0) * derivative2(x_0) > 0)
            {
                 throw new Exception("Не выполнено условие сходимости");
            }

            var x_0_derivative = derivative1(x_0);
            if (x_0_derivative == 0)
            {
                return false;
            }

            var prev = x_0;
            var curr = prev - function(prev) / x_0_derivative;
            while (Math.Abs(curr - prev) > precision)
            {
                var tmp = curr;
                curr = prev - function(prev) / x_0_derivative;
                prev = tmp;
            }
            root = curr;
            return true;
        }

        private void ClarifyRootsUsingSecantMethod()
        {
            var roots = new List<double>();
            foreach (var segment in RootContainingSegments)
            {
                var random = new Random();
                bool success;
                do
                {
                    var x_0 = segment.Left + random.NextDouble() * (segment.Right - segment.Left);
                    var x_1 = segment.Left + random.NextDouble() * (segment.Right - segment.Left);
                    x_0 = Math.Min(x_0, x_1);
                    x_1 = Math.Max(x_0, x_1);
                    success = TryClarifyRootUsingSecantMethod(x_0, x_1, out var root);
                    if (success) roots.Add(root);
                } while (!success);
            }
        }

        private bool TryClarifyRootUsingSecantMethod(double x_0, double x_1, out double root)
        {
            root = rootsFindingSegment.Left - 10;
            if (x_0 == x_1)
            {
                return false;
            }
            var prev = x_0;
            var curr = x_1;
            var next = curr - function(curr) * (curr - prev) / (function(curr) - function(prev));
            while (Math.Abs(curr - prev) > precision)
            {
                var tmpNext = next;
                if (function(curr) - function(prev) == 0)
                {
                    return false;
                }
                next = curr - function(curr) * (curr - prev) / (function(curr) - function(prev));
                prev = curr;
                curr = tmpNext;
            }
            root = next;
            return true;
        }
    }
}
