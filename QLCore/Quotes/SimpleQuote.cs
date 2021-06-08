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
   // simple quote class
   //! market element returning a stored value
   public class SimpleQuote : Quote
   {
      private double? value_;

      public SimpleQuote() { }
      public SimpleQuote(double? value) { value_ = value; }

      //! Quote interface
      public override double value()
      {
         if (!isValid())
            throw new ArgumentException("invalid SimpleQuote");
         return value_.GetValueOrDefault();
      }
      public override bool isValid() { return value_ != null; }

      //! returns the difference between the new value and the old value
      public double setValue(double? value)
      {
         double? diff = value - value_;
         if (diff.IsNotEqual(0.0))
         {
            value_ = value;
         }
         return diff.GetValueOrDefault();
      }

      public void reset()
      {
         setValue(null);
      }
   }
}