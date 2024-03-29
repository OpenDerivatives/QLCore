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
   public class MakeFixedLoan
   {
      private double nominal_;
      private Calendar calendar_;
      private Date startDate_, endDate_;
      private Frequency frequency_;
      private BusinessDayConvention convention_ ;
      private DayCounter dayCounter_;
      private double fixedRate_;
      private Loan.Type type_;
      private Loan.Amortising amortising_;
      private DateGeneration.Rule rule_;
      private bool endOfMonth_;
      private Settings settings_;

      public MakeFixedLoan(Date startDate, Date endDate, double fixedRate, Frequency frequency)
      {
         startDate_ = startDate;
         endDate_ = endDate;
         fixedRate_ = fixedRate;
         frequency_ = frequency;

         type_ = Loan.Type.Loan;
         amortising_ = Loan.Amortising.Bullet;
         nominal_ = 1.0;
         calendar_ = new TARGET();
         convention_ = BusinessDayConvention.ModifiedFollowing;
         dayCounter_ = new Actual365Fixed();
         rule_ = DateGeneration.Rule.Forward;
         endOfMonth_ = false;
         settings_ = new Settings();
      }

      public MakeFixedLoan withType(Loan.Type type)
      {
         type_ = type;
         return this;
      }

      public MakeFixedLoan withNominal(double n)
      {
         nominal_ = n;
         return this;
      }

      public MakeFixedLoan withCalendar(Calendar c)
      {
         calendar_ = c;
         return this;
      }

      public MakeFixedLoan withConvention(BusinessDayConvention bdc)
      {
         convention_ = bdc;
         return this;
      }

      public MakeFixedLoan withDayCounter(DayCounter dc)
      {
         dayCounter_ = dc;
         return this;
      }

      public MakeFixedLoan withRule(DateGeneration.Rule r)
      {
         rule_ = r;
         return this;
      }

      public MakeFixedLoan withEndOfMonth(bool flag)
      {
         endOfMonth_ = flag;
         return this;
      }

      public MakeFixedLoan withAmortising(Loan.Amortising Amortising)
      {
         amortising_ = Amortising;
         return this;
      }

      public MakeFixedLoan withSettings(Settings s)
      {
         settings_ = s;
         return this;
      }

      // Loan creator
      public static implicit operator FixedLoan(MakeFixedLoan o) { return o.value(); }

      public FixedLoan value()
      {

         Schedule fixedSchedule = new Schedule(settings_, startDate_, endDate_, new Period(frequency_),
                                               calendar_, convention_, convention_, rule_, endOfMonth_);

         Period principalPeriod = amortising_ == Loan.Amortising.Bullet ?
                                  new Period(Frequency.Once) :
                                  new Period(frequency_);

         Schedule principalSchedule = new Schedule(settings_, startDate_, endDate_, principalPeriod,
                                                   calendar_, convention_, convention_, rule_, endOfMonth_);

         FixedLoan fl = new FixedLoan(type_, nominal_, fixedSchedule, fixedRate_, dayCounter_,
                                      principalSchedule, convention_);
         return fl;

      }

   }

   public class MakeFloatingLoan
   {
      private double nominal_;
      private Calendar calendar_;
      private Date startDate_, endDate_;
      private Frequency frequency_;
      private BusinessDayConvention convention_;
      private DayCounter dayCounter_;
      private double spread_;
      private Loan.Type type_;
      private Loan.Amortising amortising_;
      private DateGeneration.Rule rule_;
      private bool endOfMonth_;
      private IborIndex index_;
      private Settings settings_;

      public MakeFloatingLoan(Date startDate, Date endDate, double spread, Frequency frequency)
      {
         startDate_ = startDate;
         endDate_ = endDate;
         spread_ = spread;
         frequency_ = frequency;

         type_ = Loan.Type.Loan;
         amortising_ = Loan.Amortising.Bullet;
         nominal_ = 1.0;
         calendar_ = new TARGET();
         convention_ = BusinessDayConvention.ModifiedFollowing;
         dayCounter_ = new Actual365Fixed();
         rule_ = DateGeneration.Rule.Forward;
         endOfMonth_ = false;
         index_ = new IborIndex();
         settings_ = new Settings();
      }

      public MakeFloatingLoan withType(Loan.Type type)
      {
         type_ = type;
         return this;
      }

      public MakeFloatingLoan withNominal(double n)
      {
         nominal_ = n;
         return this;
      }

      public MakeFloatingLoan withCalendar(Calendar c)
      {
         calendar_ = c;
         return this;
      }

      public MakeFloatingLoan withConvention(BusinessDayConvention bdc)
      {
         convention_ = bdc;
         return this;
      }

      public MakeFloatingLoan withDayCounter(DayCounter dc)
      {
         dayCounter_ = dc;
         return this;
      }

      public MakeFloatingLoan withRule(DateGeneration.Rule r)
      {
         rule_ = r;
         return this;
      }

      public MakeFloatingLoan withEndOfMonth(bool flag)
      {
         endOfMonth_ = flag;
         return this;
      }

      public MakeFloatingLoan withAmortising(Loan.Amortising Amortising)
      {
         amortising_ = Amortising;
         return this;
      }

      public MakeFloatingLoan withIndex(IborIndex index)
      {
         index_ = index;
         return this;
      }

      public MakeFloatingLoan withSettings(Settings s)
      {
         settings_ = s;
         return this;
      }

      // Loan creator
      public static implicit operator FloatingLoan(MakeFloatingLoan o) { return o.value(); }

      public FloatingLoan value()
      {

         Schedule floatingSchedule = new Schedule(settings_, startDate_, endDate_, new Period(frequency_),
                                                  calendar_, convention_, convention_, rule_, endOfMonth_);

         Period principalPeriod = amortising_ == Loan.Amortising.Bullet ?
                                  new Period(Frequency.Once) :
                                  new Period(frequency_);

         Schedule principalSchedule = new Schedule(settings_, startDate_, endDate_, principalPeriod,
                                                   calendar_, convention_, convention_, rule_, endOfMonth_);

         FloatingLoan fl = new FloatingLoan(type_, nominal_, floatingSchedule, spread_, dayCounter_,
                                            principalSchedule, convention_, index_);
         return fl;

      }

   }

   public class MakeCommercialPaper
   {
      private double nominal_;
      private Calendar calendar_;
      private Date startDate_, endDate_;
      private Frequency frequency_;
      private BusinessDayConvention convention_;
      private DayCounter dayCounter_;
      private double fixedRate_;
      private Loan.Type type_;
      private Loan.Amortising amortising_;
      private DateGeneration.Rule rule_;
      private bool endOfMonth_;
      private Settings settings_;

      public MakeCommercialPaper(Date startDate, Date endDate, double fixedRate, Frequency frequency)
      {
         startDate_ = startDate;
         endDate_ = endDate;
         fixedRate_ = fixedRate;
         frequency_ = frequency;

         type_ = Loan.Type.Loan;
         amortising_ = Loan.Amortising.Bullet;
         nominal_ = 1.0;
         calendar_ = new TARGET();
         convention_ = BusinessDayConvention.ModifiedFollowing;
         dayCounter_ = new Actual365Fixed();
         rule_ = DateGeneration.Rule.Forward;
         endOfMonth_ = false;
         settings_ = new Settings();
      }

      public MakeCommercialPaper withType(Loan.Type type)
      {
         type_ = type;
         return this;
      }

      public MakeCommercialPaper withNominal(double n)
      {
         nominal_ = n;
         return this;
      }

      public MakeCommercialPaper withCalendar(Calendar c)
      {
         calendar_ = c;
         return this;
      }

      public MakeCommercialPaper withConvention(BusinessDayConvention bdc)
      {
         convention_ = bdc;
         return this;
      }

      public MakeCommercialPaper withDayCounter(DayCounter dc)
      {
         dayCounter_ = dc;
         return this;
      }

      public MakeCommercialPaper withRule(DateGeneration.Rule r)
      {
         rule_ = r;
         return this;
      }

      public MakeCommercialPaper withEndOfMonth(bool flag)
      {
         endOfMonth_ = flag;
         return this;
      }

      public MakeCommercialPaper withAmortising(Loan.Amortising Amortising)
      {
         amortising_ = Amortising;
         return this;
      }

      public MakeCommercialPaper withSettings(Settings s)
      {
         settings_ = s;
         return this;
      }

      // Loan creator
      public static implicit operator CommercialPaper(MakeCommercialPaper o) { return o.value(); }

      public CommercialPaper value()
      {

         Schedule fixedSchedule = new Schedule(settings_, startDate_, endDate_, new Period(frequency_),
                                               calendar_, convention_, convention_, rule_, endOfMonth_);

         Period principalPeriod = amortising_ == Loan.Amortising.Bullet ?
                                  new Period(Frequency.Once) :
                                  new Period(frequency_);

         Schedule principalSchedule = new Schedule(settings_, startDate_, endDate_, principalPeriod,
                                                   calendar_, convention_, convention_, rule_, endOfMonth_);

         CommercialPaper fl = new CommercialPaper(type_, nominal_, fixedSchedule, fixedRate_, dayCounter_,
                                                  principalSchedule, convention_);
         return fl;

      }
   }

   public class MakeCash
   {
      private double nominal_;
      private Calendar calendar_;
      private Date startDate_, endDate_;
      private Frequency frequency_;
      private BusinessDayConvention convention_;
      private DayCounter dayCounter_;
      private Loan.Type type_;
      private Loan.Amortising amortising_;
      private DateGeneration.Rule rule_;
      private bool endOfMonth_;
      private Settings settings_;

      public MakeCash(Date startDate, Date endDate, double nominal)
      {
         startDate_ = startDate;
         endDate_ = endDate;
         nominal_ = nominal;

         frequency_  = Frequency.Once;
         type_ = Loan.Type.Loan;
         amortising_ = Loan.Amortising.Bullet;
         calendar_ = new TARGET();
         convention_ = BusinessDayConvention.ModifiedFollowing;
         dayCounter_ = new Actual365Fixed();
         rule_ = DateGeneration.Rule.Forward;
         endOfMonth_ = false;
         settings_ = new Settings();
      }

      public MakeCash withType(Loan.Type type)
      {
         type_ = type;
         return this;
      }

      public MakeCash withCalendar(Calendar c)
      {
         calendar_ = c;
         return this;
      }

      public MakeCash withConvention(BusinessDayConvention bdc)
      {
         convention_ = bdc;
         return this;
      }

      public MakeCash withDayCounter(DayCounter dc)
      {
         dayCounter_ = dc;
         return this;
      }

      public MakeCash withRule(DateGeneration.Rule r)
      {
         rule_ = r;
         return this;
      }

      public MakeCash withEndOfMonth(bool flag)
      {
         endOfMonth_ = flag;
         return this;
      }

      public MakeCash withAmortising(Loan.Amortising Amortising)
      {
         amortising_ = Amortising;
         return this;
      }

      public MakeCash withSettings(Settings s)
      {
         settings_ = s;
         return this;
      }

      // Loan creator
      public static implicit operator Cash(MakeCash o) { return o.value(); }

      public Cash value()
      {

         Period principalPeriod = amortising_ == Loan.Amortising.Bullet ?
                                  new Period(Frequency.Once) :
                                  new Period(frequency_);

         Schedule principalSchedule = new Schedule(settings_, startDate_, endDate_, principalPeriod,
                                                   calendar_, convention_, convention_, rule_, endOfMonth_);

         Cash c = new Cash(type_, nominal_, principalSchedule, convention_);
         return c;

      }

   }

}
