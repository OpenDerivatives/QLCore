﻿/*
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
using System.Linq;

namespace QLCore
{
   //! Quote adapter for the last fixing available of a given Index
   class LastFixingQuote : Quote, IObserver
   {
      protected Index index_;

      public LastFixingQuote(Index index)
      {
         index_ = index;
         index_.registerWith(update);
      }

      //! Quote interface
      public override double value()
      {
         if (!isValid())
            throw new ArgumentException(index_.name() + " has no fixing");
         return index_.fixing(referenceDate());
      }

      public override bool isValid()
      {
         return index_.timeSeries().Count > 0;
      }

      public Date referenceDate()
      {
         return index_.timeSeries().Keys.Last(); // must be tested
      }

      public void update()
      {
         notifyObservers();
      }

   }
}
