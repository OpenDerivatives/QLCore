﻿/*
 Copyright (C) 2020 Jean-Camille Tournier (mail@tournierjc.fr)

 This file is part of QLCore Project https://github.com/OpenDerivatives/QLCore

 QLCore is free software: you can redistribute it and/or modify it
 under the terms of the QLCore and QLNet license. You should have received a
 copy of the license along with this program; if not, license is
 available at https://github.com/OpenDerivatives/QLCore/LICENSE.

 QLCore is a forked of QLNet which is a based on QuantLib, a free-software/open-source
 library for financial quantitative analysts and developers - http://quantlib.org/
 The QuantLib license is available online at http://quantlib.org/license.shtml and the
 QLNet license is available online at https://github.com/amaggiulli/QLNet/blob/develop/LICENSE.

 This program is distributed in the hope that it will be useful, but WITHOUT
 ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 FOR A PARTICAR PURPOSE. See the license for more details.
*/

using System;

namespace QLCore
{
   public static partial class MatrixUtilitites
   {
      //! algorithm used for matricial pseudo square root
      public enum SalvagingAlgorithm
      {
         None, Spectral, Hypersphere, LowerDiagonal, Higham
      }

      public static void normalizePseudoRoot(Matrix matrix, Matrix pseudo)
      {
         int size = matrix.rows();
         Utils.QL_REQUIRE(size == pseudo.rows(), () =>
                          "matrix/pseudo mismatch: matrix rows are " + size + " while pseudo rows are " + pseudo.columns());
         int pseudoCols = pseudo.columns();

         // row normalization
         for (int i = 0; i < size; ++i)
         {
            double norm = 0.0;
            for (int j = 0; j < pseudoCols; ++j)
               norm += pseudo[i, j] * pseudo[i, j];
            if (norm > 0.0)
            {
               double normAdj = Math.Sqrt(matrix[i, i] / norm);
               for (int j = 0; j < pseudoCols; ++j)
                  pseudo[i, j] *= normAdj;
            }
         }
      }


      //cost function for hypersphere and lower-diagonal algorithm
      class HypersphereCostFunction : CostFunction
      {
         private int size_;
         private bool lowerDiagonal_;
         private Matrix targetMatrix_;
         private Vector targetVariance_;
         private Matrix currentRoot_, tempMatrix_, currentMatrix_;

         public HypersphereCostFunction(Matrix targetMatrix, Vector targetVariance, bool lowerDiagonal)
         {
            size_ = targetMatrix.rows();
            lowerDiagonal_ = lowerDiagonal;
            targetMatrix_ = targetMatrix;
            targetVariance_ = targetVariance;
            currentRoot_ = new Matrix(size_, size_);
            tempMatrix_ = new Matrix(size_, size_);
            currentMatrix_ = new Matrix(size_, size_);
         }

         public override Vector values(Vector a)
         {
            Utils.QL_FAIL("values method not implemented");
            return null;
         }

         public override double value(Vector x)
         {
            int i, j, k;
            currentRoot_.fill(1);
            if (lowerDiagonal_)
            {
               for (i = 0; i < size_; i++)
               {
                  for (k = 0; k < size_; k++)
                  {
                     if (k > i)
                     {
                        currentRoot_[i, k] = 0;
                     }
                     else
                     {
                        for (j = 0; j <= k; j++)
                        {
                           if (j == k && k != i)
                              currentRoot_[i, k] *= Math.Cos(x[i * (i - 1) / 2 + j]);
                           else if (j != i)
                              currentRoot_[i, k] *= Math.Sin(x[i * (i - 1) / 2 + j]);
                        }
                     }
                  }
               }
            }
            else
            {
               for (i = 0; i < size_; i++)
               {
                  for (k = 0; k < size_; k++)
                  {
                     for (j = 0; j <= k; j++)
                     {
                        if (j == k && k != size_ - 1)
                           currentRoot_[i, k] *= Math.Cos(x[j * size_ + i]);
                        else if (j != size_ - 1)
                           currentRoot_[i, k] *= Math.Sin(x[j * size_ + i]);
                     }
                  }
               }
            }
            double temp, error = 0;
            tempMatrix_ = Matrix.transpose(currentRoot_);
            currentMatrix_ = currentRoot_ * tempMatrix_;
            for (i = 0; i < size_; i++)
            {
               for (j = 0; j < size_; j++)
               {
                  temp = currentMatrix_[i, j] * targetVariance_[i] * targetVariance_[j] - targetMatrix_[i, j];
                  error += temp * temp;
               }
            }
            return error;
         }
      }


