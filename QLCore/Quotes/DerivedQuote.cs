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
   public class DerivedQuote : Quote
   {
      //! market quote whose value depends on another quote
      /*! \test the correctness of the returned values is tested by
                checking them against numerical calculations.
      */
      private Handle<Quote> element_;
      private Func<double, double> f_;

      public DerivedQuote(Handle<Quote> element, Func<double, double> f)
      {
         element_ = element;
         f_ = f;

         element_.registerWith(this.update);
      }

      //! Quote interface
      public override double value()
      {
         if (!isValid())
            throw new ArgumentException("invalid DerivedQuote");
         return f_(element_.link.value());
      }

      public override bool isValid()
      {
         return element_.link.isValid();
      }

      public void update()
      {
         notifyObservers();
      }

   }
}
