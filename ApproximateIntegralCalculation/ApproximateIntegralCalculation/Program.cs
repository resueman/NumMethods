using System;
using CalculationWithCompoundQuadratureFormulas;
using CalculationWithSimpleQuadratureFormulas;

namespace ApproximateIntegralCalculation
{
    class Program
    {
        static void Main(string[] args)
        {
            var program = new CalculationWithSimpleQuadratureFormulas.Program(); // 4.1
            //var program = new CalculationWithCompoundQuadratureFormulas.Program(); // 4.2
            program.Start();
        }
    }
}
