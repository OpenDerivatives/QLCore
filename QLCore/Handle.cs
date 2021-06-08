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
   //! Shared handle to an observable
   /*! All copies of an instance of this class refer to the same observable by means of a relinkable smart pointer. When such
       pointer is relinked to another observable, the change will be propagated to all the copies.
       <tt>registerAsObserver</tt> is not needed since C# does automatic garbage collection */

   public class Handle<T>
   {
      protected T link_;

      public Handle() : this(default(T)) { }

      public Handle(T h)
      {
         link_ = h;
      }

      //! dereferencing
      public T currentLink() { return link; }

      // this one is instead of c++ -> and () operators overload
      public static implicit operator T(Handle<T> ImpliedObject) { return ImpliedObject.link; }

      public T link
      {
         get
         {
            Utils.QL_REQUIRE(!empty(), () => "empty Handle cannot be dereferenced");
            return link_;
         }
      }

      //! checks if the contained shared pointer points to anything
      public bool empty() { return link_ == null; }

      #region operator overload

      public static bool operator ==(Handle<T> here, Handle<T> there)
      {
         if (ReferenceEquals(here, there))
            return true;
         if ((object)here == null || (object)there == null)
            return false;
         return here.Equals(there);
      }

      public static bool operator !=(Handle<T> here, Handle<T> there)
      {
         return !(here == there);
      }

      public override bool Equals(object o)
      {
         return this == (Handle<T>)o;
      }

      public override int GetHashCode() { return ToString().GetHashCode(); }

      #endregion operator overload
   }

   //! Relinkable handle to an observable
   /*! An instance of this class can be relinked so that it points to another observable. The change will be propagated to all
       handles that were created as copies of such instance. */

   public class RelinkableHandle<T> : Handle<T>
   {
      public RelinkableHandle() : base(default(T)) { }

      public RelinkableHandle(T h) : base(h) { }

      public void linkTo(T h)
      {
         link_ = h;
      }
   }
}
