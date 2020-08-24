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

using System.Linq;

namespace QLCore
{
   public abstract class BasketPayoff : Payoff
   {
      private Payoff basePayoff_;

      protected BasketPayoff(Payoff p)
      {
         basePayoff_ = p;
      }

      public override string name()
      {
         return basePayoff_.name();
      }

      public override string description()
      {
         return basePayoff_.description();
      }

      public override double value(double price)
      {
         return basePayoff_.value(price);
      }

      public virtual double value(Vector a)
      {
         return basePayoff_.value(accumulate(a));
      }

      public abstract double accumulate(Vector a);

      public Payoff basePayoff()
      {
         return basePayoff_;
      }
   }

   public class MinBasketPayoff : BasketPayoff
   {
      public MinBasketPayoff(Payoff p) : base(p)
      {}

      public override double accumulate(Vector a)
      {
         return a.Min();
      }
   }

   public class MaxBasketPayoff : BasketPayoff
   {
      public MaxBasketPayoff(Payoff p) : base(p)
      {}

      public override double accumulate(Vector a)
      {
         return a.Max();
      }
   }

   public class AverageBasketPayoff : BasketPayoff
   {
      public AverageBasketPayoff(Payoff p, Vector a) : base(p)
      {
         weights_ = a;
      }

      public AverageBasketPayoff(Payoff p, int n) : base(p)
      {
         weights_ = new Vector(n, 1.0 / n);
      }

      public override double accumulate(Vector a)
      {
         double tally = weights_ * a;
         return tally;
      }

      private Vector weights_;
   }

   public class SpreadBasketPayoff : BasketPayoff
   {
      public SpreadBasketPayoff(Payoff p)
         : base(p)
      {}

      public override double accumulate(Vector a)
      {
         Utils.QL_REQUIRE(a.size() == 2, () => "payoff is only defined for two underlyings");
         return a[0] - a[1];
      }
   }

   //! Basket option on a number of assets
   //! \ingroup instruments
   public class BasketOption : MultiAssetOption
   {
      public new class Engine : GenericEngine<BasketOption.Arguments, BasketOption.Results>
      {}

      public BasketOption(BasketPayoff payoff, Exercise exercise) : base(payoff, exercise)
      {}
   }
}
