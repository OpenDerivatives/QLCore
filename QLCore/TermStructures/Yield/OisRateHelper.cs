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
   public class OISRateHelper : RelativeDateRateHelper
   {
      public OISRateHelper(int settlementDays,
                           Period tenor, // swap maturity
                           Handle<Quote> fixedRate,
                           OvernightIndex overnightIndex)
         : base(fixedRate)
      {
         settlementDays_ = settlementDays;
         tenor_ = tenor;
         overnightIndex_ = overnightIndex;
         fixedFrequency_ = Frequency.Annual;
         oisFrequency_ = Frequency.Annual;

         initializeDates();
      }

      public OISRateHelper(int settlementDays,
                           Period tenor, // swap maturity
                           Handle<Quote> fixedRate,
                           OvernightIndex overnightIndex,
                           Frequency fixedFrequency,
                           Frequency oisFrequency)
         : base(fixedRate)
      {
         settlementDays_ = settlementDays;
         tenor_ = tenor;
         overnightIndex_ = overnightIndex;
         fixedFrequency_ = fixedFrequency;
         oisFrequency_ = oisFrequency;
         
         initializeDates();
      }

      public OvernightIndexedSwap swap() { return swap_; }

      protected override void initializeDates()
      {

         // dummy OvernightIndex with curve/swap arguments
         // review here
         IborIndex clonedIborIndex = overnightIndex_.clone(termStructureHandle_);
         OvernightIndex clonedOvernightIndex = clonedIborIndex as OvernightIndex;

         swap_ = new MakeOIS(tenor_, clonedOvernightIndex, 0.0)
         .withPaymentFrequency(fixedFrequency_)
         .withReceiveFrequency(oisFrequency_)
         .withSettlementDays(settlementDays_)
         .withDiscountingTermStructure(termStructureHandle_);

         earliestDate_ = swap_.startDate();
         latestDate_ = swap_.maturityDate();
      }

      public override void setTermStructure(YieldTermStructure t)
      {
         // no need to register---the index is not lazy
         termStructureHandle_.linkTo(t);
         base.setTermStructure(t);
      }

      public override double impliedQuote()
      {
         if (termStructure_ == null)
            throw new ArgumentException("term structure not set");

         // we didn't register as observers - force calculation
         swap_.recalculate();
         return swap_.fairRate().Value;
      }

      protected int settlementDays_;
      protected Period tenor_;
      protected OvernightIndex overnightIndex_;
      protected OvernightIndexedSwap swap_;
      protected Frequency fixedFrequency_, oisFrequency_;
      protected RelinkableHandle<YieldTermStructure> termStructureHandle_ = new RelinkableHandle<YieldTermStructure>();
   }


   //! Rate helper for bootstrapping over Overnight Indexed Swap rates
   public class DatedOISRateHelper : RateHelper
   {

      public DatedOISRateHelper(Date startDate,
                                Date endDate,
                                Handle<Quote> fixedRate,
                                OvernightIndex overnightIndex)

         : base(fixedRate)
      {
         // dummy OvernightIndex with curve/swap arguments
         // review here
         IborIndex clonedIborIndex = overnightIndex.clone(termStructureHandle_);
         OvernightIndex clonedOvernightIndex = clonedIborIndex as OvernightIndex;

         swap_ = new MakeOIS(new Period(), clonedOvernightIndex, 0.0)
         .withEffectiveDate(startDate)
         .withTerminationDate(endDate)
         .withDiscountingTermStructure(termStructureHandle_);

         earliestDate_ = swap_.startDate();
         latestDate_ = swap_.maturityDate();

      }


      public override void setTermStructure(YieldTermStructure t)
      {
         // no need to register---the index is not lazy
         termStructureHandle_.linkTo(t);
         base.setTermStructure(t);

      }

      public override double impliedQuote()
      {
         if (termStructure_ == null)
            throw new ArgumentException("term structure not set");

         // we didn't register as observers - force calculation
         swap_.recalculate();
         return swap_.fairRate().Value;
      }

      protected OvernightIndexedSwap swap_;
      protected RelinkableHandle<YieldTermStructure> termStructureHandle_ = new RelinkableHandle<YieldTermStructure>();
   }
}
