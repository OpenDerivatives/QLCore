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

namespace QLCore
{
   // Interface class to map the functionality of SobolBrownianGenerator
   // to the "conventional" sequence generator interface
   public class SobolBrownianBridgeRsg : IRNG
   {
      public SobolBrownianBridgeRsg(int factors, int steps,
                                    SobolBrownianGenerator.Ordering ordering = SobolBrownianGenerator.Ordering.Diagonal,
                                    ulong seed = 0,
                                    SobolRsg.DirectionIntegers directionIntegers = SobolRsg.DirectionIntegers.JoeKuoD7)
      {
         factors_ = factors;
         steps_ = steps;
         dim_ = factors * steps;
         seq_ = new Sample<List<double>>(new InitializedList<double>(factors * steps), 1.0) ;
         gen_ = new SobolBrownianGenerator(factors, steps, ordering, seed, directionIntegers);
      }

      public Sample<List<double>> nextSequence()
      {
         gen_.nextPath();
         List<double> output = new InitializedList<double>(factors_);
         for (int i = 0; i < steps_; ++i)
         {
            gen_.nextStep(output);
            for (int j = 0; j < output.Count ; j++)
            {
               seq_.value[j + i * factors_] = output[j];
            }
         }

         return seq_;
      }
      public Sample<List<double>> lastSequence() {return seq_;}
      public IRNG factory(int dimensionality, ulong seed)
      {
         throw new NotImplementedException();
      }

      public int dimension() {return dim_;}

      private int factors_, steps_, dim_;
      private Sample<List<double>> seq_;
      private SobolBrownianGenerator gen_;
   }
}
