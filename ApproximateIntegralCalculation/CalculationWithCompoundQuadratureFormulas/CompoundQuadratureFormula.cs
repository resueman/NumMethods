using Common;
using System;

namespace CalculationWithCompoundQuadratureFormulas
{
    public class CompoundQuadratureFormula
    {
        public CompoundQuadratureFormula(Func<double> formula, string name, Func<double> theoreticalError, int algebraicPrecision)
        {
            CalculateIntegral = formula;
            CalculateTheoreticalError = theoreticalError;
            Name = name;
            AlgebraicPrecision = algebraicPrecision;
        }

        public string Name { get; private set; }

        public Func<double> CalculateIntegral { get; private set; }

        public Func<double> CalculateTheoreticalError { get; private set; }

        public int AlgebraicPrecision { get; private set; }
    }
}
