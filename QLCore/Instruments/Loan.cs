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
using System.Linq;

namespace QLCore
{
   public class Loan : Instrument
   {
      public enum Type { Deposit = -1, Loan = 1 }
      public enum Amortising
      {
         Bullet = 1,
         Step = 2,
         French = 3
      }
      protected List<List<CashFlow>> legs_;
      protected List<double> payer_;
      protected List<double> notionals_;
      protected List < double? > legNPV_;

      public Loan(Settings settings, int legs)
         : base(settings)
      {
         legs_ = new InitializedList<List<CashFlow>>(legs);
         payer_ = new InitializedList<double>(legs);
         notionals_ = new List<double>();
         legNPV_ = new InitializedList < double? >(legs);
      }

      ///////////////////////////////////////////////////////////////////
      // Instrument interface
      public override bool isExpired()
      {
         Date today = settings().evaluationDate();
         return !legs_.Any<List<CashFlow>>(leg => leg.Any<CashFlow>(cf => !cf.hasOccurred(today)));
      }

      protected override void setupExpired()
      {
         base.setupExpired();
         legNPV_ = new InitializedList < double? >(legNPV_.Count);
      }

      public override void setupArguments(IPricingEngineArguments args)
      {
         Loan.Arguments arguments = args as Loan.Arguments;
         if (arguments == null)
            throw new ArgumentException("wrong argument type");

         arguments.legs = legs_;
         arguments.payer = payer_;
         arguments.settings = settings_;
      }

      public override void fetchResults(IPricingEngineResults r)
      {
         base.fetchResults(r);

         Loan.Results results = r as Loan.Results;
         if (results == null)
            throw new ArgumentException("wrong result type");

         if (results.legNPV.Count != 0)
         {
            if (results.legNPV.Count != legNPV_.Count)
               throw new ArgumentException("wrong number of leg NPV returned");
            legNPV_ = new List < double? >(results.legNPV);
         }
         else
         {
            legNPV_ = new InitializedList < double? >(legNPV_.Count);
         }

      }

      ////////////////////////////////////////////////////////////////
      // arguments, results, pricing engine
      public class Arguments : IPricingEngineArguments
      {
         public List<List<CashFlow>> legs { get; set; }
         public List<double> payer { get; set; }
         public Settings settings { get; set; }

         public virtual void validate()
         {
            if (legs.Count != payer.Count)
               throw new ArgumentException("number of legs and multipliers differ");
         }
      }

      public new class Results : Instrument.Results
      {
         public List < double? > legNPV { get; set; }
         public override void reset()
         {
            base.reset();
            // clear all previous results
            if (legNPV == null)
               legNPV = new List < double? >();
            else
               legNPV.Clear();
         }
      }

      public class Engine : GenericEngine<Arguments, Results> { }

   }


   public class FixedLoan : Loan
   {
      private Type type_;
      private double nominal_;
      private Schedule fixedSchedule_;
      private double fixedRate_;
      private DayCounter fixedDayCount_;
      private Schedule principalSchedule_;
      private BusinessDayConvention paymentConvention_;

      public FixedLoan(Type type, double nominal,
                       Schedule fixedSchedule, double fixedRate, DayCounter fixedDayCount,
                       Schedule principalSchedule, BusinessDayConvention ? paymentConvention) :
      base(fixedSchedule.settings(), 2)
      {

         type_ = type;
         nominal_ = nominal;
         fixedSchedule_ = fixedSchedule;
         fixedRate_ = fixedRate;
         fixedDayCount_ = fixedDayCount;
         principalSchedule_ = principalSchedule;

         if (paymentConvention.HasValue)
            paymentConvention_ = paymentConvention.Value;
         else
            paymentConvention_ = fixedSchedule_.businessDayConvention();

         List<CashFlow> principalLeg = new PricipalLeg(principalSchedule, fixedDayCount)
         .withNotionals(nominal)
         .withPaymentAdjustment(paymentConvention_)
         .withSign(type == Type.Loan ? -1 : 1);

         // temporary
         for (int i = 0; i < principalLeg.Count - 1; i++)
         {
            Principal p = (Principal)principalLeg[i];
            notionals_.Add(p.nominal());
         }

         List<CashFlow> fixedLeg = new FixedRateLeg(fixedSchedule)
         .withCouponRates(fixedRate, fixedDayCount)
         .withPaymentAdjustment(paymentConvention_)
         .withNotionals(notionals_);


         legs_[0] = fixedLeg;
         legs_[1] = principalLeg;
         if (type_ == Type.Loan)
         {
            payer_[0] = +1;
            payer_[1] = -1;
         }
         else
         {
            payer_[0] = -1;
            payer_[1] = +1;
         }
      }

      public List<CashFlow> fixedLeg() { return legs_[0]; }
      public List<CashFlow> principalLeg() { return legs_[1]; }
   }

   public class FloatingLoan : Loan
   {
      private Type type_;
      private double nominal_;
      private Schedule floatingSchedule_;
      private double floatingSpread_;
      private DayCounter floatingDayCount_;
      private Schedule principalSchedule_;
      private BusinessDayConvention paymentConvention_;
      private IborIndex iborIndex_;

