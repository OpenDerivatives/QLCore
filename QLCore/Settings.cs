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
   // we need only one instance of the class
   // we can not derive it from IObservable because the class is static
   public class Settings : Singleton<Settings>
   {
      public Settings() {}

      private Date evaluationDate_;
      private bool includeReferenceDateEvents_;
      private bool enforcesTodaysHistoricFixings_;
      private bool? includeTodaysCashFlows_;

      public Date evaluationDate()
      {
         if (evaluationDate_ == null)
            evaluationDate_ = new Date(Date.Today);
         return evaluationDate_;
      }


      public void setEvaluationDate(Date d)
      {
         evaluationDate_ = d;
      }

      public bool enforcesTodaysHistoricFixings
      {
         get
         {
            return enforcesTodaysHistoricFixings_;
         }
         set
         {
            enforcesTodaysHistoricFixings_ = value;
         }
      }

      public bool includeReferenceDateEvents
      {
         get
         {
            return includeReferenceDateEvents_;
         }
         set
         {
            includeReferenceDateEvents_ = value;
         }
      }

      public bool? includeTodaysCashFlows
      {
         get
         {
            return includeTodaysCashFlows_;
         }
         set
         {
            includeTodaysCashFlows_ = value;
         }
      }
   }
      

   // helper class to temporarily and safely change the settings
   public class SavedSettings : IDisposable
   {
      private Date evaluationDate_;
      private bool enforcesTodaysHistoricFixings_;
      private bool includeReferenceDateEvents_;
      private bool? includeTodaysCashFlows_;

      public SavedSettings()
      {
         evaluationDate_ = Settings.Instance.evaluationDate();
         enforcesTodaysHistoricFixings_ = Settings.Instance.enforcesTodaysHistoricFixings;
         includeReferenceDateEvents_ = Settings.Instance.includeReferenceDateEvents;
         includeTodaysCashFlows_ = Settings.Instance.includeTodaysCashFlows;
      }

      public void Dispose()
      {
         if (evaluationDate_ != Settings.Instance.evaluationDate())
            Settings.Instance.setEvaluationDate(evaluationDate_);
         Settings.Instance.enforcesTodaysHistoricFixings = enforcesTodaysHistoricFixings_;
         Settings.Instance.includeReferenceDateEvents = includeReferenceDateEvents_;
         Settings.Instance.includeTodaysCashFlows = includeTodaysCashFlows_;
      }
   }
}
