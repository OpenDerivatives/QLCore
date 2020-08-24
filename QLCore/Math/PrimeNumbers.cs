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
using System.Linq;


// ===========================================================================
// NOTE: The following copyright notice applies to the original code,
//
// Copyright (C) 2002 Peter Jäckel "Monte Carlo Methods in Finance".
// All rights reserved.
//
// Permission to use, copy, modify, and distribute this software is freely
// granted, provided that this notice is preserved.
// ===========================================================================

namespace QLCore
{
   //! Prime numbers calculator
   /*! Taken from "Monte Carlo Methods in Finance", by Peter Jäckel
    */
   public class PrimeNumbers
   {
      //! Get and store one after another.

      private static readonly ulong[] firstPrimes =
      {
         // the first two primes are mandatory for bootstrapping
         2,  3,
         // optional additional precomputed primes
         5,  7, 11, 13, 17, 19, 23, 29,
         31, 37, 41, 43, 47
      };

      private static List<ulong> primeNumbers_ = new List<ulong>();

      private PrimeNumbers()
      {}

      public static ulong get(int absoluteIndex)
      {
         if (primeNumbers_.empty())
         {
            lock (primeNumbers_)
            {
               primeNumbers_.AddRange(firstPrimes);
            }
         }

         while (primeNumbers_.Count <= absoluteIndex)
            nextPrimeNumber();

         return primeNumbers_[absoluteIndex];
      }

      private static ulong nextPrimeNumber()
      {
         lock (primeNumbers_)
         {
            ulong p, n, m = primeNumbers_.Last();
            do
            {
               // skip the even numbers
               m += 2;
               n = (ulong)Math.Sqrt((double)m);
               // i=1 since the even numbers have already been skipped
               int i = 1;
               do
               {
                  p = primeNumbers_[i];
                  ++i;
               }
               while ((m % p != 0) && p <= n);
            }
            while (p <= n);
            primeNumbers_.Add(m);
            return m;
         }
      }
   }
}
