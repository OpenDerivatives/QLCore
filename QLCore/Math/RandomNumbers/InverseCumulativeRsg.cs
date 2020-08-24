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
   //! Inverse cumulative random sequence generator
   /*! It uses a sequence of uniform deviate in (0, 1) as the
       source of cumulative distribution values.
       Then an inverse cumulative distribution is used to calculate
       the distribution deviate.

       The uniform deviate sequence is supplied by USG.
       The inverse cumulative distribution is supplied by IC.
   */

   public class InverseCumulativeRsg<USG, IC> : IRNG where USG : IRNG where IC : IValue
   {
      private USG uniformSequenceGenerator_;
      private int dimension_;
      private Sample<List<double>> x_;
      private IC ICD_;

      public InverseCumulativeRsg(USG uniformSequenceGenerator)
      {
         uniformSequenceGenerator_ = uniformSequenceGenerator;
         dimension_ = uniformSequenceGenerator_.dimension();
         x_ = new Sample<List<double>>(new InitializedList<double>(dimension_), 1.0);
      }

      public InverseCumulativeRsg(USG uniformSequenceGenerator, IC inverseCumulative) : this(uniformSequenceGenerator)
      {
         ICD_ = inverseCumulative;
      }

      #region IRNG interface

      //! returns next sample from the Gaussian distribution
      public Sample<List<double>> nextSequence()
      {
         Sample<List<double>> sample = uniformSequenceGenerator_.nextSequence();
         x_.weight = sample.weight;
         for (int i = 0; i < dimension_; i++)
         {
            x_.value[i] = ICD_.value(sample.value[i]);
         }
         return x_;
      }

      public Sample<List<double>> lastSequence()
      {
         return x_;
      }

      public int dimension()
      {
         return dimension_;
      }

      public IRNG factory(int dimensionality, ulong seed)
      {
         throw new NotSupportedException();
      }

      #endregion
   }
}
