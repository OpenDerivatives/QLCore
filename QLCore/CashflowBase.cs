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
   //! Base class for cash flows. This class is purely virtual and acts as a base class for the actual cash flow implementations.
   public abstract class CashFlow : Event, IComparable<CashFlow>
   {
      #region Event interface

      public override bool hasOccurred(Date refDate = null, bool? includeRefDate = null)
      {

         // easy and quick handling of most cases
         if (refDate != null)
         {
            Date cf = date();
            if (refDate < cf)
               return false;
            if (cf < refDate)
               return true;
         }

         if (refDate == null ||  refDate == Settings.Instance.evaluationDate())
         {
            // today's date; we override the bool with the one
            // specified in the settings (if any)
            bool? includeToday = Settings.Instance.includeTodaysCashFlows;
            if (includeToday.HasValue)
               includeRefDate = includeToday;
         }
         return base.hasOccurred(refDate, includeRefDate);

      }

      #endregion

      #region CashFlow interface

      //! returns the amount of the cash flow
      //! The amount is not discounted, i.e., it is the actual  amount paid at the cash flow date.
      public abstract double amount();
      //! returns the date that the cash flow trades exCoupon
      public virtual Date exCouponDate() {return null;}
      //! returns true if the cashflow is trading ex-coupon on the refDate
      public bool tradingExCoupon(Date refDate = null)
      {
         Date ecd = exCouponDate();
         if (ecd == null)
            return false;

         Date ref_ = refDate ?? Settings.Instance.evaluationDate();

         return ecd <= ref_;
      }

      #endregion

      public int CompareTo(CashFlow cf) { return date().CompareTo(cf.date()); }

      public override bool Equals(Object cf)
      {
         var other = cf as CashFlow;
         if (ReferenceEquals(other, null))
         {
            return false;
         }
         return CompareTo(other) == 0;
      }

      public override int GetHashCode()
      {
         return date().serialNumber();
      }

      public static bool operator ==(CashFlow left, CashFlow right)
      {
         if (ReferenceEquals(left, null))
         {
            return ReferenceEquals(right, null);
         }
         return left.Equals(right);
      }
      public static bool operator >(CashFlow left, CashFlow right)
      {
         return left.date() > right.date() ;
      }
      public static bool operator >=(CashFlow left, CashFlow right)
      {
         return left.date() >= right.date();
      }
      public static bool operator <(CashFlow left, CashFlow right)
      {
         return left.date() < right.date() ;
      }
      public static bool operator <=(CashFlow left, CashFlow right)
      {
         return left.date() <= right.date();
      }
      public static bool operator !=(CashFlow left, CashFlow right)
      {
         return !(left == right);
      }
   }
}
