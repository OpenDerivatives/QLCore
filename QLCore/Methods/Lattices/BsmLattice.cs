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
   // this is just a wrapper for QL compatibility
   public class BlackScholesLattice<T> : BlackScholesLattice where T : ITree
   {
      public BlackScholesLattice(ITree tree, double riskFreeRate, double end, int steps)
         : base(tree, riskFreeRate, end, steps)
      {}
   }

   //! Simple binomial lattice approximating the Black-Scholes model
   /*! \ingroup lattices */

   public class BlackScholesLattice : TreeLattice1D<BlackScholesLattice>, IGenericLattice
   {
      public BlackScholesLattice(ITree tree, double riskFreeRate, double end, int steps)
         : base(new TimeGrid(end, steps), 2)
      {
         tree_ = tree;
         riskFreeRate_ = riskFreeRate;
         dt_ = end / steps;
         discount_ = Math.Exp(-riskFreeRate * (end / steps));
         pd_ = tree.probability(0, 0, 0);
         pu_ = tree.probability(0, 0, 1);
      }

      public double riskFreeRate()
      {
         return riskFreeRate_;
      }

      public double dt()
      {
         return dt_;
      }

      public int size(int i)
      {
         return tree_.size(i);
      }

      public double discount(int i, int j)
      {
         return discount_;
      }

      public override void stepback(int i, Vector values, Vector newValues)
      {
         for (int j = 0; j < size(i); j++)
            newValues[j] = (pd_ * values[j] + pu_ * values[j + 1]) * discount_;
      }

      public override double underlying(int i, int index)
      {
         return tree_.underlying(i, index);
      }

      public int descendant(int i, int index, int branch)
      {
         return tree_.descendant(i, index, branch);
      }

      public double probability(int i, int index, int branch)
      {
         return tree_.probability(i, index, branch);
      }

      // this is a workaround for CuriouslyRecurringTemplate of TreeLattice
      // recheck it
      protected override BlackScholesLattice impl()
      {
         return this;
      }

      protected ITree tree_;
      protected double riskFreeRate_;
      protected double dt_;
      protected double discount_;
      protected double pd_, pu_;

   }
}
