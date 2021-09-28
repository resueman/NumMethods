using System;
using System.Collections.Generic;
using System.Linq;

namespace Interpolation
{
    class NewtonsPolynomial : IPolynomial
    {
        private double? value;

        public NewtonsPolynomial(IEnumerable<KeyValuePair<double, double>> nearestSortedNodesValuesTable, Func<double, double> function)
        {
            SortedTable = nearestSortedNodesValuesTable.ToList();
            BuildTableOfDividedDifferences();
            Function = function;
        }

        public List<KeyValuePair<double, double>> SortedTable { get; private set; }

        public List<List<double>> DividedDifferencesTable { get; private set; }

        public Func<double, double> Function { get; private set; }

        public double GetActualInaccuracy(double x) => Math.Abs(Function(x) - GetValue(x));

        public double GetValue(double x)
        {
            if (value != null)
            {
                return (double)value;
            }

            var v = 0.0;
            for (var i = 0; i < SortedTable.Count; ++i)
            {
                var coefficient = 1.0;
                for (var j = 0; j < i; ++j)
                {
                    coefficient *= x - SortedTable[j].Key;
                }
                v += DividedDifferencesTable[0][i] * coefficient;
            }
            value = v;

            return (double)value;
        }

        private void BuildTableOfDividedDifferences()
        {
            DividedDifferencesTable = new List<List<double>>();
            for (var i = 0; i < SortedTable.Count; ++i)
            {
                DividedDifferencesTable.Add(new List<double>());
                for (var j = 0; i + j < SortedTable.Count; ++j)
                {
                    DividedDifferencesTable[i].Add(0);
                }
            }

            for (var i = 0; i < SortedTable.Count; ++i)
            {
                DividedDifferencesTable[i][0] = SortedTable[i].Value;
            }

            for (var j = 1; j < SortedTable.Count; ++j)
            {
                for (var i = 0; i + j < SortedTable.Count; ++i)
                {
                    DividedDifferencesTable[i][j] = (DividedDifferencesTable[i + 1][j - 1] - DividedDifferencesTable[i][j - 1]) /
                        (SortedTable[i + j].Key - SortedTable[i].Key);
                }
            }
        }

    }
}