      // Optimization function for hypersphere and lower-diagonal algorithm
      private static Matrix hypersphereOptimize(Matrix targetMatrix, Matrix currentRoot, bool lowerDiagonal)
      {
         int i, j, k, size = targetMatrix.rows();
         Matrix result = new Matrix(currentRoot);
         Vector variance = new Vector(size);
         for (i = 0; i < size; i++)
         {
            variance[i] = Math.Sqrt(targetMatrix[i, i]);
         }
         if (lowerDiagonal)
         {
            Matrix approxMatrix = result * Matrix.transpose(result);
            result = MatrixUtilities.CholeskyDecomposition(approxMatrix, true);
            for (i = 0; i < size; i++)
            {
               for (j = 0; j < size; j++)
               {
                  result[i, j] /= Math.Sqrt(approxMatrix[i, i]);
               }
            }
         }
         else
         {
            for (i = 0; i < size; i++)
            {
               for (j = 0; j < size; j++)
               {
                  result[i, j] /= variance[i];
               }
            }
         }

         ConjugateGradient optimize = new ConjugateGradient();
         EndCriteria endCriteria = new EndCriteria(100, 10, 1e-8, 1e-8, 1e-8);
         HypersphereCostFunction costFunction = new HypersphereCostFunction(targetMatrix, variance, lowerDiagonal);
         NoConstraint constraint = new NoConstraint();

         // hypersphere vector optimization

         if (lowerDiagonal)
         {
            Vector theta = new Vector(size * (size - 1) / 2);
            const double eps = 1e-16;
            for (i = 1; i < size; i++)
            {
               for (j = 0; j < i; j++)
               {
                  theta[i * (i - 1) / 2 + j] = result[i, j];
                  if (theta[i * (i - 1) / 2 + j] > 1 - eps)
                     theta[i * (i - 1) / 2 + j] = 1 - eps;
                  if (theta[i * (i - 1) / 2 + j] < -1 + eps)
                     theta[i * (i - 1) / 2 + j] = -1 + eps;
                  for (k = 0; k < j; k++)
                  {
                     theta[i * (i - 1) / 2 + j] /= Math.Sin(theta[i * (i - 1) / 2 + k]);
                     if (theta[i * (i - 1) / 2 + j] > 1 - eps)
                        theta[i * (i - 1) / 2 + j] = 1 - eps;
                     if (theta[i * (i - 1) / 2 + j] < -1 + eps)
                        theta[i * (i - 1) / 2 + j] = -1 + eps;
                  }
                  theta[i * (i - 1) / 2 + j] = Math.Acos(theta[i * (i - 1) / 2 + j]);
                  if (j == i - 1)
                  {
                     if (result[i, i] < 0)
                        theta[i * (i - 1) / 2 + j] = -theta[i * (i - 1) / 2 + j];
                  }
               }
            }
            Problem p = new Problem(costFunction, constraint, theta);
            optimize.minimize(p, endCriteria);
            theta = p.currentValue();
            result.fill(1);
            for (i = 0; i < size; i++)
            {
               for (k = 0; k < size; k++)
               {
                  if (k > i)
                  {
                     result[i, k] = 0;
                  }
                  else
                  {
                     for (j = 0; j <= k; j++)
                     {
                        if (j == k && k != i)
                           result[i, k] *= Math.Cos(theta[i * (i - 1) / 2 + j]);
                        else if (j != i)
                           result[i, k] *= Math.Sin(theta[i * (i - 1) / 2 + j]);
                     }
                  }
               }
            }
         }
         else
         {
            Vector theta = new Vector(size * (size - 1));
            const double eps = 1e-16;
            for (i = 0; i < size; i++)
            {
               for (j = 0; j < size - 1; j++)
               {
                  theta[j * size + i] = result[i, j];
                  if (theta[j * size + i] > 1 - eps)
                     theta[j * size + i] = 1 - eps;
                  if (theta[j * size + i] < -1 + eps)
                     theta[j * size + i] = -1 + eps;
                  for (k = 0; k < j; k++)
                  {
                     theta[j * size + i] /= Math.Sin(theta[k * size + i]);
                     if (theta[j * size + i] > 1 - eps)
                        theta[j * size + i] = 1 - eps;
                     if (theta[j * size + i] < -1 + eps)
                        theta[j * size + i] = -1 + eps;
                  }
                  theta[j * size + i] = Math.Acos(theta[j * size + i]);
                  if (j == size - 2)
                  {
                     if (result[i, j + 1] < 0)
                        theta[j * size + i] = -theta[j * size + i];
                  }
               }
            }
            Problem p = new Problem(costFunction, constraint, theta);
            optimize.minimize(p, endCriteria);
            theta = p.currentValue();
            result.fill(1);
            for (i = 0; i < size; i++)
            {
               for (k = 0; k < size; k++)
               {
                  for (j = 0; j <= k; j++)
                  {
                     if (j == k && k != size - 1)
                        result[i, k] *= Math.Cos(theta[j * size + i]);
                     else if (j != size - 1)
                        result[i, k] *= Math.Sin(theta[j * size + i]);
                  }
               }
            }
         }

         for (i = 0; i < size; i++)
         {
            for (j = 0; j < size; j++)
            {
               result[i, j] *= variance[i];
            }
         }
         return result;
      }


