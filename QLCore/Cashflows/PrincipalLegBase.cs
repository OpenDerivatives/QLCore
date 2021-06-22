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

using System.Collections.Generic;

namespace QLCore
{
   public abstract class PrincipalLegBase
   {
      protected Schedule schedule_;
      protected List<double> notionals_;
      protected BusinessDayConvention paymentAdjustment_;
      protected DayCounter dayCounter_;
      protected int sign_;
      public static implicit operator List<CashFlow>(PrincipalLegBase o) { return o.value(); }
      public abstract List<CashFlow> value();

      // initializers
      public PrincipalLegBase withNotionals(double notional)
      {
         notionals_ = new List<double>() { notional };
         return this;
      }

      public PrincipalLegBase withNotionals(List<double> notionals)
      {
         notionals_ = notionals;
         return this;
      }

      public PrincipalLegBase withPaymentAdjustment(BusinessDayConvention convention)
      {
         paymentAdjustment_ = convention;
         return this;
      }

      public PrincipalLegBase withPaymentDayCounter(DayCounter dayCounter)
      {
         dayCounter_ = dayCounter;
         return this;
      }

      public PrincipalLegBase withSign(int sign)
      {
         sign_ = sign;
         return this;
      }
   }

   //! helper class building a Bullet Principal leg
   public class PricipalLeg : PrincipalLegBase
   {
      // constructor
      public PricipalLeg(Schedule schedule, DayCounter paymentDayCounter)
      {
         schedule_ = schedule;
         paymentAdjustment_ = BusinessDayConvention.Following;
         dayCounter_ = paymentDayCounter;
      }

      // creator
      public override List<CashFlow> value()
      {
         List<CashFlow> leg = new List<CashFlow>();

         // the following is not always correct
         Calendar calendar = schedule_.calendar();

         // first period
         Date start = schedule_[0], end = schedule_[schedule_.Count - 1];
         Date paymentDate = calendar.adjust(start, paymentAdjustment_);
         double nominal = notionals_[0];
         double quota = nominal / (schedule_.Count - 1);

         leg.Add(new Principal(schedule_.settings(), nominal * sign_, nominal, paymentDate, start, end, dayCounter_,  start, end));

         if (schedule_.Count == 2)
         {
            paymentDate = calendar.adjust(end, paymentAdjustment_);
            leg.Add(new Principal(schedule_.settings(), nominal * sign_ * -1, 0, paymentDate, start, end, dayCounter_, start, end));
         }
         else
         {
            end = schedule_[0];
            // regular periods
            for (int i = 1; i <= schedule_.Count - 1; ++i)
            {
               start = end; end = schedule_[i];
               paymentDate = calendar.adjust(start, paymentAdjustment_);
               nominal -= quota;

               leg.Add(new Principal(schedule_.settings(), quota * sign_ * -1, nominal, paymentDate, start, end, dayCounter_, start, end));
            }
         }

         return leg;
      }
   }

   //! helper class building a Bullet Principal leg
   public class BulletPricipalLeg : PrincipalLegBase
   {
      // constructor
      public BulletPricipalLeg(Schedule schedule)
      {
         schedule_ = schedule;
         paymentAdjustment_ = BusinessDayConvention.Following;
      }

      // creator
      public override List<CashFlow> value()
      {
         List<CashFlow> leg = new List<CashFlow>();

         // the following is not always correct
         Calendar calendar = schedule_.calendar();

         // first period might be short or long
         Date start = schedule_[0], end = schedule_[1];
         Date paymentDate = calendar.adjust(start, paymentAdjustment_);
         double nominal = notionals_[0];

         leg.Add(new Principal(schedule_.settings(), nominal, nominal, paymentDate, start, end, dayCounter_, start, end));

         paymentDate = calendar.adjust(end, paymentAdjustment_);
         leg.Add(new Principal(schedule_.settings(), nominal * -1, 0, paymentDate, start, end, dayCounter_, start, end));

         return leg;
      }
   }
}
