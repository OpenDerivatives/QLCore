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
   //! %principal payment over a fixed period
   //! This class implements part of the CashFlow interface but it is
   //  still abstract and provides derived classes with methods for accrual period calculations.
   public class Principal : CashFlow
   {
      protected double nominal_;
      protected double amount_;
      protected DayCounter dayCounter_;
      protected Date paymentDate_, accrualStartDate_, accrualEndDate_, refPeriodStart_, refPeriodEnd_;

      // access to properties
      public double nominal() { return nominal_; }
      public override Date date() { return paymentDate_; }
      public Date accrualStartDate() { return accrualStartDate_; }
      public Date accrualEndDate() { return accrualEndDate_; }
      public Date refPeriodStart { get { return refPeriodStart_; } }
      public Date refPeriodEnd { get { return refPeriodEnd_; } }
      public override double amount() { return amount_; }
      public void setAmount(double amount) { amount_ = amount; }
      public DayCounter dayCounter() { return dayCounter_; }

      // Constructors
      public Principal(Settings settings) : base(settings) { }       // default constructor
      public Principal(Settings settings,
                       double amount,
                       double nominal,
                       Date paymentDate,
                       Date accrualStartDate,
                       Date accrualEndDate,
                       DayCounter dayCounter,
                       Date refPeriodStart = null,
                       Date refPeriodEnd = null)
         : base(settings)
      {
         amount_ = amount;
         nominal_ = nominal;
         paymentDate_ = paymentDate;
         accrualStartDate_ = accrualStartDate;
         accrualEndDate_ = accrualEndDate;
         refPeriodStart_ = refPeriodStart;
         refPeriodEnd_ = refPeriodEnd;
         dayCounter_ = dayCounter;
         if (refPeriodStart_ == null)
            refPeriodStart_ = accrualStartDate_;
         if (refPeriodEnd_ == null)
            refPeriodEnd_ = accrualEndDate_;
      }
   }
}
