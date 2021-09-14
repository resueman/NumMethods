using System;
using System.Collections.Generic;
using System.Linq;

namespace Interpolation
{
    class NewtonsPolynomial : IPolynomial
    {
        public NewtonsPolynomial(IEnumerable<KeyValuePair<double, double>> nearestSortedNodesValuesTable, Func<double, double> function)
        {
            SortedTable = nearestSortedNodesValuesTable.ToList();
            Function = function;
        }

        public List<KeyValuePair<double, double>> SortedTable { get; private set; }

        public Func<double, double> Function { get; private set; }

        public double GetActualInaccuracy(double x) => Math.Abs(Function(x) - GetValue(x));

        public double GetValue(double x)
        {
            var coefficients = Get_A_i_Coefficients();
            var value = coefficients[0];
            for (var i = 1; i < coefficients.Count; ++i)
            {
                var coeficient = 1.0;
                for (var j = 0; j < i; ++j)
                {
                    coeficient *= x - SortedTable[j].Key;
                }
                value += coeficient * coefficients[i];
            }
            return value;
        }

        private List<double> Get_A_i_Coefficients()
        {
            var x_i_minus_1 = SortedTable[0].Key;
            var f_i_minus_1 = SortedTable[0].Value;
            var A_0 = SortedTable[0].Value;
            var coefficients = new List<double> { A_0 };
            for (var i = 1; i < SortedTable.Count; ++i)
            {
                var A_i = (SortedTable[i].Value - f_i_minus_1) / (SortedTable[i].Key - x_i_minus_1);
                coefficients.Add(A_i);
            }
            return coefficients;
        }
    }
}
