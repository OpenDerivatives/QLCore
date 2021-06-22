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
   //! Predetermined cash flow
   /*! This cash flow pays a predetermined amount at a given date. */
   public class SimpleCashFlow : CashFlow
   {
      private double amount_;
      public override double amount() { return amount_; }
      private Date date_;
      public override Date date() { return date_; }

      public SimpleCashFlow(Settings settings, double amount, Date date)
      : base(settings)
      {
         Utils.QL_REQUIRE(date != null, () => "null date SimpleCashFlow");
         amount_ = amount;
         date_ = date;
      }
   }

   //! Bond redemption
   /*! This class specializes SimpleCashFlow so that visitors
       can perform more detailed cash-flow analysis.
   */
   public class Redemption : SimpleCashFlow
   {
      public Redemption(Settings settings, double amount, Date date) : base(settings, amount, date) { }
   }

   //! Amortizing payment
   /*! This class specializes SimpleCashFlow so that visitors
       can perform more detailed cash-flow analysis.
   */
   public class AmortizingPayment : SimpleCashFlow
   {
      public AmortizingPayment(Settings settings, double amount, Date date) : base(settings, amount, date) { }
   }

   //! Voluntary Prepay
   /*! This class specializes SimpleCashFlow so that visitors
       can perform more detailed cash-flow analysis.
   */
   public class VoluntaryPrepay : SimpleCashFlow
   {
      public VoluntaryPrepay(Settings settings, double amount, Date date) : base(settings, amount, date) { }
   }
}
