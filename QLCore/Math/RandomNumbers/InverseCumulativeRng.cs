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
   //! Inverse cumulative random number generator
   /*! It uses a uniform deviate in (0, 1) as the source of cumulative
       distribution values.
       Then an inverse cumulative distribution is used to calculate
       the distribution deviate.

       The uniform deviate is supplied by RNG.
       The inverse cumulative distribution is supplied by IC.
   */

   public class InverseCumulativeRng<RNG, IC> where RNG : IRNGTraits where IC : IValue, new ()
   {
      private RNG uniformGenerator_;
      private IC ICND_ = FastActivator<IC>.Create();

      public InverseCumulativeRng(RNG uniformGenerator)
      {
         uniformGenerator_ = uniformGenerator;
      }

      //! returns a sample from a Gaussian distribution
      public Sample<double> next()
      {
         Sample<double> sample = uniformGenerator_.next();
         return new Sample<double>(ICND_.value(sample.value), sample.weight);
      }
   }
}
