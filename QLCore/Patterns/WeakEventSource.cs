/*
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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QLCore
{
   public class WeakEventSource
   {
      private readonly List<WeakDelegate> _handlers = new List<WeakDelegate>();

      public WeakEventSource()
      { }

      public void Raise()
      {
         lock (_handlers)
         {
            _handlers.RemoveAll(h => !h.Invoke());
         }
      }

      public void Subscribe(Callback handler)
      {
         lock (_handlers)
         {
            var weakHandlers = handler
                            .GetInvocationList()
                            .Select(d => new WeakDelegate(d))
                            .ToList();

            _handlers.AddRange(weakHandlers);
         }
      }

      public void Unsubscribe(Callback handler)
      {
         lock (_handlers)
         {
            int index = _handlers.FindIndex(h => h.IsMatch(handler));
            if (index >= 0)
               _handlers.RemoveAt(index);
         }
      }

      public void Clear()
      {
         lock (_handlers)
         {
            _handlers.Clear();
         }
      }

      private class WeakDelegate
      {
         #region Open handler generation and cache

         private delegate void OpenEventHandler(object target);

         // ReSharper disable once StaticMemberInGenericType (by design)
         private readonly ConcurrentDictionary<MethodInfo, OpenEventHandler> _openHandlerCache =
            new ConcurrentDictionary<MethodInfo, OpenEventHandler>();

         private static OpenEventHandler CreateOpenHandler(MethodInfo method)
         {
            var target = Expression.Parameter(typeof(object), "target");

            if (method.IsStatic)
            {
               var expr = Expression.Lambda<OpenEventHandler>(
                             Expression.Call(
                                method),
                             target);
               return expr.Compile();
            }
            else
            {
               var expr = Expression.Lambda<OpenEventHandler>(
                             Expression.Call(
                                Expression.Convert(target, method.DeclaringType),
                                method),
                             target);
               return expr.Compile();
            }
         }

         #endregion Open handler generation and cache

         private readonly WeakReference _weakTarget;
         private readonly MethodInfo _method;
         private readonly OpenEventHandler _openHandler;

         public WeakDelegate(Delegate handler)
         {
            _weakTarget = handler.Target != null ? new WeakReference(handler.Target) : null;
            _method = handler.GetMethodInfo();
            _openHandler = _openHandlerCache.GetOrAdd(_method, CreateOpenHandler);
         }

         public bool Invoke()
         {
            object target = null;
            if (_weakTarget != null)
            {
               target = _weakTarget.Target;
               if (target == null)
                  return false;
            }
            _openHandler(target);
            return true;
         }

         public bool IsMatch(Callback handler)
         {
            return _weakTarget.Target != null && (ReferenceEquals(handler.Target, _weakTarget.Target)
                                                  && handler.GetMethodInfo().Equals(_method));
         }
      }
   }
}
