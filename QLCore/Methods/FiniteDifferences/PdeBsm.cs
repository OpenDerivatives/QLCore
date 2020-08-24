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
   public class PdeBSM : PdeSecondOrderParabolic
   {
      private GeneralizedBlackScholesProcess process_;

      public PdeBSM() { }     // required for generics
      public PdeBSM(GeneralizedBlackScholesProcess process)
      {
         process_ = process;
      }

      public override double diffusion(double t, double x)
      {
         return process_.diffusion(t, x);
      }

      public override double drift(double t, double x)
      {
         return process_.drift(t, x);
      }

      public override double discount(double t, double x)
      {
         if (Math.Abs(t) < 1e-8)
            t = 0;
         return process_.riskFreeRate().link.forwardRate(t, t, Compounding.Continuous, Frequency.NoFrequency, true).rate();
      }

      public override PdeSecondOrderParabolic factory(GeneralizedBlackScholesProcess process) { return new PdeBSM(process); }
   }
}
