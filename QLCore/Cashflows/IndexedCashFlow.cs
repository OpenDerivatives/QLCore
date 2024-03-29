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

namespace QLCore
{
   //! Cash flow dependent on an index ratio.

   /*! This cash flow is not a coupon, i.e., there's no accrual.  The
       amount is either i(T)/i(0) or i(T)/i(0) - 1, depending on the
       growthOnly parameter.

       We expect this to be used inside an instrument that does all the date
       adjustment etc., so this takes just dates and does not change them.
       growthOnly = false means i(T)/i(0), which is a bond-type setting.
       growthOnly = true means i(T)/i(0) - 1, which is a swap-type setting.
   */
   public class IndexedCashFlow : CashFlow
   {
      public IndexedCashFlow(double notional,
                             Index index,
                             Date baseDate,
                             Date fixingDate,
                             Date paymentDate,
                             bool growthOnly = false)
         : base(index.settings())
      {
         notional_ = notional;
         index_ = index;
         baseDate_ = baseDate;
         fixingDate_ = fixingDate;
         paymentDate_ = paymentDate;
         growthOnly_ = growthOnly;
      }

      public override Date date() { return paymentDate_; }
      public virtual double notional()  { return notional_; }
      public virtual Date baseDate()  { return baseDate_; }
      public virtual Date fixingDate() { return fixingDate_; }
      public virtual Index index() { return index_; }
      public virtual bool growthOnly() { return growthOnly_; }

      public override double amount()
      {
         double I0 = index_.fixing(baseDate_);
         double I1 = index_.fixing(fixingDate_);

         if (growthOnly_)
            return notional_ * (I1 / I0 - 1.0);
         else
            return notional_ * (I1 / I0);

      }
      private double notional_;
      private Index index_;
      private Date baseDate_, fixingDate_, paymentDate_;
      private bool growthOnly_;
   }
}
