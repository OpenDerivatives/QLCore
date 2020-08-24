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
using component = System.Collections.Generic.KeyValuePair<QLCore.Instrument, double>;

namespace QLCore
{
   //! %Composite instrument
   /*! This instrument is an aggregate of other instruments. Its NPV
       is the sum of the NPVs of its components, each possibly
       multiplied by a given factor.

       \warning Methods that drive the calculation directly (such as
                recalculate(), freeze() and others) might not work
                correctly.

       \ingroup instruments
   */
   public class CompositeInstrument : Instrument
   {
      //! adds an instrument to the composite
      public void add
         (Instrument instrument, double multiplier = 1.0)
      {
         components_.Add(new KeyValuePair<Instrument, double>(instrument, multiplier));
         instrument.registerWith(update);
         update();
      }

      //! shorts an instrument from the composite
      public void subtract(Instrument instrument, double multiplier = 1.0)
      {
         add
            (instrument, -multiplier);
      }
      // Instrument interface
      public override bool isExpired()
      {
         foreach (component c in components_)
         {
            if (!c.Key.isExpired())
               return false;
         }
         return true;
      }

      protected override void performCalculations()
      {
         NPV_ = 0.0;
         foreach (component c in components_)
         {
            NPV_ += c.Value * c.Key.NPV();
         }
      }

      private List<component> components_ = new List<component>();

   }
}
