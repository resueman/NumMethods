using System;
using System.Collections.Generic;

namespace HighestAlgebraicDegreeOfAccuracyQuadratureFormulas
{
    public class LejandrePolynomialsBuilder
    {
        public static List<Func<double, double>> BuildFrom1ToN(int n)
        {
            if (n < 0)
            {
                throw new Exception();
            }

            var LejandrePolynomials = new List<Func<double, double>> { x => 1, x => x };
            for (var i = 2; i <= n; ++i)
            {
                var k = i;
                var prev = LejandrePolynomials[k - 1];
                var prev_prev = LejandrePolynomials[k - 2];
                double lejandre_k(double x) => (2.0 * k - 1.0) / k * x * prev(x) - (k - 1.0) / k * prev_prev(x);
                LejandrePolynomials.Add(lejandre_k);
            }

            return LejandrePolynomials;
        }
    }
}
