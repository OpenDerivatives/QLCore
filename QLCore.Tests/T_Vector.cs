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
using Xunit;
using QLCore;

namespace TestSuite
{

   public class T_Vector
   {
      /// <summary>
      /// Sample values.
      /// </summary>
      protected readonly List<double> Data = new List<double>() { 1, 2, 3, 4, 5 };

      /// <summary>
      /// Test vector clone
      /// </summary>
      [Fact]
      public void testClone()
      {
         Vector vector = new Vector(Data);
         Vector clone = vector.Clone();

         QAssert.AreNotSame(vector, clone);
         QAssert.AreEqual(vector.Count, clone.Count);
         QAssert.CollectionAreEqual(vector, clone);
         vector[0] = 100;
         QAssert.CollectionAreNotEqual(vector, clone);

      }

      /// <summary>
      /// Test clone a vector using <c>IClonable</c> interface method.
      /// </summary>
      [Fact]
      public void testCloneICloneable()
      {
         Vector vector = new Vector(Data);
         Vector clone = (Vector)((QLCore.ICloneable)vector).Clone();

         QAssert.AreNotSame(vector, clone);
         QAssert.AreEqual(vector.Count, clone.Count);
         QAssert.CollectionAreEqual(vector, clone);
         vector[0] = 100;
         QAssert.CollectionAreNotEqual(vector, clone);
      }

      /// <summary>
      /// Test vectors equality.
      /// </summary>
      [Fact]
      public void testEquals()
      {
         Vector vector1 = new Vector(Data);
         Vector vector2 = new Vector(Data);
         Vector vector3 = new Vector(4);
         QAssert.IsTrue(vector1.Equals(vector1));
         QAssert.IsTrue(vector1.Equals(vector2));
         QAssert.IsFalse(vector1.Equals(vector3));
         QAssert.IsFalse(vector1.Equals(null));
         QAssert.IsFalse(vector1.Equals(2));
      }

      /// <summary>
      /// Test Vector hash code.
      /// </summary>
      [Fact]
      public void testHashCode()
      {
         Vector vector = new Vector(Data);
         QAssert.AreEqual(vector.GetHashCode(), vector.GetHashCode());
         QAssert.AreEqual(vector.GetHashCode(),
         new Vector(new List<double>() { 1, 2, 3, 4, 5  }).GetHashCode());
         QAssert.AreNotEqual(vector.GetHashCode(), new Vector(new List<double>() { 1 }).GetHashCode());
      }
   }
}
