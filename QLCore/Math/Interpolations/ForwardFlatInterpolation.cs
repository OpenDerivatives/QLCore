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

using System.Collections.Generic;

namespace QLCore
{
   public class ForwardFlatInterpolationImpl : Interpolation.templateImpl
   {
      private List<double> primitive_;

      public ForwardFlatInterpolationImpl(List<double> xBegin, int size, List<double> yBegin) : base(xBegin, size, yBegin)
      {
         primitive_ = new InitializedList<double>(size_);
      }

      public override void update()
      {
         primitive_[0] = 0.0;
         for (int i = 1; i < size_; i++)
         {
            double dx = xBegin_[i] - xBegin_[i - 1];
            primitive_[i] = primitive_[i - 1] + dx * yBegin_[i - 1];
         }
      }
      public override double value(double x)
      {
         if (x >= xBegin_[size_ - 1])
            return yBegin_[size_ - 1];

         int i = locate(x);
         return yBegin_[i];
      }
      public override double primitive(double x)
      {
         int i = locate(x);
         double dx = x - xBegin_[i];
         return primitive_[i] + dx * yBegin_[i];
      }
      public override double derivative(double x) { return 0.0; }
      public override double secondDerivative(double x) { return 0.0; }
   }

   //! Forward-flat interpolation between discrete points
   public class ForwardFlatInterpolation : Interpolation
   {
      /*! \pre the \f$ x \f$ values must be sorted. */
      public ForwardFlatInterpolation(List<double> xBegin, int size, List<double> yBegin)
      {
         impl_ = new ForwardFlatInterpolationImpl(xBegin, size, yBegin);
         impl_.update();
      }
   }

   //! Forward-flat interpolation factory and traits
   public class ForwardFlat : IInterpolationFactory
   {
      public Interpolation interpolate(List<double> xBegin, int size, List<double> yBegin)
      {
         return new ForwardFlatInterpolation(xBegin, size, yBegin);
      }
      public bool global { get { return false; } }
      public int requiredPoints { get { return 2; } }
   }
}
