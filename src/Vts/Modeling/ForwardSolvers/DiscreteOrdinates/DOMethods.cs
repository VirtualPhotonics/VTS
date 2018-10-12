using System;
using System.Collections.Generic;
using System.Linq;
using Meta.Numerics.Matrices;
using Vts.Extensions;

namespace Vts.Modeling.ForwardSolvers.DiscreteOrdinates
{
    public static class DOMethods
    {
        public static GaussLegendreCoefficients GaussLegendre(int N)
        {
            var beta = from i in 1.To(N - 1)
                       select 0.5 / Math.Sqrt(1.0 - (2.0 * i).Pow(-2));

            SymmetricMatrix T = new SymmetricMatrix(N); // how to use TridiagonalMatrix?
            beta.ForEach((b, i) =>
            {
                T[i + 1, i] = b;
                T[i, i + 1] = b;
            });

            var e = T.Eigendecomposition();

            var indices = from i in 0.To(N - 1)
                orderby e.Eigenpairs[i].Eigenvalue
                select i;

            return new GaussLegendreCoefficients
            {
                mu = indices.Select(i => e.Eigenpairs[i].Eigenvalue).ToArray(),
                wt = indices.Select(i => 2 * e.Eigenpairs[i].Eigenvector[0].Pow(2)).ToArray()
            };
        }

        public static SquareMatrix GenFokkerPlanckEddington(double[] f, double[] mu, double[] wt, int N)
        {
            var ones = Ones(N);
            var eye = Eye(N);

            // compute a mesh-grid of the directions

            SquareMatrix wtMatrix = new SquareMatrix(N);
            SquareMatrix UV = new SquareMatrix(N);

            0.To(N - 1).ForEach(i => wtMatrix[i, i] = wt[i]); // diag(wt)
            0.To(N - 1).ForEach(i => 0.To(N - 1).ForEach(j => UV[i, j] = mu[i] * mu[j]));

            // compute the coefficients

            var a0 = (7.0 * f[1] * f[3] - 5.0 * f[1] * f[2] - 2.0 * f[2] * f[3])
                     / (2.0 * f[1] - 7.0 * f[2] + 5.0 * f[3]);

            var a1 = 7.0 / 6.0 * (f[1] * f[1] * f[2] - f[1] * f[1] * f[3] - f[1] * f[2] * f[2]
                              + f[1] * f[3] * f[3] + f[2] * f[2] * f[3] - f[2] * f[3] * f[3])
                     / (2.0 * f[1] - 7.0 * f[2] + 5.0 * f[3]).Pow(2);

            var a2 = (7.0 * f[2] - 4.0 * f[1] - 3.0 * f[3]) / 12.0
                     / (2.0 * f[1] - 7.0 * f[2] + 5.0 * f[3]);

            var b0 = (5.0 * f[1] * f[2] - 7.0 * f[1] * f[3] + 2.0 * f[2] * f[3]
                      + 2.0 * f[1] - 7.0 * f[2] + 5.0 * f[3])
                     / (2.0 * f[1] - 7.0 * f[2] + 5.0 * f[3]);

            var b1 = (27.0 * f[1] * f[2] - 35.0 * f[1] * f[3] + 8.0 * f[2] * f[3]
                      + 8.0 * f[0] * f[1] - 35.0 * f[0] * f[2] + 27.0 * f[0] * f[3])
                     / (8.0 * f[1] - 35.0 * f[2] + 27.0 * f[3]);

            // compute the Laplace-Beltrami operator

            var A = LaplaceBelTrami(mu, wt, N);

            // compute the scattering operator

            var L = new SquareMatrix(N);
            L = L + 0.5 * b0 * ones + 1.5 * b1 * UV;
            L = L * wtMatrix;
            L = L + a1 * A * (eye - a2 * A).Inverse();
            L = L - (1.0 - a0) * eye;

            return L;
        }

        private static SquareMatrix Eye(int N)
        {
            var I = new SquareMatrix(N);
            0.To(N - 1).ForEach(i => I[i, i] = 1);
            return I;
        }

        private static SquareMatrix Ones(int N)
        {
            var I = new SquareMatrix(N);
            0.To(N - 1).ForEach(i => 0.To(N - 1).ForEach(j => I[i, j] = 1));
            return I;
        }

        private static SquareMatrix Zeros(int N)
        {
            return new SquareMatrix(N);
        }

