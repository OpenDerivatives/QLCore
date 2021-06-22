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
   public class DiscountingLoanEngine : Loan.Engine
   {
      private readonly Handle<YieldTermStructure> discountCurve_;
      private readonly bool? includeSettlementDateFlows_;

      public Handle<YieldTermStructure> discountCurve() { return discountCurve_; }

      public DiscountingLoanEngine(Handle<YieldTermStructure> discountCurve, bool? includeSettlementDateFlows = null)
      {
         discountCurve_ = discountCurve;
         includeSettlementDateFlows_ = includeSettlementDateFlows;
      }

      public override void calculate()
      {
         QLCore.Utils.QL_REQUIRE(!discountCurve_.empty(), () => "discounting term structure handle is empty");

         results_.valuationDate = discountCurve_.link.referenceDate();
         bool includeRefDateFlows =
            includeSettlementDateFlows_.HasValue ?
            includeSettlementDateFlows_.Value :
            discountCurve_.currentLink().settings().includeReferenceDateEvents;

         results_.value = 0;
         results_.cash = 0;
         for (int i = 0; i < arguments_.legs.Count; ++i)
         {
            results_.value += CashFlows.npv(arguments_.legs[i],
                                            discountCurve_,
                                            includeRefDateFlows,
                                            results_.valuationDate,
                                            results_.valuationDate)
                              * arguments_.payer[i];

            results_.cash += CashFlows.cash(arguments_.legs[i], results_.valuationDate)
                             * arguments_.payer[i];
         }
      }
   }
}
