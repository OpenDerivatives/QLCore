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
   /*! Kernel function in the statistical sense, e.g. a nonnegative,
       real-valued function which integrates to one and is symmetric.

       Derived classes will serve as functors.
   */

   public interface IKernelFunction
   {
      double value(double x) ;
   }


   //! Gaussian kernel function
   public class GaussianKernel : IKernelFunction
   {
      public GaussianKernel(double average, double sigma)
      {
         nd_ = new NormalDistribution(average, sigma);
         cnd_ = new CumulativeNormalDistribution(average, sigma);
         // normFact is \sqrt{2*\pi}.
         normFact_ = Const.M_SQRT2 * Const.M_SQRTPI;
      }

      public double value(double x)
      {
         return nd_.value(x) * normFact_;
      }

      public double derivative(double x)
      {
         return nd_.derivative(x) * normFact_;
      }

      public double primitive(double x)
      {
         return cnd_.value(x) * normFact_;
      }

      private NormalDistribution nd_;
      private CumulativeNormalDistribution cnd_;
      private double normFact_;

   }

}
