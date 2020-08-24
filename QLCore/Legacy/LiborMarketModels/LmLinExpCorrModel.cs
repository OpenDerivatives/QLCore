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

namespace QLCore
{
   //! %linear exponential correlation model
   /*! This class describes a exponential correlation model

       References:

       Damiano Brigo, Fabio Mercurio, Massimo Morini, 2003,
       Different Covariance Parameterizations of Libor Market Model and Joint
       Caps/Swaptions Calibration,
       (<http://www.business.uts.edu.au/qfrc/conferences/qmf2001/Brigo_D.pdf>)
   */
   public class LmLinearExponentialCorrelationModel : LmCorrelationModel
   {
      public LmLinearExponentialCorrelationModel(int size, double rho, double beta, int? factors = null)
      : base(size, 2)
      {
         corrMatrix_ = new Matrix(size, size);
         factors_ = factors ?? size;
         arguments_[0] = new ConstantParameter(rho, new BoundaryConstraint(-1.0, 1.0));
         arguments_[1] = new ConstantParameter(beta, new PositiveConstraint());
         generateArguments();
      }

      public override Matrix correlation(double t, Vector x = null)
      {
         Matrix tmp = new Matrix(corrMatrix_);
         return tmp;
      }

      public override Matrix pseudoSqrt(double t, Vector x = null)
      {
         Matrix tmp = new Matrix(pseudoSqrt_);
         return tmp;
      }

      public override double correlation(int i, int j, double t, Vector x = null)
      {
         return corrMatrix_[i, j];
      }

      public override int factors() { return factors_; }

      public override bool isTimeIndependent()
      {
         return true;
      }

      protected override void generateArguments()
      {
         double rho = arguments_[0].value(0.0);
         double beta = arguments_[1].value(0.0);

         for (int i = 0; i < size_; ++i)
         {
            for (int j = i; j < size_; ++j)
            {
               corrMatrix_[i, j] = corrMatrix_[j, i]
                                   = rho + (1 - rho) * Math.Exp(-beta * Math.Abs((double) i - (double) j));
            }
         }

         pseudoSqrt_ = MatrixUtilitites.rankReducedSqrt(corrMatrix_, factors_, 1.0, MatrixUtilitites.SalvagingAlgorithm.None);
         corrMatrix_ = pseudoSqrt_ * Matrix.transpose(pseudoSqrt_);
      }

      private Matrix corrMatrix_, pseudoSqrt_;
      private int factors_;
   }
}
