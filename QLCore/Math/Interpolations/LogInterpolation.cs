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
using System.Collections.Generic;

namespace QLCore
{
   public class LogInterpolationImpl<Interpolator> : Interpolation.templateImpl
      where Interpolator : IInterpolationFactory, new ()
   {
      private List<double> logY_;
      private Interpolation interpolation_;

      public LogInterpolationImpl(List<double> xBegin, int size, List<double> yBegin)
         : this(xBegin, size, yBegin, FastActivator<Interpolator>.Create())
      {}

      public LogInterpolationImpl(List<double> xBegin, int size, List<double> yBegin, IInterpolationFactory factory)
         : base(xBegin, size, yBegin)
      {
         logY_ = new InitializedList<double>(size_);
         interpolation_ = factory.interpolate(xBegin_, size, logY_);
      }

      public override void update()
      {
         for (int i = 0; i < size_; ++i)
         {
            Utils.QL_REQUIRE(yBegin_[i] > 0.0, () => "invalid value (" + yBegin_[i] + ") at index " + i);
            logY_[i] = Math.Log(yBegin_[i]);
         }
         interpolation_.update();
      }

      public override double value(double x)
      {
         return Math.Exp(interpolation_.value(x, true));
      }

      public override double primitive(double x)
      {
         throw new NotImplementedException("LogInterpolation primitive not implemented");
      }

      public override double derivative(double x)
      {
         return value(x) * interpolation_.derivative(x, true);
      }

      public override double secondDerivative(double x)
      {
         return derivative(x) * interpolation_.derivative(x, true) +
                value(x) * interpolation_.secondDerivative(x, true);
      }
   }

   //! log-linear interpolation factory and traits
   public class LogLinear : IInterpolationFactory
   {
      public Interpolation interpolate(List<double> xBegin, int size, List<double> yBegin)
      {
         return new LogLinearInterpolation(xBegin, size, yBegin);
      }

      public bool global
      {
         get
         {
            return false;
         }
      }

      public int requiredPoints
      {
         get
         {
            return 2;
         }
      }
   }

   //! %log-linear interpolation between discrete points
   public class LogLinearInterpolation : Interpolation
   {
      /*! \pre the \f$ x \f$ values must be sorted. */

      public LogLinearInterpolation(List<double> xBegin, int size, List<double> yBegin)
      {
         impl_ = new LogInterpolationImpl<Linear>(xBegin, size, yBegin);
         impl_.update();
      }
   }

   //! log-cubic interpolation factory and traits
   public class LogCubic : IInterpolationFactory
   {
      private CubicInterpolation.DerivativeApprox da_;
      private bool monotonic_;
      private CubicInterpolation.BoundaryCondition leftType_, rightType_;
      private double leftValue_, rightValue_;

      public LogCubic()
      {}

      public LogCubic(CubicInterpolation.DerivativeApprox da, bool monotonic,
                      CubicInterpolation.BoundaryCondition leftCondition, double leftConditionValue,
                      CubicInterpolation.BoundaryCondition rightCondition, double rightConditionValue)
      {
         da_ = da;
         monotonic_ = monotonic;
         leftType_ = leftCondition;
         rightType_ = rightCondition;
         leftValue_ = leftConditionValue;
         rightValue_ = rightConditionValue;
      }

      public Interpolation interpolate(List<double> xBegin, int size, List<double> yBegin)
      {
         return new LogCubicInterpolation(xBegin, size, yBegin, da_, monotonic_,
                                          leftType_, leftValue_, rightType_, rightValue_);
      }

      public bool global
      {
         get
         {
            return true;
         }
      }

      public int requiredPoints
      {
         get
         {
            return 2;
         }
      }
   }

   //! %log-cubic interpolation between discrete points
   public class LogCubicInterpolation : Interpolation
   {
      /*! \pre the \f$ x \f$ values must be sorted. */

      public LogCubicInterpolation(List<double> xBegin, int size, List<double> yBegin,
                                   CubicInterpolation.DerivativeApprox da,
                                   bool monotonic,
                                   CubicInterpolation.BoundaryCondition leftC,
                                   double leftConditionValue,
                                   CubicInterpolation.BoundaryCondition rightC,
                                   double rightConditionValue)
      {
         impl_ = new LogInterpolationImpl<Cubic>(xBegin, size, yBegin,
                                                 new Cubic(da, monotonic, leftC, leftConditionValue, rightC, rightConditionValue));
         impl_.update();
      }
   }
}
