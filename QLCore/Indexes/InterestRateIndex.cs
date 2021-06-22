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
   //! base class for interest rate indexes
   /*! \todo add methods returning InterestRate */
   public abstract class InterestRateIndex : Index
   {
      protected InterestRateIndex(string familyName,
                                  Period tenor,
                                  int fixingDays,
                                  Currency currency,
                                  Calendar fixingCalendar,
                                  DayCounter dayCounter,
                                  Settings settings)
      {
         familyName_ = familyName;
         tenor_ = tenor;
         fixingDays_ = fixingDays;
         currency_ = currency;
         dayCounter_ = dayCounter;
         fixingCalendar_ = fixingCalendar;
         settings_ = settings ?? new Settings();

         tenor_.normalize();

         string res = familyName_;
         if (tenor_ == new Period(1, TimeUnit.Days))
         {
            if (fixingDays_ == 0)
               res += "ON";
            else if (fixingDays_ == 1)
               res += "TN";
            else if (fixingDays_ == 2)
               res += "SN";
            else
               res += tenor_.ToShortString();
         }
         else
            res += tenor_.ToShortString();
         res = res + " " + dayCounter_.name();
         name_ =  res;
      }

      // Index interface
      public override string name()
      {
         return name_;
      }
      public override Calendar fixingCalendar() { return fixingCalendar_; }
      public override bool isValidFixingDate(Date fixingDate) { return fixingCalendar().isBusinessDay(fixingDate); }
      public override double fixing(Date fixingDate, bool forecastTodaysFixing = false)
      {
         Utils.QL_REQUIRE(isValidFixingDate(fixingDate), () => "Fixing date " + fixingDate + " is not valid");

         Date today = settings_.evaluationDate();

         if (fixingDate > today ||
             (fixingDate == today && forecastTodaysFixing))
            return forecastFixing(fixingDate);

         if (fixingDate < today || settings_.enforcesTodaysHistoricFixings)
         {
            // must have been fixed
            // do not catch exceptions
            double? result = pastFixing(fixingDate);
            Utils.QL_REQUIRE(result != null, () => "Missing " + name() + " fixing for " + fixingDate);
            return result.Value;
         }

         try
         {
            // might have been fixed
            double? result = pastFixing(fixingDate);
            if (result != null)
               return result.Value;

         }
         catch (Exception)
         {

         }
         return forecastFixing(fixingDate);
      }

      // Inspectors
      public string familyName() { return familyName_; }
      public Period tenor() { return tenor_; }
      public int fixingDays() { return fixingDays_; }
      public Date fixingDate(Date valueDate)
      {
         Date fixingDate = fixingCalendar().advance(valueDate, -fixingDays_, TimeUnit.Days);
         return fixingDate;
      }
      public Currency currency() { return currency_; }
      public DayCounter dayCounter() { return dayCounter_; }

      // Date calculations
      // These methods can be overridden to implement particular conventions (e.g. EurLibor) */
      public virtual Date valueDate(Date fixingDate)
      {
         Utils.QL_REQUIRE(isValidFixingDate(fixingDate), () => fixingDate + " is not a valid fixing date");
         return fixingCalendar().advance(fixingDate, fixingDays_, TimeUnit.Days);
      }
      public abstract Date maturityDate(Date valueDate);

      // Fixing calculations
      //! It can be overridden to implement particular conventions
      public abstract double forecastFixing(Date fixingDate);
      public virtual double? pastFixing(Date fixingDate)
      {
         Utils.QL_REQUIRE(isValidFixingDate(fixingDate), () => fixingDate + " is not a valid fixing date");
         if (timeSeries().ContainsKey(fixingDate))
            return timeSeries()[fixingDate];
         else
            return null;
      }


      protected string familyName_;
      protected Period tenor_;
      protected int fixingDays_;
      protected Currency currency_;
      protected DayCounter dayCounter_;
      protected string name_;

      private Calendar fixingCalendar_;


      // need by CashFlowVectors
      protected InterestRateIndex() { }
   }

}
