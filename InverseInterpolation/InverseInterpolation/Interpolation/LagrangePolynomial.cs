using System.Collections.Generic;
using System.Linq;

namespace InverseInterpolation.Interpolation
{
    class LagrangePolynomial
    {
        private List<KeyValuePair<double, double>> sortedTable;

        public LagrangePolynomial(List<KeyValuePair<double, double>> sortedTable)
        {
            this.sortedTable = sortedTable;
        }

        public double GetValue(double x)
        {
            var value = 0.0;
            for (var k = 0; k < sortedTable.Count; ++k)
            {
                double coefficient = 1;
                for (var i = 0; i < sortedTable.Count; ++i)
                {
                    if (i != k)
                    {
                        coefficient *= (x - sortedTable[i].Key) / (sortedTable[k].Key - sortedTable[i].Key);
                    }
                }
                value += coefficient * sortedTable[k].Value;
            }
            return value;
        }
    }
}
