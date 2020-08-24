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
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace QLCore
{
   // Base on Sergey Teplyakov code
   // https://blogs.msdn.microsoft.com/seteplia/2017/02/01/dissecting-the-new-constraint-in-c-a-perfect-example-of-a-leaky-abstraction/
   //
   public static class FastActivator<T> where T : new ()
   {
      /// <summary>
      /// Extremely fast generic factory method that returns an instance
      /// of the type <typeparam name="T"/>.
      /// </summary>
      public static readonly Func<T> Create = DynamicModuleLambdaCompiler.GenerateFactory<T>();
   }

   public static class DynamicModuleLambdaCompiler
   {
      public static Func<T> GenerateFactory<T>() where T : new ()
      {
         Expression<Func<T>> expr = () => new T();
         NewExpression newExpr = (NewExpression)expr.Body;

         var method = new DynamicMethod(
            name: "lambda",
            returnType: newExpr.Type,
            parameterTypes: new Type[0],
            m: typeof(DynamicModuleLambdaCompiler).GetTypeInfo().Module,
            skipVisibility: true);

         ILGenerator ilGen = method.GetILGenerator();
         // Constructor for value types could be null
         if (newExpr.Constructor != null)
         {
            ilGen.Emit(OpCodes.Newobj, newExpr.Constructor);
         }
         else
         {
            LocalBuilder temp = ilGen.DeclareLocal(newExpr.Type);
            ilGen.Emit(OpCodes.Ldloca, temp);
            ilGen.Emit(OpCodes.Initobj, newExpr.Type);
            ilGen.Emit(OpCodes.Ldloc, temp);
         }

         ilGen.Emit(OpCodes.Ret);

         return (Func<T>)method.CreateDelegate(typeof(Func<T>));
      }
   }

}
