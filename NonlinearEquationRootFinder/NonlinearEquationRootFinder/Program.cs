using System;

namespace NonlinearEquationRootFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            static double function(double x) => Math.Pow(2, -x) - Math.Sin(x);
            static double derivative1(double x) => -Math.Cos(x) - Math.Log(2, Math.E) / Math.Pow(2, x);
            static double derivative2(double x) => Math.Sin(x) - Math.Pow(Math.Log(2, Math.E), 2) / Math.Pow(2, x);
            var rootFinder = new RootFinder(function, derivative1, derivative2, new Segment(-5, 10), 0.000001, 100);
            rootFinder.FindRoots();
        }
    }
}
