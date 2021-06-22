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
   /// <summary>
   /// This class provides a more comfortable way to instantiate standard cds.
   /// </summary>
   public class MakeCreditDefaultSwap
   {
      public MakeCreditDefaultSwap(Settings settings, Period tenor, double couponRate)
      {
         side_ = CreditDefaultSwap.Protection.Side.Buyer;
         nominal_ = 1.0;
         tenor_ = tenor;
         couponTenor_ = new Period(3, TimeUnit.Months);
         couponRate_ = couponRate;
         upfrontRate_ = 0.0;
         dayCounter_ = new Actual360();
         lastPeriodDayCounter_ = new Actual360(true);
         cashSettlementDays_ = 3;
         rule_ = DateGeneration.Rule.CDS;
         convention_ = BusinessDayConvention.Following;
         terminalConvention_ = BusinessDayConvention.Unadjusted;
         calendar_ = new WeekendsOnly();
         settings_ = settings;
      }

      public MakeCreditDefaultSwap(Settings settings, Date termDate, double couponRate)
      {
         side_ = CreditDefaultSwap.Protection.Side.Buyer;
         nominal_ = 1.0;
         termDate_ = termDate;
         couponTenor_ = new Period(3, TimeUnit.Months);
         couponRate_ = couponRate;
         upfrontRate_ = 0.0;
         dayCounter_ = new Actual360();
         lastPeriodDayCounter_ = new Actual360(true);
         cashSettlementDays_ = 3;
         rule_ = DateGeneration.Rule.CDS;
         convention_ = BusinessDayConvention.Following;
         terminalConvention_ = BusinessDayConvention.Unadjusted;
         calendar_ = new WeekendsOnly();
         settings_ = settings;
      }

      public CreditDefaultSwap value()
      {
         Date tradeDate = settings_.evaluationDate();
         Date upfrontDate = new WeekendsOnly().advance(tradeDate, new Period(cashSettlementDays_, TimeUnit.Days));
         
         Date protectionStart;
         if (rule_ == DateGeneration.Rule.CDS2015 || rule_ == DateGeneration.Rule.CDS)
         {
            protectionStart = tradeDate;
         }
         else
         {
            protectionStart = tradeDate + 1;
         }

         Date end;
         if (tenor_ != null)
         {
            if (rule_ == DateGeneration.Rule.CDS2015 || rule_ == DateGeneration.Rule.CDS || rule_ == DateGeneration.Rule.OldCDS) {
                end = Utils.cdsMaturity(tradeDate, tenor_, rule_);
            } else {
                end = tradeDate + tenor_;
            }
         }
         else
         {
            end = termDate_;
         }

         Schedule schedule = new Schedule(settings_, protectionStart, end, couponTenor_, calendar_,
                                          convention_, terminalConvention_, rule_,
                                          false);

         CreditDefaultSwap cds = new CreditDefaultSwap(side_, nominal_, upfrontRate_, couponRate_, schedule,
                                                       convention_, dayCounter_, true, true, protectionStart, upfrontDate, null, lastPeriodDayCounter_,
                                                       true, tradeDate, cashSettlementDays_);

         cds.setPricingEngine(engine_);
         return cds;
      }

      public MakeCreditDefaultSwap withCalendar(Calendar calendar)
      {
         calendar_ = calendar;
         return this;
      }

      public MakeCreditDefaultSwap withConvention(BusinessDayConvention convention)
      {
         convention_ = convention;
         return this;
      }

      public MakeCreditDefaultSwap withTerminalConvention(BusinessDayConvention convention)
      {
         terminalConvention_ = convention;
         return this;
      }

      public MakeCreditDefaultSwap withUpfrontRate(double upfrontRate)
      {
         upfrontRate_ = upfrontRate;
         return this;
      }

      public MakeCreditDefaultSwap withSide(CreditDefaultSwap.Protection.Side side)
      {
         side_ = side;
         return this;
      }

      public MakeCreditDefaultSwap withNominal(double nominal)
      {
         nominal_ = nominal;
         return this;
      }

      public MakeCreditDefaultSwap withCouponTenor(Period couponTenor)
      {
         couponTenor_ = couponTenor;
         return this;
      }

      public MakeCreditDefaultSwap withDayCounter(DayCounter dayCounter)
      {
         dayCounter_ = dayCounter;
         return this;
      }

      public MakeCreditDefaultSwap withLastPeriodDayCounter(DayCounter lastPeriodDayCounter)
      {
         lastPeriodDayCounter_ = lastPeriodDayCounter;
         return this;
      }

      public MakeCreditDefaultSwap withPricingEngine(IPricingEngine engine)
      {
         engine_ = engine;
         return this;
      }

      public MakeCreditDefaultSwap withRule(DateGeneration.Rule rule)
      {
         rule_ = rule;
         return this;
      }

      private CreditDefaultSwap.Protection.Side side_;
      double nominal_;
      Period tenor_;
      Date termDate_;
      Period couponTenor_;
      double couponRate_;
      double upfrontRate_;
      DayCounter dayCounter_;
      DayCounter lastPeriodDayCounter_;
      Settings settings_;
      IPricingEngine engine_;
      int cashSettlementDays_;
      DateGeneration.Rule rule_;
      BusinessDayConvention convention_, terminalConvention_;
      Calendar calendar_;
   }
}
