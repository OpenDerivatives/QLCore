/*
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

namespace QLCore
{
   internal class Var_Helper
   {
      public LfmCovarianceParameterization param_;
      public int i_;
      public int j_;

      public Var_Helper(LfmCovarianceParameterization param, int i, int j)
      {
         param_ = param;
         i_ = i;
         j_ = j;
      }

      public virtual double value(double t)
      {
         Matrix m = param_.diffusion(t, new Vector());
         double u = 0;
         m.row(i_).ForEach((ii, vv) => u += vv * m.row(j_)[ii]);
         return u;
      }
   }

   public abstract class LfmCovarianceParameterization
   {
      protected int size_;
      protected int factors_;

      protected LfmCovarianceParameterization(int size, int factors)
      {
         size_ = size;
         factors_ = factors;
      }

      public int size()
      {
         return size_;
      }

      public int factors()
      {
         return factors_;
      }

      public abstract Matrix diffusion(double t);

      public abstract Matrix diffusion(double t, Vector x);

      public virtual Matrix covariance(double t)
      {
         return covariance(t, null);
      }

      public virtual Matrix covariance(double t, Vector x)
      {
         Matrix sigma = diffusion(t, x);
         Matrix result = sigma * Matrix.transpose(sigma);
         return result;
      }

      public virtual Matrix integratedCovariance(double t, Vector x = null)
      {
         // this implementation is not intended for production.
         // because it is too slow and too inefficient.
         // This method is useful for testing and R&D.
         // Please overload the method within derived classes.

         Utils.QL_REQUIRE(x == null, () => "can not handle given x here");

         Matrix tmp = new Matrix(size_, size_, 0.0);

         for (int i = 0; i < size_; ++i)
         {
            for (int j = 0; j <= i; ++j)
            {
               Var_Helper helper = new Var_Helper(this, i, j);
               GaussKronrodAdaptive integrator = new GaussKronrodAdaptive(1e-10, 10000);
               for (int k = 0; k < 64; ++k)
               {
                  tmp[i, j] += integrator.value(helper.value, k * t / 64.0, (k + 1) * t / 64.0);
               }
               tmp[j, i] = tmp[i, j];
            }
         }
         return tmp;
      }
   }

}
