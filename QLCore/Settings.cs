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
   public class Settings : IDisposable
   {
      public Settings() {}

      private ThreadLocal<Date> evaluationDate_;
      private ThreadLocal<bool> includeReferenceDateEvents_;
      private ThreadLocal<bool> enforcesTodaysHistoricFixings_;
      private ThreadLocal<bool?> includeTodaysCashFlows_;

      public Date evaluationDate()
      {
         if (evaluationDate_ == null || evaluationDate_.Value == null)
            evaluationDate_ = new ThreadLocal<Date>(() => Date.Today);

         return evaluationDate_.Value;
      }

      public void setEvaluationDate(Date d)
      {
          if (evaluationDate_ == null || evaluationDate_.Value == null)
            evaluationDate_ = new ThreadLocal<Date>(() => Date.Today);

         evaluationDate_.Value = d;
      }

      public bool enforcesTodaysHistoricFixings
      {
         get
         {
             if (enforcesTodaysHistoricFixings_ == null)
               enforcesTodaysHistoricFixings_ = new ThreadLocal<bool>(() => false);

            return enforcesTodaysHistoricFixings_.Value;
         }
         set
         {
            if (enforcesTodaysHistoricFixings_ == null)
               enforcesTodaysHistoricFixings_ = new ThreadLocal<bool>(() => false);

            enforcesTodaysHistoricFixings_.Value = value;
         }
      }

      public bool includeReferenceDateEvents
      {
         get
         {
            if (includeReferenceDateEvents_ == null)
               includeReferenceDateEvents_ = new ThreadLocal<bool>(() => false);

            return includeReferenceDateEvents_.Value;
         }
         set
         {
            if (includeReferenceDateEvents_ == null)
               includeReferenceDateEvents_ = new ThreadLocal<bool>(() => false);

            includeReferenceDateEvents_.Value = value;
         }
      }

      public bool? includeTodaysCashFlows
      {
         get
         {
            if (includeTodaysCashFlows_ == null)
               includeTodaysCashFlows_ = new ThreadLocal<bool?>(() => false);

            return includeTodaysCashFlows_.Value;
         }
         set
         {
            if (includeTodaysCashFlows_ == null)
               includeTodaysCashFlows_ = new ThreadLocal<bool?>(() => false);

            includeTodaysCashFlows_.Value = value;
         }
      }

      public void Dispose()
      {
         if (evaluationDate_ != null)
            evaluationDate_.Dispose();
         
         if (enforcesTodaysHistoricFixings_ != null)
            enforcesTodaysHistoricFixings_.Dispose();
         
         if (includeReferenceDateEvents_ != null)
            includeReferenceDateEvents_.Dispose();

         if (includeTodaysCashFlows_ != null)
            includeTodaysCashFlows_.Dispose();
      }
   }
}
