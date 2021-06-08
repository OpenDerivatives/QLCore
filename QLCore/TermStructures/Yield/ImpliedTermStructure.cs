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
   //! Implied term structure at a given date in the future
   /*! The given date will be the implied reference date.

       \note This term structure will remain linked to the original structure, i.e., any changes in the latter will be
             reflected in this structure as well.

       \ingroup yieldtermstructures

       \test
       - the correctness of the returned values is tested by checking them against numerical calculations.
       - observability against changes in the underlying term structure is checked.
   */
   public class ImpliedTermStructure : YieldTermStructure
   {
      private Handle<YieldTermStructure> originalCurve_;

      public ImpliedTermStructure(Handle<YieldTermStructure> h, Date referenceDate)
         : base(referenceDate)
      {
         originalCurve_ = h;
      }

      // YieldTermStructure interface
      public override DayCounter dayCounter() { return originalCurve_.link.dayCounter(); }
      public override Calendar calendar() { return originalCurve_.link.calendar(); }
      public override int settlementDays() { return originalCurve_.link.settlementDays(); }
      public override Date maxDate() { return originalCurve_.link.maxDate(); }

      //! returns the discount factor as seen from the evaluation date
      /* t is relative to the current reference date and needs to be converted to the time relative
         to the reference date of the original curve */
      protected internal override double discountImpl(double t)
      {
         Date refDate = referenceDate();
         double originalTime = t + dayCounter().yearFraction(originalCurve_.link.referenceDate(), refDate);
         /* discount at evaluation date cannot be cached since the original curve could change between
            invocations of this method */
         return originalCurve_.link.discount(originalTime, true) / originalCurve_.link.discount(refDate, true);
      }
   }
}
