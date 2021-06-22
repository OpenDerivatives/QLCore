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
   //! %instrument callability
   public class Callability : Event
   {
      //! amount to be paid upon callability
      public class Price
      {
         public enum Type { Dirty, Clean }

         public Price()
         {
            amount_ = null;
         }

         public Price(double amount, Type type)
         {
            amount_ = amount;
            type_ = type;
         }

         public double amount()
         {
            Utils.QL_REQUIRE(amount_ != null, () => "no amount given");
            return amount_.Value;
         }

         public Type type()  { return type_; }

         private double? amount_;
         private Type type_;
      }

      //! type of the callability
      public enum Type { Call, Put }

      public Callability(Settings settings, Price price, Type type, Date date)
      : base(settings)
      {
         price_ = price;
         type_ = type;
         date_ = date;
      }
      public Price price()
      {
         Utils.QL_REQUIRE(price_ != null, () => "no price given");
         return price_;
      }
      public Type type() { return type_; }
      // Event interface
      public override Date date() { return date_; }

      private Price price_;
      private Type type_;
      private Date date_;

   }

   public class CallabilitySchedule : List<Callability> {}


}