      public FloatingLoan(Type type, double nominal,
                          Schedule floatingSchedule, double floatingSpread, DayCounter floatingDayCount,
                          Schedule principalSchedule, BusinessDayConvention ? paymentConvention, IborIndex index) :
      base(floatingSchedule.settings(), 2)
      {

         type_ = type;
         nominal_ = nominal;
         floatingSchedule_ = floatingSchedule;
         floatingSpread_ = floatingSpread;
         floatingDayCount_ = floatingDayCount;
         principalSchedule_ = principalSchedule;
         iborIndex_ = index;

         if (paymentConvention.HasValue)
            paymentConvention_ = paymentConvention.Value;
         else
            paymentConvention_ = floatingSchedule_.businessDayConvention();

         List<CashFlow> principalLeg = new PricipalLeg(principalSchedule, floatingDayCount)
         .withNotionals(nominal)
         .withPaymentAdjustment(paymentConvention_)
         .withSign(type == Type.Loan ? -1 : 1);

         // temporary
         for (int i = 0; i < principalLeg.Count - 1; i++)
         {
            Principal p = (Principal)principalLeg[i];
            notionals_.Add(p.nominal());
         }

         List<CashFlow> floatingLeg = new IborLeg(floatingSchedule, iborIndex_)
         .withPaymentDayCounter(floatingDayCount_)
         .withSpreads(floatingSpread_)
         .withPaymentAdjustment(paymentConvention_)
         .withNotionals(notionals_);


         legs_[0] = floatingLeg;
         legs_[1] = principalLeg;
         if (type_ == Type.Loan)
         {
            payer_[0] = -1;
            payer_[1] = +1;
         }
         else
         {
            payer_[0] = +1;
            payer_[1] = -1;
         }
      }

      public List<CashFlow> floatingLeg() { return legs_[0]; }
      public List<CashFlow> principalLeg() { return legs_[1]; }
   }

   public class CommercialPaper : Loan
   {
      private Type type_;
      private double nominal_;
      private Schedule fixedSchedule_;
      private double fixedRate_;
      private DayCounter fixedDayCount_;
      private Schedule principalSchedule_;
      private BusinessDayConvention paymentConvention_;

      public CommercialPaper(Type type, double nominal,
                             Schedule fixedSchedule, double fixedRate, DayCounter fixedDayCount,
                             Schedule principalSchedule, BusinessDayConvention ? paymentConvention) :
      base(fixedSchedule.settings(), 2)
      {

         type_ = type;
         nominal_ = nominal;
         fixedSchedule_ = fixedSchedule;
         fixedRate_ = fixedRate;
         fixedDayCount_ = fixedDayCount;
         principalSchedule_ = principalSchedule;

         if (paymentConvention.HasValue)
            paymentConvention_ = paymentConvention.Value;
         else
            paymentConvention_ = fixedSchedule_.businessDayConvention();

         List<CashFlow> principalLeg = new PricipalLeg(principalSchedule, fixedDayCount)
         .withNotionals(nominal)
         .withPaymentAdjustment(paymentConvention_)
         .withSign(type == Type.Loan ? -1 : 1);

         // temporary
         for (int i = 0; i < principalLeg.Count - 1; i++)
         {
            Principal p = (Principal)principalLeg[i];
            notionals_.Add(p.nominal());
         }

         List<CashFlow> fixedLeg = new FixedRateLeg(fixedSchedule)
         .withCouponRates(fixedRate, fixedDayCount)
         .withPaymentAdjustment(paymentConvention_)
         .withNotionals(notionals_);

         // Discounting Pricipal
         notionals_.Clear();
         double n;
         for (int i = 0; i < fixedLeg.Count ; i++)
         {
            FixedRateCoupon c = (FixedRateCoupon)fixedLeg[i];
            n = i > 0 ? notionals_.Last() : c.nominal();
            notionals_.Add(n / (1 + (c.rate()* c.dayCounter().yearFraction(c.referencePeriodStart, c.referencePeriodEnd))));
         }

         // New Leg
         List<CashFlow> discountedFixedLeg = new FixedRateLeg(fixedSchedule)
         .withCouponRates(fixedRate, fixedDayCount)
         .withPaymentAdjustment(paymentConvention_)
         .withNotionals(notionals_);
         // Adjust Principal
         Principal p0 = (Principal)principalLeg[0];
         p0.setAmount(notionals_.Last());

         legs_[0] = discountedFixedLeg;
         legs_[1] = principalLeg;
         if (type_ == Type.Loan)
         {
            payer_[0] = +1;
            payer_[1] = -1;
         }
         else
         {
            payer_[0] = -1;
            payer_[1] = +1;
         }
      }

      public List<CashFlow> fixedLeg() { return legs_[0]; }
      public List<CashFlow> principalLeg() { return legs_[1]; }
   }

   public class Cash : Loan
   {
      private Type type_;
      private double nominal_;
      private Schedule principalSchedule_;
      private BusinessDayConvention paymentConvention_;

      public Cash(Type type, double nominal,
                  Schedule principalSchedule, BusinessDayConvention ? paymentConvention) :
      base(principalSchedule.settings(), 1)
      {

         type_ = type;
         nominal_ = nominal;
         principalSchedule_ = principalSchedule;
         paymentConvention_ = paymentConvention.Value;

         List<CashFlow> principalLeg = new PricipalLeg(principalSchedule, new Actual365Fixed())
         .withNotionals(nominal)
         .withPaymentAdjustment(paymentConvention_)
         .withSign(type == Type.Loan ? -1 : 1);

         legs_[0] = principalLeg;
         if (type_ == Type.Loan)
         {
            payer_[0] = +1;
         }
         else
         {
            payer_[0] = -1;
         }
      }

      public List<CashFlow> principalLeg() { return legs_[0]; }
   }

}
