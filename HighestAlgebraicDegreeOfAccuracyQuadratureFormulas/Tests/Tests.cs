using HighestAlgebraicDegreeOfAccuracyQuadratureFormulas;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using NonlinearEquationRootFinder;
using System.Linq;
using HighestAlgebraicDegreeOfAccuracyQuadratureFormulas.Common;

namespace Tests
{
    public class AlgebraicDegreeoFAccuracyForGaussFormulaTests
    {
        private List<Func<double, double>> LejandrePolynomials;
        private List<GaussQuadratureFormula> GaussQuadratureFormulas;
        private double precision = Math.Pow(10, -13);

        [OneTimeSetUp]
        public void Setup()
        {
            LejandrePolynomials = new List<Func<double, double>>();
            GaussQuadratureFormulas = new List<GaussQuadratureFormula>();

            LejandrePolynomials = LejandrePolynomialsBuilder.BuildFrom1ToN(5);
            GaussQuadratureFormulas = new List<GaussQuadratureFormula>();
            for (var k = 1; k <= 5; ++k)
            {
                var lejandrePolynomial = LejandrePolynomials[k];
                var rootFinder = new RootFinder(lejandrePolynomial, new Segment(-1, 1), Math.Pow(10, -15), 10000);
                var lejandreRoots = rootFinder.FindRoots().Select(ra => ra.Root).ToList();

                var GaussQF = new GaussQuadratureFormula(k, lejandreRoots, LejandrePolynomials);
                GaussQuadratureFormulas.Add(GaussQF);
            }
        }

        private void CheckAccuracy(GaussQuadratureFormula formula, Function function)
        {
            var actual = formula.CalculateIntegral(function);
            var expected = function.CountIntegral(new Segment(-1, 1));
            Assert.Less(Math.Abs(actual - expected), precision);
        }

        [Test]
        public void GaussWithThreeNodesShouldBeHADAQuadratureFormulaForLessSixthPolynomials()
        {
            var formula = GaussQuadratureFormulas.First(f => f.N == 3);
            var functions = new List<Function>
            {
                Functions.ZeroDegreeFunction,
                Functions.FirstDegreePolynomial,
                Functions.SecondDegreePolynomial,
                Functions.ThirdDegreePolynomial,
                Functions.FourthDegreeFunction,
                Functions.FivthDegreeFunction,
            };

            foreach (var function in functions)
            {
                CheckAccuracy(formula, function);
            }
        }

        [Test]
        public void GaussWithFourNodesShouldBeHADAQuadratureFormulaForLessEighthPolynomials()
        {
            var formula = GaussQuadratureFormulas.First(f => f.N == 4);
            var functions = new List<Function>
            {
                Functions.ZeroDegreeFunction,
                Functions.FirstDegreePolynomial,
                Functions.SecondDegreePolynomial,
                Functions.ThirdDegreePolynomial,
                Functions.FourthDegreeFunction,
                Functions.FivthDegreeFunction,
                Functions.SixthDegreeFunction,
                Functions.SeventhDegreeFunction
            };

            foreach (var function in functions)
            {
                CheckAccuracy(formula, function);
            }
        }

        [Test]
        public void GaussWithFiveNodesShouldBeHADAQuadratureFormulaForLessTenthPolynomials()
        {
            var formula = GaussQuadratureFormulas.First(f => f.N == 5);
            var functions = new List<Function>
            {
                Functions.ZeroDegreeFunction,
                Functions.FirstDegreePolynomial,
                Functions.SecondDegreePolynomial,
                Functions.ThirdDegreePolynomial,
                Functions.FourthDegreeFunction,
                Functions.FivthDegreeFunction,
                Functions.SixthDegreeFunction,
                Functions.SeventhDegreeFunction,
                Functions.EighthDegreeFunction,
                Functions.NinethDegreeFunction,
            };

            foreach (var function in functions)
            {
                CheckAccuracy(formula, function);
            }
        }
    }
}