      // Matrix infinity norm. See Golub and van Loan (2.3.10) or
      // <http://en.wikipedia.org/wiki/Matrix_norm>
      private static double normInf(Matrix M)
      {
         int rows = M.rows();
         int cols = M.columns();
         double norm = 0.0;
         for (int i = 0; i < rows; ++i)
         {
            double colSum = 0.0;
            for (int j = 0; j < cols; ++j)
               colSum += Math.Abs(M[i, j]);
            norm = Math.Max(norm, colSum);
         }
         return norm;
      }


      // Take a matrix and make all the diagonal entries 1.
      private static Matrix projectToUnitDiagonalMatrix(Matrix M)
      {
         int size = M.rows();
         Utils.QL_REQUIRE(size == M.columns(), () => "matrix not square");

         Matrix result = new Matrix(M);
         for (int i = 0; i < size; ++i)
            result[i, i] = 1.0;

         return result;
      }


      // Take a matrix and make all the eigenvalues non-negative
      private static Matrix projectToPositiveSemidefiniteMatrix(Matrix M)
      {
         int size = M.rows();
         Utils.QL_REQUIRE(size == M.columns(), () => "matrix not square");

         Matrix diagonal = new Matrix(size, size);
         SymmetricSchurDecomposition jd = new SymmetricSchurDecomposition(M);
         for (int i = 0; i < size; ++i)
            diagonal[i, i] = Math.Max(jd.eigenvalues()[i], 0.0);

         Matrix result = jd.eigenvectors() * diagonal * Matrix.transpose(jd.eigenvectors());
         return result;
      }


