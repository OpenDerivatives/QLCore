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
   //! Crank-Nicolson scheme for finite difference methods
   /*! In this implementation, the passed operator must be derived
       from either TimeConstantOperator or TimeDependentOperator.
       Also, it must implement at least the following interface:

       \warning The differential operator must be linear for
                this evolver to work.

       \ingroup findiff
   */
   public class CrankNicolson<Operator> : MixedScheme<Operator>, ISchemeFactory where Operator : IOperator
   {
      // constructors
      public CrankNicolson() { }  // required for generics
      public CrankNicolson(Operator L, List<BoundaryCondition<IOperator>> bcs)
         : base(L, 0.5, bcs) { }

      public IMixedScheme factory(object L, object bcs, object[] additionalFields = null)
      {
         return new CrankNicolson<Operator>((Operator)L, (List<BoundaryCondition<IOperator>>)bcs);
      }
   }
}
