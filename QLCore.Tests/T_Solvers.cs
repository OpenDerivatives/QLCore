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

using System;
using Xunit;
using QLCore;

namespace TestSuite
{

   public class T_Solvers
   {
      class Foo : ISolver1d
      {
         public override double value(double x) { return x * x - 1.0; }
         public override double derivative(double x) { return 2.0 * x; }
      }

      protected void test(Solver1D solver, string name)
      {
         double[] accuracy = new double[] { 1.0e-4, 1.0e-6, 1.0e-8 };
         double expected = 1.0;
         for (int i = 0; i < accuracy.Length; i++)
         {
            double root = solver.solve(new Foo(), accuracy[i], 1.5, 0.1);
            if (Math.Abs(root - expected) > accuracy[i])
            {
               QAssert.Fail(name + " solver:\n"
                            + "    expected:   " + expected + "\n"
                            + "    calculated: " + root + "\n"
                            + "    accuracy:   " + accuracy[i]);
            }
            root = solver.solve(new Foo(), accuracy[i], 1.5, 0.0, 1.0);
            if (Math.Abs(root - expected) > accuracy[i])
            {
               QAssert.Fail(name + " solver (bracketed):\n"
                            + "    expected:   " + expected + "\n"
                            + "    calculated: " + root + "\n"
                            + "    accuracy:   " + accuracy[i]);
            }
         }
      }

      [Fact]
      public void testBrent()
      {
         test(new Brent(), "Brent");
      }
      [Fact]
      public void testNewton()
      {
         test(new Newton(), "Newton");
      }
      [Fact]
      public void testFalsePosition()
      {
         test(new FalsePosition(), "FalsePosition");
      }
      [Fact]
      public void testBisection()
      {
         test(new Bisection(), "Bisection");
      }
      [Fact]
      public void testRidder()
      {
         test(new Ridder(), "Ridder");
      }
      [Fact]
      public void testSecant()
      {
         test(new Secant(), "Secant");
      }
   }
}
