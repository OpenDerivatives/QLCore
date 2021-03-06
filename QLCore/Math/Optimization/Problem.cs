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

namespace QLCore
{
   //! Constrained optimization problem
   public class Problem
   {
      //! Unconstrained cost function
      protected CostFunction costFunction_;
      public CostFunction costFunction() { return costFunction_; }

      //! Constraint
      protected Constraint constraint_;
      public Constraint constraint() { return constraint_; }

      //! current value of the local minimum
      protected Vector currentValue_;
      public Vector currentValue() { return currentValue_; }

      //! function and gradient norm values at the curentValue_ (i.e. the last step)
      protected double? functionValue_, squaredNorm_;
      public double functionValue() { return functionValue_.GetValueOrDefault(); }
      public double gradientNormValue() { return squaredNorm_.GetValueOrDefault(); }

      //! number of evaluation of cost function and its gradient
      protected int functionEvaluation_, gradientEvaluation_;
      public int functionEvaluation() { return functionEvaluation_; }
      public int gradientEvaluation() { return gradientEvaluation_; }


      //! default constructor
      //public Problem(CostFunction costFunction, Constraint constraint, Vector initialValue = Array())
      public Problem(CostFunction costFunction, Constraint constraint, Vector initialValue)
      {
         costFunction_ = costFunction;
         constraint_ = constraint;
         currentValue_ = initialValue.Clone();
         Utils.QL_REQUIRE(!constraint.empty(), () => "empty constraint given");
      }

      /*! \warning it does not reset the current minumum to any initial value
      */
      public void reset()
      {
         functionEvaluation_ = gradientEvaluation_ = 0;
         functionValue_ = squaredNorm_ = null;
      }

      //! call cost function computation and increment evaluation counter
      public double value(Vector x)
      {
         ++functionEvaluation_;
         return costFunction_.value(x);
      }

      //! call cost values computation and increment evaluation counter
      public Vector values(Vector x)
      {
         ++functionEvaluation_;
         return costFunction_.values(x);
      }

      //! call cost function gradient computation and increment
      //  evaluation counter
      public void gradient(ref Vector grad_f, Vector x)
      {
         ++gradientEvaluation_;
         costFunction_.gradient(ref grad_f, x);
      }

      //! call cost function computation and it gradient
      public double valueAndGradient(ref Vector grad_f, Vector x)
      {
         ++functionEvaluation_;
         ++gradientEvaluation_;
         return costFunction_.valueAndGradient(ref grad_f, x);
      }

      public void setCurrentValue(Vector currentValue)
      {
         currentValue_ = currentValue.Clone();
      }

      public void setFunctionValue(double functionValue)
      {
         functionValue_ = functionValue;
      }

      public void setGradientNormValue(double squaredNorm)
      {
         squaredNorm_ = squaredNorm;
      }
   }
}
