using HighestAlgebraicDegreeOfAccuracyQuadratureFormulas.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public static class Functions
    {
        public static Function ZeroDegreeFunction = new(
            "21",
            x => 21,
            y => 21 * y);

        public static Function FirstDegreePolynomial = new(
            "8 * x + 39",
            x => 8 * x + 39,
            y => 4 * Math.Pow(y, 2) + 39 * y);

        public static Function SecondDegreePolynomial = new(
            "27 * x ^ 2 - 8 * x + 33",
            x => 27 * Math.Pow(x, 2) - 8 * x + 33,
            y => 9 * Math.Pow(y, 3) - 4 * Math.Pow(y, 2) + 33 * y);

        public static Function ThirdDegreePolynomial = new(
            "16 * x ^ 3 - 9 * x ^ 2 + 42 * x - 99",
            x => 16 * Math.Pow(x, 3) - 9 * Math.Pow(x, 2) + 42 * x - 99,
            y => 4 * Math.Pow(y, 4) - 3 * Math.Pow(y, 3) + 21 * Math.Pow(y, 2) - 99 * y);

        public static Function FourthDegreeFunction = new(
            "25 * x ^ 4 - 32 * x ^ 3 - 27 * x ^ 2 + 30 * x - 9",
            x => 25 * Math.Pow(x, 4) - 32 * Math.Pow(x, 3) - 27 * Math.Pow(x, 2) + 30 * x - 9,
            y => 5 * Math.Pow(y, 5) - 8 * Math.Pow(y, 4) - 9 * Math.Pow(y, 3) + 15 * Math.Pow(y, 2) - 9 * y);

        public static Function FivthDegreeFunction = new(
            "36 * x ^ 5 + 27 * x ^ 2 - 30 * x - 19",
            x => 36 * Math.Pow(x, 5) + 27 * Math.Pow(x, 2) - 30 * x - 19,
            y => 6 * Math.Pow(y, 6) + 9 * Math.Pow(y, 3) - 15 * Math.Pow(y, 2) - 19 * y);

        public static Function SixthDegreeFunction = new(
            "49 * x ^ 6 - 24 * x ^ 3 + 5",
            x => 49 * Math.Pow(x, 6) - 24 * Math.Pow(x, 3) + 5,
            y => 7 * Math.Pow(y, 7) - 6 * Math.Pow(y, 4) + 5 * y);

        public static Function SeventhDegreeFunction = new(
            "64 * x ^ 7 - 21 * x ^ 2 - 3",
            x => 64 * Math.Pow(x, 7) - 21 * Math.Pow(x, 2) - 3,
            y => 8 * Math.Pow(y, 8) - 7 * Math.Pow(y, 3) - 3 * y);

        public static Function EighthDegreeFunction = new(
            "27 * x ^ 8 - 64 * x ^ 7 - 21 * x ^ 2 - 3",
            x => 27 * Math.Pow(x, 8) - 64 * Math.Pow(x, 7) - 21 * Math.Pow(x, 2) - 3,
            y => 3 * Math.Pow(y, 9) - 8 * Math.Pow(y, 8) - 7 * Math.Pow(y, 3) - 3 * y);

        public static Function NinethDegreeFunction = new(
            "10 * x ^ 9 + 36 * x ^ 5 + 27 * x ^ 2 - 30 * x - 19",
            x => 10 * Math.Pow(x, 9) + 36 * Math.Pow(x, 5) + 27 * Math.Pow(x, 2) - 30 * x - 19,
            y => Math.Pow(y, 10) + 6 * Math.Pow(y, 6) + 9 * Math.Pow(y, 3) - 15 * Math.Pow(y, 2) - 19 * y);
    }
}
