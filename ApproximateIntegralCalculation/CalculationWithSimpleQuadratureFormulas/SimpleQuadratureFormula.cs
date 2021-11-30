using Common;
using System;

namespace CalculationWithSimpleQuadratureFormulas
{
    public class SimpleQuadratureFormula
    {
        public SimpleQuadratureFormula(Func<Func<double, double>, Segment, double> formula, string name, string errorEstimation)
        {
            this.formula = formula;
            Name = name;
            ErrorEstimation = errorEstimation;
        }

        private Func<Func<double, double>, Segment, double> formula;

        public string Name { get; private set; }
        
        public string ErrorEstimation { get; private set; }

        public (double Actual, double AbsoluteActualError) CalculateIntegral(Function function, Segment segment)
        {
            var actual = formula(function.Func, segment);
            var absoluteActualError = Math.Abs(actual - function.CountIntegral(segment));
            return (actual, absoluteActualError);
        }
    }
}
