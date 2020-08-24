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
   //! Multi-dimensional Conjugate Gradient class.
   /*! Fletcher-Reeves-Polak-Ribiere algorithm
       adapted from Numerical Recipes in C, 2nd edition.

       User has to provide line-search method and optimization end criteria.

       This optimization method requires the knowledge of
       the gradient of the cost function.

       \ingroup optimizers
   */
   public class ConjugateGradient : LineSearchBasedMethod
   {
      public ConjugateGradient(LineSearch lineSearch = null)
         : base(lineSearch)
      {}

      protected override Vector getUpdatedDirection(Problem P, double gold2, Vector gradient)
      {
         return lineSearch_.lastGradient() * -1 +
                (P.gradientNormValue() / gold2) * lineSearch_.searchDirection;
      }
   }
}