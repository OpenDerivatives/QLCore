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
   //! %EuriborSwapIsdaFixA index base class
   /*! %Euribor %Swap indexes fixed by ISDA in cooperation with
       Reuters and Intercapital Brokers at 11am Frankfurt.
       Annual 30/360 vs 6M Euribor, 1Y vs 3M Euribor.
       Reuters page ISDAFIX2 or EURSFIXA=.

       Further info can be found at <http://www.isda.org/fix/isdafix.html> or
       Reuters page ISDAFIX.

   */
   public  class EuriborSwapIsdaFixA : SwapIndex
   {
      public EuriborSwapIsdaFixA(Period tenor)
         : this(tenor, new Handle<YieldTermStructure>()) {}

      public EuriborSwapIsdaFixA(Period tenor, Handle<YieldTermStructure> h)
         : base("EuriborSwapIsdaFixA", // familyName
                tenor,
                2, // settlementDays
                new EURCurrency(),
                new TARGET(),
                new Period(1, TimeUnit.Years), // fixedLegTenor
                BusinessDayConvention.ModifiedFollowing, // fixedLegConvention
                new Thirty360(Thirty360.Thirty360Convention.BondBasis), // fixedLegDaycounter
                tenor > new Period(1, TimeUnit.Years) ?
                new Euribor(new Period(6, TimeUnit.Months), h) :
                new Euribor(new Period(3, TimeUnit.Months), h)) {}

      public EuriborSwapIsdaFixA(Period tenor,
                                 Handle<YieldTermStructure> forwarding,
                                 Handle<YieldTermStructure> discounting)
         : base("EuriborSwapIsdaFixA", // familyName
                tenor,
                2, // settlementDays
                new EURCurrency(),
                new TARGET(),
                new Period(1, TimeUnit.Years), // fixedLegTenor
                BusinessDayConvention.ModifiedFollowing, // fixedLegConvention
                new Thirty360(Thirty360.Thirty360Convention.BondBasis), // fixedLegDaycounter
                tenor > new Period(1, TimeUnit.Years) ?
                new Euribor(new Period(6, TimeUnit.Months), forwarding) :
                new Euribor(new Period(3, TimeUnit.Months), forwarding),
                discounting) {}
   }

   //! %EuriborSwapIsdaFixB index base class
   /*! %Euribor %Swap indexes fixed by ISDA in cooperation with
       Reuters and Intercapital Brokers at 12am Frankfurt.
       Annual 30/360 vs 6M Euribor, 1Y vs 3M Euribor.
       Reuters page ISDAFIX2 or EURSFIXB=.

       Further info can be found at <http://www.isda.org/fix/isdafix.html> or
       Reuters page ISDAFIX.

   */
   public class EuriborSwapIsdaFixB : SwapIndex
   {
      public EuriborSwapIsdaFixB(Period tenor)
         : this(tenor, new Handle<YieldTermStructure>()) {}

      public EuriborSwapIsdaFixB(Period tenor, Handle<YieldTermStructure> h)
         : base("EuriborSwapIsdaFixB", // familyName
                tenor,
                2, // settlementDays
                new EURCurrency(),
                new TARGET(),
                new Period(1, TimeUnit.Years), // fixedLegTenor
                BusinessDayConvention.ModifiedFollowing, // fixedLegConvention
                new Thirty360(Thirty360.Thirty360Convention.BondBasis), // fixedLegDaycounter
                tenor > new Period(1, TimeUnit.Years) ?
                new Euribor(new Period(6, TimeUnit.Months), h) :
                new Euribor(new Period(3, TimeUnit.Months), h)) {}


      public EuriborSwapIsdaFixB(Period tenor,
                                 Handle<YieldTermStructure> forwarding,
                                 Handle<YieldTermStructure> discounting)
         : base("EuriborSwapIsdaFixB", // familyName
                tenor,
                2, // settlementDays
                new EURCurrency(),
                new TARGET(),
                new Period(1, TimeUnit.Years), // fixedLegTenor
                BusinessDayConvention.ModifiedFollowing, // fixedLegConvention
                new Thirty360(Thirty360.Thirty360Convention.BondBasis), // fixedLegDaycounter
                tenor > new Period(1, TimeUnit.Years) ?
                new Euribor(new Period(6, TimeUnit.Months), forwarding) :
                new Euribor(new Period(3, TimeUnit.Months), forwarding),
                discounting) {}
   }

   //! %EuriborSwapIfrFix index base class
   /*! %Euribor %Swap indexes published by IFR Markets and
       distributed by Reuters page TGM42281 and by Telerate.
       Annual 30/360 vs 6M Euribor, 1Y vs 3M Euribor.
       For more info see <http://www.ifrmarkets.com>.

   */
   public class EuriborSwapIfrFix : SwapIndex
   {
      public EuriborSwapIfrFix(Period tenor)
         : this(tenor, new Handle<YieldTermStructure>()) { }

      public EuriborSwapIfrFix(Period tenor, Handle<YieldTermStructure> h)
         : base("EuriborSwapIfrFix", // familyName
                tenor,
                2, // settlementDays
                new EURCurrency(),
                new TARGET(),
                new Period(1, TimeUnit.Years), // fixedLegTenor
                BusinessDayConvention.ModifiedFollowing, // fixedLegConvention
                new Thirty360(Thirty360.Thirty360Convention.BondBasis), // fixedLegDaycounter
                tenor > new Period(1, TimeUnit.Years) ?
                new Euribor(new Period(6, TimeUnit.Months), h) :
                new Euribor(new Period(3, TimeUnit.Months), h)) {}

      public EuriborSwapIfrFix(Period tenor,
                               Handle<YieldTermStructure> forwarding,
                               Handle<YieldTermStructure> discounting)

         : base("EuriborSwapIfrFix", // familyName
                tenor,
                2, // settlementDays
                new EURCurrency(),
                new TARGET(),
                new Period(1, TimeUnit.Years), // fixedLegTenor
                BusinessDayConvention.ModifiedFollowing, // fixedLegConvention
                new Thirty360(Thirty360.Thirty360Convention.BondBasis), // fixedLegDaycounter
                tenor > new Period(1, TimeUnit.Years) ?
                new Euribor(new Period(6, TimeUnit.Months), forwarding) :
                new Euribor(new Period(3, TimeUnit.Months), forwarding),
                discounting) {}

   }
}
