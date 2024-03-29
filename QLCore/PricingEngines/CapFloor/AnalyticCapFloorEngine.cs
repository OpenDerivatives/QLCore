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

namespace QLCore
{

   public class AnalyticCapFloorEngine : GenericModelEngine<IAffineModel,
      CapFloor.Arguments,
      CapFloor.Results>
   {

      /*! \note the term structure is only needed when the short-rate
                model cannot provide one itself.
      */

      private Handle<YieldTermStructure> termStructure_;

      public AnalyticCapFloorEngine(IAffineModel model)
         : this(model, new Handle<YieldTermStructure>()) { }


      public AnalyticCapFloorEngine(IAffineModel model,
                                    Handle<YieldTermStructure> termStructure)
         : base(model)
      {
         termStructure_ = termStructure;
      }

      public override void calculate()
      {
         if (model_ == null)
            throw new ArgumentException("null model");

         Date referenceDate = new Date();
         DayCounter dayCounter = new DayCounter();
         try
         {
            TermStructureConsistentModel tsmodel = (TermStructureConsistentModel)model_.link;
            ///if (tsmodel != null)
            referenceDate = tsmodel.termStructure().link.referenceDate();
            dayCounter = tsmodel.termStructure().link.dayCounter();
         }
         //else
         catch
         {
            referenceDate = termStructure_.link.referenceDate();
            dayCounter = termStructure_.link.dayCounter();
         }

         double value = 0.0;
         CapFloorType type = arguments_.type;
         int nPeriods = arguments_.endDates.Count;

         for (int i = 0; i < nPeriods; i++)
         {
            double fixingTime =
               dayCounter.yearFraction(referenceDate,
                                       arguments_.fixingDates[i]);
            double paymentTime =
               dayCounter.yearFraction(referenceDate,
                                       arguments_.endDates[i]);
#if QL_TODAYS_PAYMENTS
            if (paymentTime >= 0.0)
            {
#else
            if (paymentTime > 0.0)
            {
#endif
               double tenor = arguments_.accrualTimes[i];
               double fixing = (double)arguments_.forwards[i];
               if (fixingTime <= 0.0)
               {
                  if (type == CapFloorType.Cap || type == CapFloorType.Collar)
                  {
                     double discount = model_.link.discount(paymentTime);
                     double strike = (double)arguments_.capRates[i];
                     value += discount * arguments_.nominals[i] * tenor
                              * arguments_.gearings[i]
                              * Math.Max(0.0, fixing - strike);
                  }
                  if (type == CapFloorType.Floor || type == CapFloorType.Collar)
                  {
                     double discount = model_.link.discount(paymentTime);
                     double strike = (double)arguments_.floorRates[i];
                     double mult = (type == CapFloorType.Floor) ? 1.0 : -1.0;
                     value += discount * arguments_.nominals[i] * tenor
                              * mult * arguments_.gearings[i]
                              * Math.Max(0.0, strike - fixing);
                  }
               }
               else
               {
                  double maturity =
                     dayCounter.yearFraction(referenceDate,
                                             arguments_.startDates[i]);
                  if (type == CapFloorType.Cap || type == CapFloorType.Collar)
                  {
                     double temp = 1.0 + (double)arguments_.capRates[i] * tenor;
                     value += arguments_.nominals[i] *
                              arguments_.gearings[i] * temp *
                              model_.link.discountBondOption(Option.Type.Put, 1.0 / temp,
                                                             maturity, paymentTime);
                  }
                  if (type == CapFloorType.Floor || type == CapFloorType.Collar)
                  {
                     double temp = 1.0 + (double)arguments_.floorRates[i] * tenor;
                     double mult = (type == CapFloorType.Floor) ? 1.0 : -1.0;
                     value += arguments_.nominals[i] *
                              arguments_.gearings[i] * temp * mult *
                              model_.link.discountBondOption(Option.Type.Call, 1.0 / temp,
                                                             maturity, paymentTime);
                  }
               }
            }
         }
         results_.value = value;
      }
   }
}