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

   public class FlatExtrapolator2D : Interpolation2D
   {
      public FlatExtrapolator2D(Interpolation2D decoratedInterpolation)
      {
         impl_ = new FlatExtrapolator2DImpl(decoratedInterpolation);
      }

      protected class FlatExtrapolator2DImpl: Interpolation2D.Impl
      {
         public FlatExtrapolator2DImpl(Interpolation2D decoratedInterpolation)
         {
            decoratedInterp_ = decoratedInterpolation;
            calculate();
         }
         public double xMin() { return decoratedInterp_.xMin();}
         public double xMax() { return decoratedInterp_.xMax();}
         public List<double> xValues() {return decoratedInterp_.xValues();}
         public int locateX(double x) {return decoratedInterp_.locateX(x);}
         public double yMin() {return decoratedInterp_.yMin();}
         public double yMax() {return decoratedInterp_.yMax();}
         public List<double> yValues() {return decoratedInterp_.yValues();}
         public int locateY(double y) {return decoratedInterp_.locateY(y);}
         public Matrix zData() {return decoratedInterp_.zData();}
         public bool isInRange(double x, double y) {return decoratedInterp_.isInRange(x, y);}
         public void update() {decoratedInterp_.update();}
         public void calculate()
         {
            // Nothing to do here
         }
         public double value(double x, double y)
         {
            x = bindX(x);
            y = bindY(y);
            return decoratedInterp_.value(x, y);
         }

         private Interpolation2D decoratedInterp_;

         private double bindX(double x)
         {
            if (x < xMin())
               return xMin();
            if (x > xMax())
               return xMax();
            return x;
         }
         private double bindY(double y)
         {
            if (y < yMin())
               return yMin();
            if (y > yMax())
               return yMax();
            return y;
         }
      }
   }
}
