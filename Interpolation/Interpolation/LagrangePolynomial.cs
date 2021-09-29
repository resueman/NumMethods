using System;
using System.Collections.Generic;
using System.Linq;

namespace Interpolation
{
    class LagrangePolynomial : IPolynomial
    {
        private double? value;

        public LagrangePolynomial(IEnumerable<KeyValuePair<double, double>> nearestSortedNodesValuesTable, Func<double, double> function)
        {
            SortedTable = nearestSortedNodesValuesTable.ToList();
            Function = function;
        }
        
        public List<KeyValuePair<double, double>> SortedTable { get; private set; }

        public Func<double, double> Function { get; private set; }

        public double GetActualInaccuracy(double x) => Math.Abs(Function(x) - GetValue(x));

        public double GetValue(double x)
        {
            if (value != null)
            {
                return (double)value;
            }

            var v = 0.0;
            for (var k = 0; k < SortedTable.Count; ++k)
            {
                double coefficient = 1;
                for (var i = 0; i < SortedTable.Count; ++i)
                {
                    if (i != k)
                    {
                        coefficient *= (x - SortedTable[i].Key) / (SortedTable[k].Key - SortedTable[i].Key);
                    }
                }
                v += coefficient * SortedTable[k].Value;
            }
            value = v;

            return (double)value;
        }
    }
}
