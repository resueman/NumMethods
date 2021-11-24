using Common;
using NUnit.Framework;
using System;
using CalculationWithSimpleQuadratureFormulas;

namespace SimpleQuadratureFormulasTests
{
    public class Tests
    {
        private Program program;
        private Segment segment;
        private double precisious = Math.Pow(10, -63);

        [SetUp]
        public void Setup()
        {
            program = new Program();
            segment = new Segment(-3, 9);
        }

        [Test]
        public void LeftRectangeFormulaShouldHaveZeroAlgebraicPrecisionTest()
        {
            var (_, absoluteActualError) = program.LeftRectangle.CalculateIntegral(program.ZeroDegreePolynomial, segment);
            Assert.Less(absoluteActualError, precisious);
        }

        [Test]
        public void RightRectangeFormulaShouldHaveZeroAlgebraicPrecisionTest()
        {
            var (_, absoluteActualError) = program.RightRectangle.CalculateIntegral(program.ZeroDegreePolynomial, segment);
            Assert.Less(absoluteActualError, precisious);
        }

        [Test]
        public void MiddleRectangeFormulaShouldHaveFirstAlgebraicPrecisionTest()
        {
            var result = program.MiddleRectangle.CalculateIntegral(program.ZeroDegreePolynomial, segment);
            Assert.Less(result.AbsoluteActualError, precisious);

            result = program.MiddleRectangle.CalculateIntegral(program.FirstDegreePolynomial, segment);
            Assert.Less(result.AbsoluteActualError, precisious);
        }

        [Test]
        public void TrapeziumFormulaShouldHaveFirstAlgebraicPrecisionTest()
        {
            var result = program.Trapezium.CalculateIntegral(program.ZeroDegreePolynomial, segment);
            Assert.Less(result.AbsoluteActualError, precisious);

            result = program.Trapezium.CalculateIntegral(program.FirstDegreePolynomial, segment);
            Assert.Less(result.AbsoluteActualError, precisious);
        }

        [Test]
        public void SimpsonFormulaShouldHaveThirdAlgebraicPrecisionTest()
        {
            var result = program.Simpson.CalculateIntegral(program.ZeroDegreePolynomial, segment);
            Assert.Less(result.AbsoluteActualError, precisious);

            result = program.Simpson.CalculateIntegral(program.FirstDegreePolynomial, segment);
            Assert.Less(result.AbsoluteActualError, precisious);

            result = program.Simpson.CalculateIntegral(program.SecondDegreePolynomial, segment);
            Assert.Less(result.AbsoluteActualError, precisious);

            result = program.Simpson.CalculateIntegral(program.ThirdDegreePolynomial, segment);
            Assert.Less(result.AbsoluteActualError, precisious);
        }

        [Test]
        public void ThreeEightsFormulaShouldHaveThirdAlgebraicPrecisionTest()
        {
            var result = program.ThreeEighths.CalculateIntegral(program.ZeroDegreePolynomial, segment);
            Assert.Less(result.AbsoluteActualError, precisious);

            result = program.ThreeEighths.CalculateIntegral(program.FirstDegreePolynomial, segment);
            Assert.Less(result.AbsoluteActualError, precisious);

            result = program.ThreeEighths.CalculateIntegral(program.SecondDegreePolynomial, segment);
            Assert.Less(result.AbsoluteActualError, precisious);

            result = program.ThreeEighths.CalculateIntegral(program.ThirdDegreePolynomial, segment);
            Assert.Less(result.AbsoluteActualError, precisious);
        }
    }
}