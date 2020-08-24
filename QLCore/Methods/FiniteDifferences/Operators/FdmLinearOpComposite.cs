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
   public abstract class FdmLinearOpComposite : FdmLinearOp
   {
      //! Time \f$t1 <= t2\f$ is required
      public abstract void setTime(double t1, double t2);

      public abstract Vector apply_mixed(Vector r);

      public abstract Vector apply_direction(int direction, Vector r);
      public abstract Vector solve_splitting(int direction, Vector r, double s);
      public abstract Vector preconditioner(Vector r, double s);

      public virtual List<SparseMatrix> toMatrixDecomp()
      {
         return null;
      }

      public override SparseMatrix toMatrix()
      {
         List<SparseMatrix> dcmp = toMatrixDecomp();
         SparseMatrix retVal = dcmp.accumulate(1, dcmp.Count, dcmp.First(), (a, b) => a + b);
         return retVal;
      }
   }
}
