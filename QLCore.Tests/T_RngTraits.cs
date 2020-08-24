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
using System.Collections.Generic;
using Xunit;
using QLCore;

namespace TestSuite
{

   public class T_RNGTraits
   {
      [Fact]
      public void testGaussian()
      {
         //("Testing Gaussian pseudo-random number generation...");

         var rsg = (InverseCumulativeRsg<RandomSequenceGenerator<MersenneTwisterUniformRng>, InverseCumulativeNormal>)
                   new PseudoRandom().make_sequence_generator(100, 1234);

         List<double> values = rsg.nextSequence().value;
         double sum = 0.0;
         for (int i = 0; i < values.Count; i++)
            sum += values[i];

         double stored = 4.09916;
         double tolerance = 1.0e-5;
         if (Math.Abs(sum - stored) > tolerance)
            QAssert.Fail("the sum of the samples does not match the stored value\n"
                         + "    calculated: " + sum + "\n"
                         + "    expected:   " + stored);
      }

      [Fact]
      public void testDefaultPoisson()
      {

         //("Testing Poisson pseudo-random number generation...");

         PoissonPseudoRandom.icInstance = new InverseCumulativePoisson();
         IRNG rsg = (IRNG)new PoissonPseudoRandom().make_sequence_generator(100, 1234);

         List<double> values = rsg.nextSequence().value;
         double sum = 0.0;
         for (int i = 0; i < values.Count; i++)
            sum += values[i];

         double stored = 108.0;
         if (!Utils.close(sum, stored))
            QAssert.Fail("the sum of the samples does not match the stored value\n"
                         + "    calculated: " + sum + "\n"
                         + "    expected:   " + stored);
      }

      [Fact]
      public void testCustomPoisson()
      {

         //("Testing custom Poisson pseudo-random number generation...");

         PoissonPseudoRandom.icInstance = new InverseCumulativePoisson(4.0);
         IRNG rsg = (IRNG)new PoissonPseudoRandom().make_sequence_generator(100, 1234);

         List<double> values = rsg.nextSequence().value;
         double sum = 0.0;
         for (int i = 0; i < values.Count; i++)
            sum += values[i];

         double stored = 409.0;
         if (!Utils.close(sum, stored))
            QAssert.Fail("the sum of the samples does not match the stored value\n"
                         + "    calculated: " + sum + "\n"
                         + "    expected:   " + stored);
      }
   }
}
