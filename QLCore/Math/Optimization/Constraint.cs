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
   public interface IConstraint
   {
      bool test(Vector param);
      Vector upperBound(Vector parameters);
      Vector lowerBound(Vector parameters);
   }

   //! Base constraint class
   public class Constraint
   {
      protected IConstraint impl_;
      public bool empty() { return impl_ == null; }

      public Constraint() : this(null) { }
      public Constraint(IConstraint impl)
      {
         impl_ = impl;
      }

      public double update(ref Vector p, Vector direction, double beta)
      {
         double diff = beta;
         Vector newParams = p + diff * direction;
         bool valid = test(newParams);
         int icount = 0;
         while (!valid)
         {
            if (icount > 200)
               Utils.QL_FAIL("can't update parameter vector");
            diff *= 0.5;
            icount++;
            newParams = p + diff * direction;
            valid = test(newParams);
         }
         p += diff * direction;
         return diff;
      }

      //! Tests if params satisfy the constraint
      public virtual bool test(Vector p) { return impl_.test(p); }

      //! Returns upper bound for given parameters
      public virtual Vector upperBound(Vector parameters)
      {
         Vector result = impl_.upperBound(parameters);
         Utils.QL_REQUIRE(parameters.size() == result.size(), () =>
                          "upper bound size (" + result.size()
                          + ") not equal to params size ("
                          + parameters.size() + ")");
         return result;
      }

      //! Returns lower bound for given parameters
      public virtual Vector lowerBound(Vector parameters)
      {
         Vector result = impl_.lowerBound(parameters);
         Utils.QL_REQUIRE(parameters.size() == result.size(), () =>
                          "lower bound size (" + result.size()
                          + ") not equal to params size ("
                          + parameters.size() + ")");
         return result;
      }
   }

   //! No constraint
   public class NoConstraint : Constraint
   {
      private class Impl : IConstraint
      {
         public bool test(Vector v) { return true; }
         public Vector upperBound(Vector parameters)
         {
            return new Vector(parameters.size(), Double.MaxValue);
         }

         public Vector lowerBound(Vector parameters)
         {
            return new Vector(parameters.size(), Double.MinValue);
         }
      }
      public NoConstraint() : base(new Impl()) { }
   }

   //! %Constraint imposing positivity to all arguments
   public class PositiveConstraint : Constraint
   {
      public PositiveConstraint()
         : base(new PositiveConstraint.Impl())
      {
      }

      private class Impl : IConstraint
      {
         public bool test(Vector v)
         {
            for (int i = 0; i < v.Count; ++i)
            {
               if (v[i] <= 0.0)
                  return false;
            }
            return true;
         }

         public Vector upperBound(Vector parameters)
         {
            return new Vector(parameters.size(), Double.MaxValue);
         }

         public Vector lowerBound(Vector parameters)
         {
            return new Vector(parameters.size(), 0.0);
         }
      }
   }

   //! %Constraint imposing all arguments to be in [low,high]
   public class BoundaryConstraint : Constraint
   {
      public BoundaryConstraint(double low, double high)
         : base(new BoundaryConstraint.Impl(low, high))
      {
      }

      private class Impl : IConstraint
      {
         private double low_;
         private double high_;

         public Impl(double low, double high)
         {
            low_ = low;
            high_ = high;
         }
         public bool test(Vector v)
         {
            for (int i = 0; i < v.Count; i++)
            {
               if ((v[i] < low_) || (v[i] > high_))
                  return false;
            }
            return true;
         }

         public Vector upperBound(Vector parameters)
         {
            return new Vector(parameters.size(), high_);
         }

         public Vector lowerBound(Vector parameters)
         {
            return new Vector(parameters.size(), low_);
         }
      }
   }

   //! %Constraint enforcing both given sub-constraints
   public class CompositeConstraint : Constraint
   {
      public CompositeConstraint(Constraint c1, Constraint c2) : base(new Impl(c1, c2)) { }

      private class Impl : IConstraint
      {
         private Constraint c1_, c2_;

         public Impl(Constraint c1, Constraint c2)
         {
            c1_ = c1;
            c2_ = c2;
         }

         public bool test(Vector p)
         {
            return c1_.test(p) && c2_.test(p);
         }

         public Vector upperBound(Vector parameters)
         {
            Vector c1ub = c1_.upperBound(parameters);
            Vector c2ub = c2_.upperBound(parameters);
            Vector rtrnArray = new Vector(c1ub.size(), 0.0);

            for (int iter = 0; iter < c1ub.size(); iter++)
            {
               rtrnArray[iter] = Math.Min(c1ub[iter], c2ub[iter]);
            }

            return rtrnArray;
         }

         public Vector lowerBound(Vector parameters)
         {
            Vector c1lb = c1_.lowerBound(parameters);
            Vector c2lb = c2_.lowerBound(parameters);
            Vector rtrnArray = new Vector(c1lb.size(), 0.0);

            for (int iter = 0; iter < c1lb.size(); iter++)
            {
               rtrnArray[iter] = Math.Max(c1lb[iter], c2lb[iter]);

            }

            return rtrnArray;
         }
      }
   }

   //! %Constraint imposing i-th argument to be in [low_i,high_i] for all i
   public class NonhomogeneousBoundaryConstraint: Constraint
   {
      private class Impl: IConstraint
      {
         public Impl(Vector low, Vector high)
         {
            low_ = low;
            high_ = high;
            Utils.QL_REQUIRE(low_.Count == high_.Count, () => "Upper and lower boundaries sizes are inconsistent.");
         }

         public bool test(Vector parameters)
         {
            Utils.QL_REQUIRE(parameters.size() == low_.Count, () =>
                             "Number of parameters and boundaries sizes are inconsistent.");

            for (int i = 0; i < parameters.size(); i++)
            {
               if ((parameters[i] < low_[i]) || (parameters[i] > high_[i]))
                  return false;
            }
            return true;
         }

         public Vector upperBound(Vector v)
         {
            return high_;
         }

         public Vector lowerBound(Vector v)
         {
            return low_;
         }

         private Vector low_, high_;
      }

      public NonhomogeneousBoundaryConstraint(Vector low, Vector high)
         : base(new Impl(low, high))
      {}
   }

}
