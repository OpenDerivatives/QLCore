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
   public class BondFactory
   {
      public static AmortizingBond makeAmortizingBond(Settings settings,
                                                      double FaceValue,
                                                      double MarketValue,
                                                      double CouponRate,
                                                      Date IssueDate,
                                                      Date MaturityDate,
                                                      Date TradeDate,
                                                      Frequency payFrequency,
                                                      DayCounter dCounter,
                                                      AmortizingMethod Method,
                                                      double gYield = 0)
      {
         return new AmortizingBond(settings,
                                   FaceValue,
                                   MarketValue,
                                   CouponRate,
                                   IssueDate,
                                   MaturityDate,
                                   TradeDate,
                                   payFrequency,
                                   dCounter,
                                   Method,
                                   new NullCalendar(), gYield);
      }


      public static AmortizingFixedRateBond makeAmortizingFixedBond(Settings settings,
                                                                    Date startDate,
                                                                    Period bondLength,
                                                                    DayCounter dCounter,
                                                                    Frequency payFrequency,
                                                                    double amount,
                                                                    double rate)
      {
         return makeAmortizingFixedBond(settings, startDate, bondLength, dCounter, payFrequency, amount, rate, new TARGET());
      }


      public static AmortizingFixedRateBond makeAmortizingFixedBond(Settings settings,
                                                                    Date startDate,
                                                                    Period bondLength,
                                                                    DayCounter dCounter,
                                                                    Frequency payFrequency,
                                                                    double amount,
                                                                    double rate,
                                                                    Calendar calendar)
      {
         AmortizingFixedRateBond bond;
         Date endDate = calendar.advance(startDate, bondLength);

         Schedule schedule = new Schedule(settings, startDate, endDate, bondLength, calendar, BusinessDayConvention.Unadjusted,
                                          BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, false);

         bond = new AmortizingFixedRateBond(settings, 0, calendar, amount, startDate, bondLength, payFrequency, rate, dCounter);

         return bond;

      }

      public static MBSFixedRateBond makeMBSFixedBond(Settings settings,
                                                      Date startDate,
                                                      Period bondLength,
                                                      Period originalLength,
                                                      DayCounter dCounter,
                                                      Frequency payFrequency,
                                                      double amount,
                                                      double WACRate,
                                                      double PassThroughRate,
                                                      PSACurve psaCurve)
      {
         return makeMBSFixedBond(settings, startDate, bondLength, originalLength, dCounter, payFrequency, amount, WACRate, PassThroughRate, psaCurve, new TARGET());
      }


      public static MBSFixedRateBond makeMBSFixedBond(Settings settings,
                                                      Date startDate,
                                                      Period bondLength,
                                                      Period originalLength,
                                                      DayCounter dCounter,
                                                      Frequency payFrequency,
                                                      double amount,
                                                      double WACrate,
                                                      double PassThroughRate,
                                                      PSACurve psaCurve,
                                                      Calendar calendar)
      {
         MBSFixedRateBond bond;
         Date endDate = calendar.advance(startDate, bondLength);

         Schedule schedule = new Schedule(settings, startDate, endDate, bondLength, calendar, BusinessDayConvention.Unadjusted,
                                          BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, false);

         bond = new MBSFixedRateBond(settings, 0, calendar, amount, startDate, bondLength, originalLength, payFrequency, WACrate, PassThroughRate, dCounter, psaCurve);

         return bond;

      }
   }
}
