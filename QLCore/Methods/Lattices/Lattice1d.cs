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
   //! One-dimensional tree-based lattice.
   public class TreeLattice1D<T> : TreeLattice<T> where T : IGenericLattice
   {
      public TreeLattice1D(TimeGrid timeGrid, int n) : base(timeGrid, n) { }

      public override Vector grid(double t)
      {
         int i = timeGrid().index(t);
         Vector grid = new Vector(impl().size(i));
         for (int j = 0; j < grid.size(); j++)
            grid[j] = impl().underlying(i, j);
         return grid;
      }
      public virtual double underlying(int i, int index)
      {
         return impl().underlying(i, index);
      }
   }
}
