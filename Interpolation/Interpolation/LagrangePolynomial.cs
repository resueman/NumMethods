using System;
using System.Collections.Generic;
using System.Linq;

namespace Interpolation
{
    class LagrangePolynomial : IPolynomial
    {
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
            var value = 0.0;
            for (var k = 0; k < SortedTable.Count; ++k)
            {
                var coefficient = 1.0;
                for (var i = 0; i < SortedTable.Count; ++i)
                {
                    if (i == k)
                    {
                        continue;
                    }
                    coefficient *= (x - SortedTable[i].Key) / (SortedTable[k].Key - SortedTable[i].Key);
                }
                value += SortedTable[k].Value;
            }
            return value;
        }
    }
}
