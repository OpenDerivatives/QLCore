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

using System;

namespace QLCore
{
   //! forward-measure stochastic process
   /*! stochastic process whose dynamics are expressed in the forward
       measure.

       \ingroup processes
   */
   public class ForwardMeasureProcess : StochasticProcess
   {
      public virtual void setForwardMeasureTime(double T)
      {
         T_ = T;
      }
      public double getForwardMeasureTime() { return T_; }

      protected ForwardMeasureProcess() {}
      protected ForwardMeasureProcess(double T)
      {
         T_ = T;
      }
      protected ForwardMeasureProcess(IDiscretization disc)
         : base(disc)
      {}

      protected double T_;
      public override int size()
      {
         throw new NotImplementedException();
      }

      public override Vector initialValues()
      {
         throw new NotImplementedException();
      }

      public override Vector drift(double t, Vector x)
      {
         throw new NotImplementedException();
      }

      public override Matrix diffusion(double t, Vector x)
      {
         throw new NotImplementedException();
      }
   }

   //! forward-measure 1-D stochastic process
   /*! 1-D stochastic process whose dynamics are expressed in the
        forward measure.

        \ingroup processes
   */
   public class ForwardMeasureProcess1D : StochasticProcess1D
   {
      public virtual void setForwardMeasureTime(double T)
      {
         T_ = T;
      }
      public double getForwardMeasureTime()
      {
         return T_;
      }

      protected ForwardMeasureProcess1D() {}
      protected ForwardMeasureProcess1D(double T)
      {
         T_ = T;
      }
      protected ForwardMeasureProcess1D(IDiscretization1D disc)
         : base(disc) {}

      protected double T_;
      public override double x0()
      {
         throw new NotImplementedException();
      }

      public override double drift(double t, double x)
      {
         throw new NotImplementedException();
      }

      public override double diffusion(double t, double x)
      {
         throw new NotImplementedException();
      }
   }

}
