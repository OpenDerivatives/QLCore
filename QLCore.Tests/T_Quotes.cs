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
using Xunit;
using QLCore;

namespace TestSuite
{


   public class T_Quotes
   {
      double add10(double x) { return x + 10; }
      double mul10(double x) { return x * 10; }
      double sub10(double x) { return x - 10; }

      double add
         (double x, double y) { return x + y; }
      double mul(double x, double y) { return x * y; }
      double sub(double x, double y) { return x - y; }

      [Fact]
      public void testDerived()
      {

         // Testing derived quotes

         Func<double, double>[] f = {add10, mul10, sub10};

         Quote me = new SimpleQuote(17.0);
         Handle<Quote> h = new Handle<Quote>(me);

         for (int i = 0; i < 3; i++)
         {
            DerivedQuote derived = new DerivedQuote(h, f[i]);
            double x = derived.value(),
                   y = f[i](me.value());
            if (Math.Abs(x - y) > 1.0e-10)
               QAssert.Fail("derived quote yields " + x + "function result is " + y);
         }

      }

      [Fact]
      public void testComposite()
      {
         // Testing composite quotes

         Func<double, double, double >[] f = { add, mul, sub };

         Quote me1 = new SimpleQuote(12.0),
         me2 = new SimpleQuote(13.0);
         Handle<Quote> h1 = new Handle<Quote>(me1),
         h2 = new Handle<Quote>(me2);

         for (int i = 0; i < 3; i++)
         {
            CompositeQuote composite = new CompositeQuote(h1, h2, f[i]);
            double x = composite.value(),
                   y = f[i](me1.value(), me2.value());
            if (Math.Abs(x - y) > 1.0e-10)
               QAssert.Fail("composite quote yields " + x + "function result is " + y);
         }
      }


   }
}
