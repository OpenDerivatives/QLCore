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

namespace QLCore
{
   public class FirstDerivativeOp : TripleBandLinearOp
   {
      public FirstDerivativeOp(FirstDerivativeOp rhs)
         : base(rhs.direction_, rhs.mesher_)
      {
         lower_ = rhs.lower_;
         diag_ = rhs.diag_;
         upper_ = rhs.upper_;
      }
      public FirstDerivativeOp(int direction, FdmMesher mesher)
         : base(direction, mesher)
      {
         FdmLinearOpLayout layout = mesher.layout();
         FdmLinearOpIterator endIter = layout.end();

         for (FdmLinearOpIterator iter = layout.begin(); iter != endIter; ++iter)
         {
            int i = iter.index();
            double? hm = mesher.dminus(iter, direction_);
            double? hp = mesher.dplus(iter, direction_);

            double? zetam1 = hm * (hm + hp);
            double? zeta0 = hm * hp;
            double? zetap1 = hp * (hm + hp);

            if (iter.coordinates()[direction_] == 0)
            {
               //upwinding scheme
               lower_[i] = 0.0;
               diag_[i] = -(upper_[i] = 1 / hp.Value);
            }
            else if (iter.coordinates()[direction_]
                     == layout.dim()[direction] - 1)
            {
               // downwinding scheme
               lower_[i] = -(diag_[i] = 1 / hm.Value);
               upper_[i] = 0.0;
            }
            else
            {
               lower_[i] = -hp.Value / zetam1.Value;
               diag_[i] = (hp.Value - hm.Value) / zeta0.Value;
               upper_[i] = hm.Value / zetap1.Value;
            }
         }
      }
   }
}