      // implementation of the Higham algorithm to find the nearest correlation matrix.
      private static Matrix highamImplementation(Matrix A, int maxIterations, double tolerance)
      {
         int size = A.rows();
         Matrix R, Y = new Matrix(A), X = new Matrix(A), deltaS = new Matrix(size, size, 0.0);

         Matrix lastX = new Matrix(X);
         Matrix lastY = new Matrix(Y);

         for (int i = 0; i < maxIterations; ++i)
         {
            R = Y - deltaS;
            X = projectToPositiveSemidefiniteMatrix(R);
            deltaS = X - R;
            Y = projectToUnitDiagonalMatrix(X);

            // convergence test
            if (Math.Max(normInf(X - lastX) / normInf(X),
                         Math.Max(normInf(Y - lastY) / normInf(Y),
                                  normInf(Y - X) / normInf(Y)))
                <= tolerance)
            {
               break;
            }
            lastX = X;
            lastY = Y;
         }

         // ensure we return a symmetric matrix
         for (int i = 0; i < size; ++i)
            for (int j = 0; j < i; ++j)
               Y[i, j] = Y[j, i];

         return Y;
      }


      //! Returns the pseudo square root of a real symmetric matrix
      /*! Given a matrix \f$ M \f$, the result \f$ S \f$ is defined
          as the matrix such that \f$ S S^T = M. \f$
          If the matrix is not positive semi definite, it can
          return an approximation of the pseudo square root
          using a (user selected) salvaging algorithm.

          For more information see: "The most general methodology to create
          a valid correlation matrix for risk management and option pricing
          purposes", by R. Rebonato and P. Jдckel.
          The Journal of Risk, 2(2), Winter 1999/2000
          http://www.rebonato.com/correlationmatrix.pdf

          Revised and extended in "Monte Carlo Methods in Finance",
          by Peter Jдckel, Chapter 6.

          \pre the given matrix must be symmetric.

          \relates Matrix

          \warning Higham algorithm only works for correlation matrices.

          \test
          - the correctness of the results is tested by reproducing
            known good data.
          - the correctness of the results is tested by checking
            returned values against numerical calculations.
      */
      public static Matrix pseudoSqrt(Matrix matrix, SalvagingAlgorithm sa)
      {
         int size = matrix.rows();

#if QL_EXTRA_SAFETY_CHECKS
         checkSymmetry(matrix);
#else
         Utils.QL_REQUIRE(size == matrix.columns(), () =>
                          "non square matrix: " + size + " rows, " + matrix.columns() + " columns");
#endif

         // spectral (a.k.a Principal Component) analysis
         SymmetricSchurDecomposition jd = new SymmetricSchurDecomposition(matrix);
         Matrix diagonal = new Matrix(size, size, 0.0);

         // salvaging algorithm
         Matrix result = new Matrix(size, size);
         bool negative;
         switch (sa)
         {
            case SalvagingAlgorithm.None:
               // eigenvalues are sorted in decreasing order
               Utils.QL_REQUIRE(jd.eigenvalues()[size - 1] >= -1e-16, () =>
                                "negative eigenvalue(s) (" + jd.eigenvalues()[size - 1] + ")");
               result = MatrixUtilities.CholeskyDecomposition(matrix, true);
               break;

            case SalvagingAlgorithm.Spectral:
               // negative eigenvalues set to zero
               for (int i = 0; i < size; i++)
                  diagonal[i, i] = Math.Sqrt(Math.Max(jd.eigenvalues()[i], 0.0));

               result = jd.eigenvectors() * diagonal;
               normalizePseudoRoot(matrix, result);
               break;

            case SalvagingAlgorithm.Hypersphere:
               // negative eigenvalues set to zero
               negative = false;
               for (int i = 0; i < size; ++i)
               {
                  diagonal[i, i] = Math.Sqrt(Math.Max(jd.eigenvalues()[i], 0.0));
                  if (jd.eigenvalues()[i] < 0.0)
                     negative = true;
               }
               result = jd.eigenvectors() * diagonal;
               normalizePseudoRoot(matrix, result);

               if (negative)
                  result = hypersphereOptimize(matrix, result, false);
               break;

            case SalvagingAlgorithm.LowerDiagonal:
               // negative eigenvalues set to zero
               negative = false;
               for (int i = 0; i < size; ++i)
               {
                  diagonal[i, i] = Math.Sqrt(Math.Max(jd.eigenvalues()[i], 0.0));
                  if (jd.eigenvalues()[i] < 0.0)
                     negative = true;
               }
               result = jd.eigenvectors() * diagonal;

               normalizePseudoRoot(matrix, result);

               if (negative)
                  result = hypersphereOptimize(matrix, result, true);
               break;

            case SalvagingAlgorithm.Higham:
               int maxIterations = 40;
               double tol = 1e-6;
               result = highamImplementation(matrix, maxIterations, tol);
               result = MatrixUtilities.CholeskyDecomposition(result, true);
               break;

            default:
               Utils.QL_FAIL("unknown salvaging algorithm");
               break;
         }

         return result;
      }