        public static SquareMatrix LaplaceBelTrami(double[] mu, double[] wt, int N)
        {
            // compute alpha

            var alpha = new ColumnVector(N + 1); //zeros( N + 1, 1 );

            for (int im = 1; im < N + 1; im++)
            {
                alpha[im] = alpha[im - 1] - 2.0 * mu[im - 1] * wt[im - 1];
            }

            // allocate memory for LB matrix

            var LB = new SquareMatrix(N); // actually tridiagonal

            // compute the LB matrix

            double beta, gamma;

            beta = alpha[1] / (mu[1] - mu[0]) / wt[0];

            LB[0, 0] = -beta;
            LB[0, 1] = beta;

            for (int im = 1; im < N - 1; im++)
            {
                beta = alpha[im + 1] / (mu[im + 1] - mu[im]) / wt[im];
                gamma = alpha[im] / (mu[im] - mu[im - 1]) / wt[im];

                LB[im, im] = -beta - gamma;
                LB[im, im - 1] = gamma;
                LB[im, im + 1] = beta;
            }

            gamma = alpha[N - 1] / (mu[N - 1] - mu[N - 2]) / wt[N - 1];

            LB[N - 1, N - 1] = -gamma;
            LB[N - 1, N - 2] = gamma;

            return LB;
        }

        public static double[] PWHalfSpace(double mu_a, double mu_s, double[] mu, double[] wt, SquareMatrix L, int N)
        {
            var zeros = Zeros(N);
            var eye = Eye(N);

            // construct the eigenvalue problem
            
            var A = -mu_a * eye + mu_s * L;

            var invWtMatrix = new SymmetricMatrix(N);
            0.To(N - 1).ForEach(i => invWtMatrix[i, i] = 1 / mu[i]); // diag(wt)

            A = invWtMatrix * A;

            Console.WriteLine("A:\n");
            PrintMatrix(A);

            // solve the eigenvalue problems
            var e = A.Eigendecomposition();

            //0.To(N - 1).ForEach(i => PrintVector(e.Eigenvector(i).ToArray()));

            // sort the eigenvalues
            var indices = from i in 0.To(N - 1)
                orderby e.Eigenpairs[i].Eigenvalue.Re
                select i;
            // compute the expansion coefficients for the solution

            // eval           = eval(N/2+1:N);
            // evec           = V(:,indx(N/2+1:N));
            var eval = indices.Select(i => e.Eigenpairs[i].Eigenvalue.Re).Skip(N / 2);
            var evec = indices.Select(i => e.Eigenpairs[i].Eigenvector).Skip(N / 2);

            Console.WriteLine("eval:\n");
            PrintVector(eval.ToArray());

            var temp1 = evec.Select(t => t.Take(N / 2).Reverse());

            var M = new SquareMatrix(N / 2);
            temp1.ForEach((eveci, i) =>
            {
                eveci.ForEach((evecj, j) =>
                {
                    M[i, j] = evecj.Re; // ??
                });
            });

            Console.WriteLine("M:\n");
            PrintMatrix(M);

            // solve the linear system for the expansion coefficients

            var vectorValues = new double[N / 2];
            vectorValues[N / 2 - 1] = 1.0 / wt[N - 1];

            var mInverse = M.Inverse();
            Console.WriteLine("mInverse:\n");
            PrintMatrix(mInverse);

            var c = mInverse.Transpose * new ColumnVector(vectorValues); // why Transpose?
            //var c = mInverse * new ColumnVector(vectorValues);

            // compute the reflectance

            var temp2 = new RectangularMatrix(N, N/2);
            evec.ForEach((eveci, i) =>
            {
                eveci.ForEach((evecj, j) =>
                {
                    temp2[j, i] = evecj.Re;
                });
            });
            Console.WriteLine("temp2 (evec):\n");
            PrintMatrix(temp2);

            var R = temp2 * new ColumnVector(c.Take(N / 2).ToArray());
            Console.WriteLine("R:\n");
            PrintMatrix(R);

            //return R.ToArray();
            return R.Reverse().ToArray(); // why backwards?
        }

        private static void PrintMatrix(SquareMatrix M)
        {
            PrintMatrix(new RectangularMatrix(M.RowCount, M.ColumnCount));    
        }

        private static void PrintMatrix(ColumnVector M)
        {
            PrintMatrix(new RectangularMatrix(M.RowCount, M.ColumnCount));
        }

        private static void PrintMatrix(RectangularMatrix M)
        {
            for (int r = 0; r < M.RowCount; r++)
            {
                for (int c = 0; c < M.ColumnCount; c++)
                {
                    Console.Write(String.Format("{0,12:g8} ", M[r, c]));
                }
                Console.WriteLine(String.Empty);
            }
            Console.WriteLine("--");
        }

        private static void PrintVector(IList<double> v)
        {
            for (int i = 0; i < v.Count; i++)
            {
                Console.Write(String.Format("{0} ", v[i]));
            }
            Console.WriteLine(String.Empty);
            Console.WriteLine("-");
        }
    }

    internal static class HelperMath
    {
        public static double Pow(this double x, double y)
        {
            return Math.Pow(x, y);
        }

        public static IEnumerable<int> To(this int from, int to)
        {
            return Enumerable.Range(from, to - from + 1);
        }
    }
}
