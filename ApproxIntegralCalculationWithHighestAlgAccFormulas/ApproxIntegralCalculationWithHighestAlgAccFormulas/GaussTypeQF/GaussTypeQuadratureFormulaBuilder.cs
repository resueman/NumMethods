using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApproxIntegralCalculationWithHighestAlgAccFormulas.GaussTypeQF
{
    class GaussTypeQuadratureFormulaBuilder
    {
        public static double CalculateIntegral(Segment segment, int N, Function function)
        {
            var moments = CalculateMoments(N, segment); // from 0 to 2 * n - 1 
            var orthogonalPolynomialCoefficients = CalculateOrthogonalPolynomialCoefficients(moments, N); // using moments
            Func<double, double> orthogonalPolynomial = BuildOrthogonalPolynomial(orthogonalPolynomialCoefficients, N); //using polynomial coefficients
            
            var rootFinder = new RootFinder(orthogonalPolynomial, segment, Math.Pow(10, -14), 10000);
            var orthogonalPolynomialRoots = rootFinder.FindRoots().Select(ra => ra.Root).ToList(); // finds orthogonal polynomial roots using secant method 
            var quadratureFormulaCoefficients = FindQuadratureFormulaCoefficients(orthogonalPolynomialRoots, moments, N); //using orthogonal polynomial roots and moments
            var integral = BuildGaussTypeQuadratureFormula(function, orthogonalPolynomialRoots, quadratureFormulaCoefficients); // using coefficients and root of ortogonal polynomial
            return integral;
        }

        private static List<double> CalculateMoments(int n, Segment segment)
        {
            var moments = new List<double>();
            for (var i = 0; i < 2 * n; ++i)
            {
                var k = i;
                var degree = 1 / 4 + k;
                var function = new Function(
                    $"x ^ (0.25 + {k})", 
                    x => Math.Pow(x, degree), 
                    y => Math.Pow(y, degree + 1) / (degree + 1));
                
                var moment = function.CountIntegral(segment);
                moments.Add(moment);
            }
            return moments;
        }

        private static List<double> CalculateOrthogonalPolynomialCoefficients(List<double> moments, int N)
        {
            var matrix = BuildLeftMomentsMatrixOfTheSystem(moments, N);
            var A = Matrix<double>.Build.DenseOfArray(matrix);
            var B = BuildRightMomentsMatrixOfTheSystem(moments, N);
            var coefficients = A.Solve(DenseVector.Build.DenseOfArray(B)).ToList();
            return coefficients;
        }

        private static double[,] BuildLeftMomentsMatrixOfTheSystem(List<double> moments, int N)
        {
            var matrix = new double[N, N];
            var lineNumber = 0;
            for (var i = N - 1; i < 2 * N - 1; ++i)
            {
                for (var j = 0; j < N; ++j)
                {
                    matrix[lineNumber, j] = moments[i - j];   
                }
                ++lineNumber;
            }
            return matrix;
        }

        private static double[] BuildRightMomentsMatrixOfTheSystem(List<double> moments, int N)
        {
            var matrix = new double[N];
            for (var i = 0; i < N; ++i)
            {
                matrix[i] = -moments[N + i];
            }
            return matrix;
        }

        private static Func<double, double> BuildOrthogonalPolynomial(List<double> coefficients, int N)
        {
            var addendums = new List<Func<double, double>> { x => Math.Pow(x, N) };
            var degree = N - 1;
            for (var i = 0; i < N; ++i)
            {
                var localI = i;
                var localDegree = degree;
                Func<double, double> addendum = x => coefficients[localI] * Math.Pow(x, localDegree);
                addendums.Add(addendum);
                --degree;
            }
            Func<double, double> orthogonalPolynomial = x => addendums.Sum(a => a(x));
            return orthogonalPolynomial;
        }

        private static List<double> FindQuadratureFormulaCoefficients(List<double> orthogonalPolynomialRoots, List<double> moments, int N)
        {
            var matrix = BuildLeftMatrixOfTheSystemForQfCoefficientsFinding(orthogonalPolynomialRoots, N);
            var A = Matrix<double>.Build.DenseOfArray(matrix);
            var B = BuildRightMatrixOfTheSystemForQfCoefficientsFinding(moments, N);
            var coefficients = A.Solve(DenseVector.Build.DenseOfArray(B)).ToList();
            return coefficients;
        }

        private static double[,] BuildLeftMatrixOfTheSystemForQfCoefficientsFinding(List<double> roots, int N)
        {
            var matrix = new double[N, N];
            for (var j = 0; j < N; ++j)
            {
                matrix[0, j] = 1;
            }

            for (var i = 1; i <= N - 1; ++i)
            {
                for (var j = 0; j < N; ++j)
                {
                    matrix[i, j] = Math.Pow(roots[j], i);
                }
            }

            return matrix;
        }

        private static double[] BuildRightMatrixOfTheSystemForQfCoefficientsFinding(List<double> moments, int N)
        {
            var matrix = new double[N];
            for (var i = 0; i < N; ++i)
            {
                matrix[i] = moments[i];
            }
            return matrix;
        }

        private static double BuildGaussTypeQuadratureFormula(Function function, 
            List<double> orthogonalPolynomialRoots, List<double> quadratureFormulaCoefficients)
        {
            var value = 0.0;
            var nodeCoefficientPairs = orthogonalPolynomialRoots
                .Zip(quadratureFormulaCoefficients)
                .ToList<(double x_k, double A_k)>();

            foreach (var (x_k, A_k) in nodeCoefficientPairs)
            {
                value += A_k * function.Func(x_k);
            }
            return value;
        }
    }
}
