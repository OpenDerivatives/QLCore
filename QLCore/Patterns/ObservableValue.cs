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
   //! %observable and assignable proxy to concrete value
   /*! Observers can be registered with instances of this class so
       that they are notified when a different value is assigned to
       such instances. Client code can copy the contained value or
       pass it to functions via implicit conversion.
       \note it is not possible to call non-const method on the
             returned value. This is by design, as this possibility
             would necessarily bypass the notification code; client
             code should modify the value via re-assignment instead.
   */
   public class ObservableValue<T> : IObservable where T : new ()
   {
      private ThreadLocal<T> value_;

      public ObservableValue()
      {
         value_ = new ThreadLocal<T>(() => { return FastActivator<T>.Create(); });
      }

      public ObservableValue(T t)
      {
         value_ = new ThreadLocal<T>(() => { return t; });
      }

      public ObservableValue(ObservableValue<T> t)
      {
         value_ = new ThreadLocal<T>(() => { return t.value_.Value; });
      }

      // controlled assignment
      public ObservableValue<T> Assign(T t)
      {
         value_.Value = t;
         notifyObservers();
         return this;
      }

      public ObservableValue<T> Assign(ObservableValue<T> t)
      {
         value_ = t.value_;
         notifyObservers();
         return this;
      }

      public static implicit operator ObservableValue<T>(T d)
      {
         ObservableValue<T> r = new ObservableValue<T>();
         r.Assign(d);
         return r;
      }

      public static implicit operator T(ObservableValue<T> d)
      {
         return d.value();
      }

      //! explicit inspector
      public T value() 
      { 
         return value_.Value; 
      }


      // Subjects, i.e. observables, should define interface internally like follows.
      private readonly ThreadLocal<WeakEventSource> eventSource = new ThreadLocal<WeakEventSource>(() => { return new WeakEventSource(); });
      public event Callback notifyObserversEvent
      {
         add
         {
            eventSource.Value.Subscribe(value);
         }
         remove
         {
            eventSource.Value.Unsubscribe(value);
         }
      }

      public void registerWith(Callback handler) { notifyObserversEvent += handler; }
      public void unregisterWith(Callback handler) { notifyObserversEvent -= handler; }
      protected void notifyObservers()
      {
         eventSource.Value.Raise();
      }
   }
}
