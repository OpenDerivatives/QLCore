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
using System.Threading;

namespace QLCore
{
   public class Singleton<T> where T : Singleton<T>, new()
   {
      private Lazy<T> instance = null;
      private static readonly ThreadLocal<Singleton<T>> threadLocal_ = new ThreadLocal<Singleton<T>>( () => { return new Singleton<T>()
                                                                                                                        {
                                                                                                                           instance = new Lazy<T>(() => FastActivator<T>.Create())
                                                                                                                        };
                                                                                                            });

      protected Singleton() { }

      public static T Instance
      {
         get
         {                
            return threadLocal_.Value.instance.Value;
         }
      }
   }
}