using System;
using CalculationWithCompoundQuadratureFormulas;
using CalculationWithSimpleQuadratureFormulas;

namespace ApproximateIntegralCalculation
{
    class Program
    {
        static void Main()
        {
            //var program = new CalculationWithSimpleQuadratureFormulas.Program(); // 4.1
            //var program = new CalculationWithCompoundQuadratureFormulas.Program(true); // 4.2
            var program = new CalculationWithCompoundQuadratureFormulas.Program(false); // 4.3
            program.Start();
        }
    }
}
