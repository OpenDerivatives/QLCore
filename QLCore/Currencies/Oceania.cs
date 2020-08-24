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

   //! Australian dollar
//    ! The ISO three-letter code is AUD; the numeric code is 36.
//        It is divided into 100 cents.
//
//        \ingroup currencies
//
   public class AUDCurrency : Currency
   {
      public AUDCurrency() : base("Australian dollar", "AUD", 36, "A$", "", 100, new Rounding(), "%3% %1$.2f") { }
   }

   //! New Zealand dollar
//    ! The ISO three-letter code is NZD; the numeric code is 554.
//        It is divided in 100 cents.
//
//        \ingroup currencies
//
   public class NZDCurrency : Currency
   {
      public NZDCurrency() : base("New Zealand dollar", "NZD", 554, "NZ$", "", 100, new Rounding(), "%3% %1$.2f") { }
   }

}
