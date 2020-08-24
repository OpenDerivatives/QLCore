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
   //! transformed grid
   /*! This package encapuslates an array of grid points.  It is used primarily in PDE calculations.
   */
   public class TransformedGrid
   {
      protected Vector grid_;
      protected Vector transformedGrid_;
      protected Vector dxm_;
      protected Vector dxp_;
      protected Vector dx_;

      public Vector gridArray() { return grid_;}
      public Vector transformedGridArray() { return transformedGrid_;}
      public Vector dxmArray() { return dxm_;}
      public Vector dxpArray() { return dxp_;}
      public Vector dxArray() { return dx_;}

      public TransformedGrid(Vector grid)
      {
         grid_ = grid.Clone();
         transformedGrid_ = grid.Clone();
         dxm_ = new Vector(grid.size());
         dxp_ = new Vector(grid.size());
         dx_ = new Vector(grid.size());

         for (int i = 1; i < transformedGrid_.size() - 1 ; i++)
         {
            dxm_[i] = transformedGrid_[i] - transformedGrid_[i - 1];
            dxp_[i] = transformedGrid_[i + 1] - transformedGrid_[i];
            dx_[i] = dxm_[i] + dxp_[i];
         }
      }

      public TransformedGrid(Vector grid, Func<double, double> func)
      {
         grid_ = grid.Clone();
         transformedGrid_ = new Vector(grid.size());
         dxm_ = new Vector(grid.size());
         dxp_ = new Vector(grid.size());
         dx_ = new Vector(grid.size());

         for (int i = 0; i < grid.size(); i++)
            transformedGrid_[i] = func(grid_[i]);

         for (int i = 1; i < transformedGrid_.size() - 1 ; i++)
         {
            dxm_[i] = transformedGrid_[i] - transformedGrid_[i - 1];
            dxp_[i] = transformedGrid_[i + 1] - transformedGrid_[i];
            dx_[i] = dxm_[i] + dxp_[i];
         }
      }

      public double grid(int i) { return grid_[i]; }
      public double transformedGrid(int i) { return transformedGrid_[i]; }
      public double dxm(int i) { return dxm_[i];}
      public double dxp(int i) { return dxp_[i]; }
      public double dx(int i) { return dx_[i]; }
      public int size() {return grid_.size();}
   }

   public class LogGrid : TransformedGrid
   {
      public LogGrid(Vector grid) : base(grid, Math.Log) {}

      public Vector logGridArray() { return transformedGridArray();}
      public double logGrid(int i) { return transformedGrid(i);}
   }
}
