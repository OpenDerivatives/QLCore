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
   /// <summary>
   /// One-dimensional simple FDM mesher object working on an index
   /// </summary>
   public class Fdm1dMesher
   {
      public Fdm1dMesher(int size)
      {
         locations_ = new InitializedList<double>(size);
         dplus_ = new InitializedList < double? >(size);
         dminus_ = new InitializedList < double? >(size);
      }

      public int size()
      {
         return locations_.Count;
      }

      public double? dplus(int index)
      {
         return dplus_[index];
      }

      public double? dminus(int index)
      {
         return dminus_[index];
      }

      public double location(int index)
      {
         return locations_[index];
      }

      public List<double> locations()
      {
         return locations_;
      }

      protected List<double> locations_;
      protected List < double? > dplus_, dminus_;
   }
}
