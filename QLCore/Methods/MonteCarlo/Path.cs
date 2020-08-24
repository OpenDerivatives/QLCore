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
using System.Linq;

namespace QLCore
{
   //! single-factor random walk
   /*! \ingroup mcarlo

       \note the path includes the initial asset value as its first point.
   */

   public interface IPath : ICloneable
   {
      int length();
   }

   public interface IPathGenerator<GSG>
   {
      Sample<IPath> next();
      Sample<IPath> antithetic();
   }

   public class Path : IPath
   {
      private TimeGrid timeGrid_;
      private Vector values_;

      // required for generics
      public Path() { }

      public Path(TimeGrid timeGrid) : this(timeGrid, new Vector()) { }
      public Path(TimeGrid timeGrid, Vector values)
      {
         timeGrid_ = timeGrid;
         values_ = values.Clone();
         if (values_.empty())
            values_ = new Vector(timeGrid_.size());

         Utils.QL_REQUIRE(values_.size() == timeGrid_.size(), () => "different number of times and asset values");
      }

      // inspectors
      public bool empty() { return timeGrid_.empty(); }
      public int length() { return timeGrid_.size(); }

      //! asset value at the \f$ i \f$-th point
      public double this[int i] { get { return values_[i]; } set { values_[i] = value; } }
      public double value(int i) { return values_[i]; }
      public Vector values() { return values_; }

      //! time at the \f$ i \f$-th point
      public double time(int i) { return timeGrid_[i]; }

      //! initial asset value
      public double front() { return values_.First(); }
      public void setFront(double value) { values_[0] = value; }

      //! final asset value
      public double back() { return values_.Last(); }

      //! time grid
      public TimeGrid timeGrid() { return timeGrid_; }

      // ICloneable interface
      public object Clone()
      {
         Path temp = (Path)this.MemberwiseClone();
         temp.values_ = new Vector(this.values_);
         return temp;
      }
   }
}
