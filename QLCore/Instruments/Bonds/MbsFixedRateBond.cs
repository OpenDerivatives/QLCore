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
using System.Collections.Generic;

namespace QLCore
{
   public class MBSFixedRateBond : AmortizingFixedRateBond
   {
      public MBSFixedRateBond(Settings settings,
                              int settlementDays,
                              Calendar calendar,
                              double faceAmount,
                              Date startDate,
                              Period bondTenor,
                              Period originalLength,
                              Frequency sinkingFrequency,
                              double WACRate,
                              double PassThroughRate,
                              DayCounter accrualDayCounter,
                              IPrepayModel prepayModel,
                              BusinessDayConvention paymentConvention = BusinessDayConvention.Following,
                              Date issueDate = null)
         : base(settings, settlementDays, calendar, faceAmount, startDate, bondTenor, sinkingFrequency, WACRate, accrualDayCounter, paymentConvention, issueDate)
      {
         prepayModel_ = prepayModel;
         originalLength_ = originalLength;
         remainingLength_ = bondTenor;
         WACRate_ = WACRate;
         PassThroughRate_ = PassThroughRate;
         dCounter_ = accrualDayCounter;
         cashflows_ = expectedCashflows();
      }

      public List<CashFlow> expectedCashflows()
      {
         calcBondFactor();

         List<CashFlow> expectedcashflows = new List<CashFlow>();

         List<double> notionals = new InitializedList<double>(schedule_.Count);
         notionals[0] = notionals_[0];
         for (int i = 0; i < schedule_.Count - 1; ++i)
         {
            double currentNotional = notionals[i];
            double smm = SMM(schedule_[i]);
            double prepay = (notionals[i] * bondFactors_[i + 1]) / bondFactors_[i] * smm;
            double actualamort = currentNotional * (1 - bondFactors_[i + 1] / bondFactors_[i]);
            notionals[i + 1] = currentNotional - actualamort - prepay;

            // ADD
            CashFlow c1 = new VoluntaryPrepay(settings(), prepay, schedule_[i + 1]);
            CashFlow c2 = new AmortizingPayment(settings(), actualamort, schedule_[i + 1]);
            CashFlow c3 = new FixedRateCoupon(settings(), schedule_[i + 1], currentNotional, new InterestRate(PassThroughRate_, dCounter_, Compounding.Simple, Frequency.Annual), schedule_[i], schedule_[i + 1]);
            expectedcashflows.Add(c1);
            expectedcashflows.Add(c2);
            expectedcashflows.Add(c3);

         }
         notionals[notionals.Count - 1] = 0.0;

         return expectedcashflows;
      }

      public double SMM(Date d)
      {
         if (prepayModel_ != null)
         {
            return prepayModel_.getSMM(d + (originalLength_ - remainingLength_));
         }
         else
            return 0;
      }

      public double MonthlyYield()
      {
         Brent solver = new Brent();
         solver.setMaxEvaluations(100);
         List<CashFlow> cf = expectedCashflows();

         MonthlyYieldFinder objective = new MonthlyYieldFinder(settings(), notional(settlementDate()), cf, settlementDate());
         return solver.solve(objective, 1.0e-10, 0.02, 0.0, 1.0) / 100 ;
      }

      public double BondEquivalentYield()
      {
         return 2 * (Math.Pow(1 + MonthlyYield(), 6) - 1);
      }

      protected void calcBondFactor()
      {
         bondFactors_ = new InitializedList<double>(notionals_.Count);
         for (int i = 0 ; i < notionals_.Count ; i++)
         {
            if (i == 0)
               bondFactors_[i] = 1;
            else
               bondFactors_[i] = notionals_[i] / notionals_[0];
         }
      }

      public List<double> BondFactors() { if (bondFactors_ == null) calcBondFactor(); return bondFactors_; }

      protected List<double> bondFactors_;
      protected IPrepayModel prepayModel_;
      protected Period originalLength_, remainingLength_;
      protected double WACRate_;
      protected double PassThroughRate_;
      protected DayCounter dCounter_;

   }

   public class MonthlyYieldFinder : ISolver1d
   {
      private double faceAmount_;
      private List<CashFlow> cashflows_;
      private Date settlement_;
      private Settings settings_;

      public MonthlyYieldFinder(Settings settings, double faceAmount, List<CashFlow> cashflows, Date settlement)
      {
         faceAmount_ = faceAmount;
         cashflows_ = cashflows;
         settlement_ = settlement;
         settings_ = settings;
      }

      public override double value(double yield)
      {
         return Utils.PVDifference(settings_, faceAmount_, cashflows_, yield, settlement_);
      }
   }


   public partial class Utils
   {
      public static double PVDifference(Settings settings, double faceAmount, List<CashFlow> cashflows, double yield, Date settlement)
      {
         double price = 0.0;
         Date actualDate = new Date(1, 1, 1970) ;
         int cashflowindex = 0 ;


         for (int i = 0; i < cashflows.Count; i++)
         {
            if (cashflows[i].hasOccurred(settlement))
               continue;
            // TODO use daycounter to find cashflowindex
            if (cashflows[i].date() != actualDate)
            {
               actualDate = cashflows[i].date();
               cashflowindex++;
            }
            double amount = cashflows[i].amount();
            price += amount / Math.Pow((1 + yield / 100), cashflowindex);
         }

         return price - faceAmount;


      }
   }
}
