using System;

namespace NonlinearEquationRootFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            var rootFinder = new RootFinder(x => Math.Pow(2, -x) - Math.Sin(x), new Segment(-5, 10), 0.000001, 100);
            rootFinder.SeparateRoots();
            rootFinder.ClarifyRootsUsingBisection();
        }
    }
}
