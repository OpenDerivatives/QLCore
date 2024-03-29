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

using System.Collections.Generic;

namespace QLCore
{
   //! Predetermined cash flow
   /*! This cash flow pays a predetermined amount at a given date. */
   public abstract class Dividend : CashFlow
   {
      protected Date date_;
      // Event interface
      public override Date date() { return date_; }

      protected Dividend(Settings settings, Date date)
      : base(settings)
      {
         date_ = date;
      }

      public abstract double amount(double underlying);
   }

   //! Predetermined cash flow
   /*! This cash flow pays a predetermined amount at a given date. */
   public class FixedDividend : Dividend
   {
      protected double amount_;
      public override double amount() { return amount_; }
      public override double amount(double d) { return amount_; }

      public FixedDividend(Settings settings, double amount, Date date)
         : base(settings, date)
      {
         amount_ = amount;
      }
   }

   //! Predetermined cash flow
   /*! This cash flow pays a predetermined amount at a given date. */
   public class FractionalDividend : Dividend
   {
      protected double rate_;
      public double rate() { return rate_; }

      protected double? nominal_;
      public double? nominal() { return nominal_; }

      public FractionalDividend(Settings settings, double rate, Date date)
         : base(settings, date)
      {
         rate_ = rate;
         nominal_ = null;
      }

      public FractionalDividend(Settings settings, double rate, double nominal, Date date)
         : base(settings, date)
      {
         rate_ = rate;
         nominal_ = nominal;
      }

      // Dividend interface
      public override double amount()
      {
         Utils.QL_REQUIRE(nominal_ != null, () => "no nominal given");
         return rate_ * nominal_.GetValueOrDefault();
      }

      public override double amount(double underlying)
      {
         return rate_ * underlying;
      }
   }

   public static partial class Utils
   {
      //! helper function building a sequence of fixed dividends
      public static DividendSchedule DividendVector(Settings settings, List<Date> dividendDates, List<double> dividends)
      {
         QL_REQUIRE(dividendDates.Count == dividends.Count, () => "size mismatch between dividend dates and amounts");

         DividendSchedule items = new DividendSchedule();
         for (int i = 0; i < dividendDates.Count; i++)
            items.Add(new FixedDividend(settings, dividends[i], dividendDates[i]));
         return items;
      }
   }
}
