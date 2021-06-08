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
   public class CompositeQuote : Quote
   {
      //! market element whose value depends on two other market element
      /*! \test the correctness of the returned values is tested by
                checking them against numerical calculations.
      */
      private Handle<Quote> element1_;
      private Handle<Quote> element2_;
      private Func<double, double, double> f_;

      public CompositeQuote(Handle<Quote> element1, Handle<Quote> element2, Func<double, double, double> f)
      {
         element1_ = element1;
         element2_ = element2;
         f_ = f;
      }

      // inspectors
      public double value1() { return element1_.link.value(); }
      public double value2() { return element2_.link.value(); }

      //! Quote interface
      public override double value()
      {
         if (!isValid())
            throw new ArgumentException("invalid DerivedQuote");
         return f_(element1_.link.value(), element2_.link.value());
      }

      public override bool isValid()
      {
         return (element1_.link.isValid() && element2_.link.isValid());
      }

   }
}
