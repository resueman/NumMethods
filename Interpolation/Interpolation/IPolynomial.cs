using System;
using System.Collections.Generic;

namespace Interpolation
{
    public interface IPolynomial
    {
        List<KeyValuePair<double, double>> SortedTable { get; }

        Func<double, double> Function { get; }

        double GetValue(double x);

        double GetActualInaccuracy(double x);
    }
}