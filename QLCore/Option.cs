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
   //! base option class
   public class Option : Instrument
   {
      public enum Type
      {
         Put = -1,
         Call = 1
      }

      // arguments
      protected Payoff payoff_;

      public Payoff payoff()
      {
         return payoff_;
      }

      protected Exercise exercise_;

      public Exercise exercise()
      {
         return exercise_;
      }

      public Option(Payoff payoff, Exercise exercise)
      {
         payoff_ = payoff;
         exercise_ = exercise;
      }

      public override void setupArguments(IPricingEngineArguments args)
      {
         Option.Arguments arguments = args as Option.Arguments;

         Utils.QL_REQUIRE(arguments != null, () => "wrong argument type");

         arguments.payoff = payoff_;
         arguments.exercise = exercise_;
      }

      //! basic %option %arguments
      public class Arguments : IPricingEngineArguments
      {
         public Payoff payoff { get; set; }
         public Exercise exercise { get; set; }

         public virtual void validate()
         {
            Utils.QL_REQUIRE(payoff != null, () => "no payoff given");
            Utils.QL_REQUIRE(exercise != null, () => "no exercise given");
         }
      }
   }
}