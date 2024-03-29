﻿/*
 Copyright (C) 2020 Jean-Camille Tournier (jean-camille.tournier@avivainvestors.com)

 This file is part of QLCore Project https://github.com/QLFramework/QLCore

 QLCore is free software: you can redistribute it and/or modify it
 under the terms of the QLCore and QLNet license. You should have received a
 copy of the license along with this program; if not, license is
 available at https://github.com/QLFramework/QLCore/LICENSE.

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
   public interface Curve<T> : ITraits<T>, InterpolatedCurve
       where T : TermStructure
   {
      #region ITraits

      //protected
      ITraits<T> traits_ { get; }
      #endregion

      #region InterpolatedCurve
      #endregion

      List<BootstrapHelper<T>> instruments_ { get; }
      void setTermStructure(BootstrapHelper<T> helper);

      double accuracy_ { get; }
      bool moving_ { get; }
      Date initialDate();
      double timeFromReference(Date d);
      double initialValue();
   }

}