      public static Matrix rankReducedSqrt(Matrix matrix,
                                           int maxRank,
                                           double componentRetainedPercentage,
                                           SalvagingAlgorithm sa)
      {
         int size = matrix.rows();

#if QL_EXTRA_SAFETY_CHECKS
         checkSymmetry(matrix);
#else
         Utils.QL_REQUIRE(size == matrix.columns(), () =>
                          "non square matrix: " + size + " rows, " + matrix.columns() + " columns");
#endif

         Utils.QL_REQUIRE(componentRetainedPercentage > 0.0, () => "no eigenvalues retained");
         Utils.QL_REQUIRE(componentRetainedPercentage <= 1.0, () => "percentage to be retained > 100%");
         Utils.QL_REQUIRE(maxRank >= 1, () => "max rank required < 1");

         // spectral (a.k.a Principal Component) analysis
         SymmetricSchurDecomposition jd = new SymmetricSchurDecomposition(matrix);
         Vector eigenValues = jd.eigenvalues();

         // salvaging algorithm
         switch (sa)
         {
            case SalvagingAlgorithm.None:
               // eigenvalues are sorted in decreasing order
               Utils.QL_REQUIRE(eigenValues[size - 1] >= -1e-16, () =>
                                "negative eigenvalue(s) (" + eigenValues[size - 1] + ")");
               break;
            case SalvagingAlgorithm.Spectral:
               // negative eigenvalues set to zero
               for (int i = 0; i < size; ++i)
                  eigenValues[i] = Math.Max(eigenValues[i], 0.0);
               break;
            case SalvagingAlgorithm.Higham:
            {
               int maxIterations = 40;
               double tolerance = 1e-6;
               Matrix adjustedMatrix = highamImplementation(matrix, maxIterations, tolerance);
               jd = new SymmetricSchurDecomposition(adjustedMatrix);
               eigenValues = jd.eigenvalues();
            }
            break;
            default:
               Utils.QL_FAIL("unknown or invalid salvaging algorithm");
               break;
         }

         // factor reduction
         double accumulate = 0;
         eigenValues.ForEach((ii, vv) => accumulate += eigenValues[ii]);
         double enough = componentRetainedPercentage * accumulate;

         if (componentRetainedPercentage.IsEqual(1.0))
         {
            // numerical glitches might cause some factors to be discarded
            enough *= 1.1;
         }
         // retain at least one factor
         double components = eigenValues[0];
         int retainedFactors = 1;
         for (int i = 1; components < enough && i < size; ++i)
         {
            components += eigenValues[i];
            retainedFactors++;
         }
         // output is granted to have a rank<=maxRank
         retainedFactors = Math.Min(retainedFactors, maxRank);

         Matrix diagonal = new Matrix(size, retainedFactors, 0.0);
         for (int i = 0; i < retainedFactors; ++i)
            diagonal[i, i] = Math.Sqrt(eigenValues[i]);
         Matrix result = jd.eigenvectors() * diagonal;

         normalizePseudoRoot(matrix, result);
         return result;
      }
   }
}
