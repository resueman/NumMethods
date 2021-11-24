using Common;
using System;

namespace CalculationWithCompoundQuadratureFormulas
{
    public class CompoundQuadratureFormula
    {
        public CompoundQuadratureFormula(Func<double> formula, string name, Func<double> theoreticalError)
        {
            this.formula = formula;
            TheoreticalError = theoreticalError;
            Name = name;
        }

        private Func<double> formula;

        public string Name { get; private set; }

        public Func<double> TheoreticalError { get; private set; }

        public (double Actual, double AbsoluteActualError) CalculateIntegral(Function function, Segment segment)
        {
            var actual = formula();
            var absoluteActualError = Math.Abs(actual - function.CountIntegral(segment));
            return (actual, absoluteActualError);
        }
    }
}
