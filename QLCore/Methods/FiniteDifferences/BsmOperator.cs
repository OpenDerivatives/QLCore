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
   //! Black-Scholes-Merton differential operator
   /*! \ingroup findiff */
   public class BSMOperator : TridiagonalOperator
   {
      public BSMOperator() { }

      public BSMOperator(int size, double dx, double r, double q, double sigma) : base(size)
      {
         double sigma2 = sigma * sigma;
         double nu = r - q - sigma2 / 2;
         double pd = -(sigma2 / dx - nu) / (2 * dx);
         double pu = -(sigma2 / dx + nu) / (2 * dx);
         double pm = sigma2 / (dx * dx) + r;
         setMidRows(pd, pm, pu);
      }

      public BSMOperator(Vector grid, GeneralizedBlackScholesProcess process, double residualTime) : base(grid.size())
      {
         LogGrid logGrid = new LogGrid(grid);
         var cc = new PdeConstantCoeff<PdeBSM>(process, residualTime, process.stateVariable().link.value());
         cc.generateOperator(residualTime, logGrid, this);
      }
   }
}
