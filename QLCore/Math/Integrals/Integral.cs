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
   public abstract class Integrator
   {
      protected Integrator(double? absoluteAccuracy, int maxEvaluations)
      {
         absoluteAccuracy_ = absoluteAccuracy;
         maxEvaluations_ = maxEvaluations;
         if (absoluteAccuracy != null)
            Utils.QL_REQUIRE(absoluteAccuracy > double.Epsilon, () =>
                             "required tolerance (" + absoluteAccuracy + ") not allowed. It must be > " + double.Epsilon);
      }

      public double value(Func<double, double> f, double a, double b)
      {
         evaluations_ = 0;
         if (a.IsEqual(b))
            return 0.0;
         if (b > a)
            return integrate(f, a, b);

         return -integrate(f, b, a);
      }

      // Modifiers
      public void setAbsoluteAccuracy(double accuracy)
      {
         absoluteAccuracy_ = accuracy;
      }

      public void setMaxEvaluations(int maxEvaluations)
      {
         maxEvaluations_ = maxEvaluations;
      }

      // Inspectors
      public double? absoluteAccuracy()
      {
         return absoluteAccuracy_;
      }

      public int maxEvaluations()
      {
         return maxEvaluations_;
      }

      public double absoluteError()
      {
         return absoluteError_;
      }

      public int numberOfEvaluations()
      {
         return evaluations_;
      }

      public bool integrationSuccess()
      {
         return evaluations_ <= maxEvaluations_ && absoluteError_ <= absoluteAccuracy_;
      }

      protected abstract double integrate(Func<double, double> f, double a, double b);

      protected void setAbsoluteError(double error)
      {
         absoluteError_ = error;
      }

      protected void setNumberOfEvaluations(int evaluations)
      {
         evaluations_ = evaluations;
      }

      protected void increaseNumberOfEvaluations(int increase)
      {
         evaluations_ += increase;
      }

      protected double? absoluteAccuracy_;
      protected double absoluteError_;
      protected int maxEvaluations_;
      protected int evaluations_;


   }

}
