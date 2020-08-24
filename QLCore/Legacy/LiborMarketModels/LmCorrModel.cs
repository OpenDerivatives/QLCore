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
   // libor forward correlation model
   public abstract class LmCorrelationModel
   {
      protected LmCorrelationModel(int size, int nArguments)
      {
         size_ = size;
         arguments_ = new InitializedList<Parameter>(nArguments);
      }

      public virtual int size()
      {
         return size_;
      }

      public virtual int factors()
      {
         return size_;
      }

      public List<Parameter> parameters()
      {
         return arguments_;
      }

      public void setParams(List<Parameter> arguments)
      {
         arguments_ = arguments;
         generateArguments();
      }

      public abstract Matrix correlation(double t, Vector x = null);

      public virtual double correlation(int i, int j, double t, Vector x = null)
      {
         // inefficient implementation, please overload in derived classes
         return correlation(t, x)[i, j];
      }

      public virtual Matrix pseudoSqrt(double t, Vector x = null)
      {
         return MatrixUtilitites.pseudoSqrt(this.correlation(t, x),
                                            MatrixUtilitites.SalvagingAlgorithm.Spectral);
      }

      public virtual bool isTimeIndependent()
      {
         return false;
      }

      protected abstract void generateArguments();

      protected int size_;
      protected List<Parameter> arguments_;


   }
}
