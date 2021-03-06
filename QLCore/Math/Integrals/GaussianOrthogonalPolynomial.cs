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
   //! orthogonal polynomial for Gaussian quadratures
   /*! References:
       Gauss quadratures and orthogonal polynomials

       G.H. Gloub and J.H. Welsch: Calculation of Gauss quadrature rule.
       Math. Comput. 23 (1986), 221-230

       "Numerical Recipes in C", 2nd edition,
       Press, Teukolsky, Vetterling, Flannery,

       The polynomials are defined by the three-term recurrence relation

   */
   public abstract class GaussianOrthogonalPolynomial
   {
      public abstract double mu_0();
      public abstract double alpha(int i);
      public abstract double beta(int i);
      public abstract double w(double x);

      public double value(int n, double x)
      {
         if (n > 1)
         {
            return (x - alpha(n - 1)) * value(n - 1, x)
                   - beta(n - 1) * value(n - 2, x);
         }
         else if (n == 1)
         {
            return x - alpha(0);
         }
         return 1;
      }

      public double weightedValue(int n, double x)
      {
         return Math.Sqrt(w(x)) * value(n, x);
      }
   }

   //! Gauss-Laguerre polynomial
   public class GaussLaguerrePolynomial : GaussianOrthogonalPolynomial
   {
      private double s_;

      public GaussLaguerrePolynomial() : this(0.0) { }
      public GaussLaguerrePolynomial(double s)
      {
         s_ = s;
         Utils.QL_REQUIRE(s > -1.0, () => "s must be bigger than -1");
      }

      public override double mu_0() { return Math.Exp(GammaFunction.logValue(s_ + 1)); }
      public override double alpha(int i) { return 2 * i + 1 + s_; }
      public override double beta(int i) { return i * (i + s_); }
      public override double w(double x) { return Math.Pow(x, s_) * Math.Exp(-x); }
   }

   //! Gauss-Hermite polynomial
   public class GaussHermitePolynomial : GaussianOrthogonalPolynomial
   {
      private double mu_;

      public GaussHermitePolynomial() : this(0.0) { }
      public GaussHermitePolynomial(double mu)
      {
         mu_ = mu;
         Utils.QL_REQUIRE(mu > -0.5, () => "mu must be bigger than -0.5");
      }

      public override double mu_0() { return Math.Exp(GammaFunction.logValue(mu_ + 0.5)); }
      public override double alpha(int i) { return 0.0; }
      public override double beta(int i) { return (i % 2 != 0) ? i / 2.0 + mu_ : i / 2.0; }
      public override double w(double x) { return Math.Pow(Math.Abs(x), 2 * mu_) * Math.Exp(-x * x); }
   }

   //! Gauss-Jacobi polynomial
   public class GaussJacobiPolynomial : GaussianOrthogonalPolynomial
   {
      private double alpha_;
      private double beta_;

      public GaussJacobiPolynomial(double alpha, double beta)
      {
         alpha_ = alpha;
         beta_  = beta;

         Utils.QL_REQUIRE(alpha_ + beta_ > -2.0, () => "alpha+beta must be bigger than -2");
         Utils.QL_REQUIRE(alpha_ > -1.0, () => "alpha must be bigger than -1");
         Utils.QL_REQUIRE(beta_ > -1.0, () => "beta  must be bigger than -1");
      }

      public override double mu_0()
      {
         return Math.Pow(2.0, alpha_ + beta_ + 1)
                * Math.Exp(GammaFunction.logValue(alpha_ + 1)
                           + GammaFunction.logValue(beta_ + 1)
                           - GammaFunction.logValue(alpha_ + beta_ + 2));
      }
      public override double alpha(int i)
      {
         double num = beta_ * beta_ - alpha_ * alpha_;
         double denom = (2.0 * i + alpha_ + beta_) * (2.0 * i + alpha_ + beta_ + 2);

         if (denom.IsEqual(0.0))
         {
            if (num.IsNotEqual(0.0))
            {
               Utils.QL_FAIL("can't compute a_k for jacobi integration");
            }
            else
            {
               // l'Hospital
               num  = 2 * beta_;
               denom = 2 * (2.0 * i + alpha_ + beta_ + 1);

               Utils.QL_REQUIRE(denom.IsNotEqual(0.0), () => "can't compute a_k for jacobi integration");
            }
         }

         return num / denom;
      }
      public override double beta(int i)
      {
         double num = 4.0 * i * (i + alpha_) * (i + beta_) * (i + alpha_ + beta_);
         double denom = (2.0 * i + alpha_ + beta_) * (2.0 * i + alpha_ + beta_)
                        * ((2.0 * i + alpha_ + beta_) * (2.0 * i + alpha_ + beta_) - 1);

         if (denom.IsEqual(0.0))
         {
            if (num.IsNotEqual(0.0))
            {
               Utils.QL_FAIL("can't compute b_k for jacobi integration");
            }
            else
            {
               // l'Hospital
               num  = 4.0 * i * (i + beta_) * (2.0 * i + 2 * alpha_ + beta_);
               denom = 2.0 * (2.0 * i + alpha_ + beta_);
               denom *= denom - 1;
               Utils.QL_REQUIRE(denom.IsNotEqual(0.0), () => "can't compute b_k for jacobi integration");
            }
         }
         return num / denom;
      }
      public override double w(double x)
      {
         return Math.Pow(1 - x, alpha_) * Math.Pow(1 + x, beta_);
      }
   }

   //! Gauss-Legendre polynomial
   public class GaussLegendrePolynomial : GaussJacobiPolynomial
   {
      public GaussLegendrePolynomial() : base(0.0, 0.0) { }
   }

   //! Gauss-Chebyshev polynomial
   public class GaussChebyshevPolynomial : GaussJacobiPolynomial
   {
      public GaussChebyshevPolynomial() : base(-0.5, -0.5) { }
   }

   //! Gauss-Chebyshev polynomial (second kind)
   public class GaussChebyshev2ndPolynomial : GaussJacobiPolynomial
   {
      public GaussChebyshev2ndPolynomial() : base(0.5, 0.5) { }
   }

   //! Gauss-Gegenbauer polynomial
   public class GaussGegenbauerPolynomial : GaussJacobiPolynomial
   {
      public GaussGegenbauerPolynomial(double lambda) : base(lambda - 0.5, lambda - 0.5) { }
   }

   //! Gauss hyperbolic polynomial
   public class GaussHyperbolicPolynomial : GaussianOrthogonalPolynomial
   {
      public override double mu_0() { return Const.M_PI; }
      public override double alpha(int i) { return 0.0; }
      public override double beta(int i) { return i != 0 ? Const.M_PI_2 * Const.M_PI_2 * i * i : Const.M_PI; }
      public override double w(double x) { return 1 / Math.Cosh(x); }
   }
}
