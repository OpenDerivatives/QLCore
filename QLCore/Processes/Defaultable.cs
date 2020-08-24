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

   public abstract class Defaultable
   {
      public abstract double defaultRecovery(double t, double underlying);
      public abstract double hazardRate(double t, double underlying);
   }

   public class NegativePowerDefaultIntensity : Defaultable
   {
      private double alpha_;
      private double p_;
      private double recovery_;
      public NegativePowerDefaultIntensity(double alpha, double p) : this(alpha, p, 0.0)
      {
      }

      public NegativePowerDefaultIntensity(double alpha, double p, double recovery)
      {
         alpha_ = alpha;
         p_ = p;
         recovery_ = recovery;
      }
      public override double hazardRate(double t, double s)
      {
         if (s <= 0.0)
            return 0.0;

         return alpha_ * Math.Pow(s, -p_);
      }
      public override double defaultRecovery(double t, double s)
      {
         return recovery_;
      }
   }

   public class ConstantDefaultIntensity : Defaultable
   {
      private double constant_;
      private double recovery_;
      public ConstantDefaultIntensity(double constant) : this(constant, 0.0)
      {
      }
      public ConstantDefaultIntensity(double constant, double recovery)
      {
         constant_ = constant;
         recovery_ = recovery;
      }
      public override double hazardRate(double t, double s)
      {
         return constant_;
      }
      public override double defaultRecovery(double t, double s)
      {
         return recovery_;
      }
   }
}
